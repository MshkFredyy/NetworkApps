using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace CLserv
{
    public partial class Form1 : Form
    {
        private TcpListener tcpListener;
        private Thread listenerThread;
        private bool isRunning = false;

        // Структуры для управления чатом
        private Dictionary<string, List<ChatClient>> groups = new Dictionary<string, List<ChatClient>>();
        private Dictionary<string, string> userGroups = new Dictionary<string, string>();
        private object groupsLock = new object();

        private class ChatClient
        {
            public TcpClient Client { get; set; }
            public NetworkStream Stream { get; set; }
            public string Username { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void FormServer_Load(object sender, EventArgs e)
        {
            txtServerPath.Text = Path.Combine(Application.StartupPath, "ServerFiles");
            if (!Directory.Exists(txtServerPath.Text))
                Directory.CreateDirectory(txtServerPath.Text);
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                try
                {
                    int port = int.Parse(txtPort.Text);
                    tcpListener = new TcpListener(IPAddress.Any, port);
                    listenerThread = new Thread(new ThreadStart(ListenForClients));
                    listenerThread.IsBackground = true;
                    listenerThread.Start();

                    isRunning = true;
                    btnStartServer.Text = "Остановить сервер";
                    AddLog($"Сервер запущен на порту {port}");
                    AddLog($"Папка для файлов: {txtServerPath.Text}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка запуска сервера: {ex.Message}");
                }
            }
            else
            {
                StopServer();
            }
        }

        private void ListenForClients()
        {
            tcpListener.Start();

            while (isRunning)
            {
                try
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.IsBackground = true;
                    clientThread.Start(client);

                    AddLog($"Новое подключение: {client.Client.RemoteEndPoint}");
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (isRunning)
                        AddLog($"Ошибка при принятии соединения: {ex.Message}");
                }
            }
        }

        private void HandleClientComm(object clientObj)
        {
            TcpClient tcpClient = (TcpClient)clientObj;
            ChatClient chatClient = null;

            try
            {
                // Устанавливаем таймауты
                tcpClient.SendTimeout = 60000;
                tcpClient.ReceiveTimeout = 60000;

                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    // Получаем тип сообщения
                    byte[] messageTypeBytes = new byte[4];
                    int bytesRead = ReadFull(clientStream, messageTypeBytes, 4);

                    if (bytesRead == 0)
                    {
                        AddLog("Клиент отключился");
                        return;
                    }

                    if (bytesRead != 4)
                    {
                        AddLog("Не удалось получить тип сообщения");
                        return;
                    }

                    int messageType = BitConverter.ToInt32(messageTypeBytes, 0);
                    AddLog($"Получен запрос типа: {messageType}");

                    switch (messageType)
                    {
                        case 1: // Загрузка файла на сервер
                            ReceiveFile(clientStream);
                            break;
                        case 2: // Скачивание списка файлов
                            SendFileList(clientStream);
                            break;
                        case 3: // Скачивание файла с сервера
                            SendFile(clientStream);
                            break;
                        case 4: // Подключение к чату
                            chatClient = HandleChatConnection(clientStream, tcpClient);
                            if (chatClient != null)
                                HandleChatCommunication(chatClient);
                            break;
                 
                        default:
                            AddLog($"Неизвестный тип сообщения: {messageType}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка обработки клиента: {ex.Message}");
            }
            finally
            {
                if (chatClient != null)
                {
                    RemoveClientFromGroup(chatClient);
                }
                try
                {
                    tcpClient?.Close();
                }
                catch { }
            }
        }

        private ChatClient HandleChatConnection(NetworkStream stream, TcpClient client)
        {
            try
            {
                // Получаем длину имени пользователя
                byte[] usernameLengthBytes = new byte[4];
                if (ReadFull(stream, usernameLengthBytes, 4) != 4) return null;
                int usernameLength = BitConverter.ToInt32(usernameLengthBytes, 0);

                // Получаем имя пользователя
                byte[] usernameBytes = new byte[usernameLength];
                if (ReadFull(stream, usernameBytes, usernameLength) != usernameLength) return null;
                string username = Encoding.UTF8.GetString(usernameBytes);

                AddLog($"Новый пользователь чата: {username}");

                return new ChatClient
                {
                    Client = client,
                    Stream = stream,
                    Username = username
                };
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка обработки подключения к чату: {ex.Message}");
                return null;
            }
        }

        private void HandleChatCommunication(ChatClient chatClient)
        {
            while (isRunning && chatClient.Client.Connected)
            {
                try
                {
                    byte[] messageTypeBytes = new byte[4];
                    int bytesRead = ReadFull(chatClient.Stream, messageTypeBytes, 4);

                    if (bytesRead == 0) break;
                    if (bytesRead != 4) continue;

                    int messageType = BitConverter.ToInt32(messageTypeBytes, 0);

                    switch (messageType)
                    {
                        case 5: // Текстовое сообщение
                            HandleChatMessage(chatClient);
                            break;
                        case 6: // Создание/вход в группу
                            HandleGroupJoin(chatClient);
                            break;
                        case 7: // Выход из группы
                            HandleGroupLeave(chatClient);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (isRunning)
                        AddLog($"Ошибка обработки сообщения чата: {ex.Message}");
                    break;
                }
            }
        }

        private void HandleChatMessage(ChatClient sender)
        {
            try
            {
                // Получаем длину сообщения
                byte[] messageLengthBytes = new byte[4];
                if (ReadFull(sender.Stream, messageLengthBytes, 4) != 4) return;
                int messageLength = BitConverter.ToInt32(messageLengthBytes, 0);

                // Получаем сообщение
                byte[] messageBytes = new byte[messageLength];
                if (ReadFull(sender.Stream, messageBytes, messageLength) != messageLength) return;
                string message = Encoding.UTF8.GetString(messageBytes);

                // Получаем длину названия группы
                byte[] groupLengthBytes = new byte[4];
                if (ReadFull(sender.Stream, groupLengthBytes, 4) != 4) return;
                int groupLength = BitConverter.ToInt32(groupLengthBytes, 0);

                string groupName = "";
                if (groupLength > 0)
                {
                    byte[] groupBytes = new byte[groupLength];
                    if (ReadFull(sender.Stream, groupBytes, groupLength) == groupLength)
                    {
                        groupName = Encoding.UTF8.GetString(groupBytes);
                    }
                }

                AddLog($"Сообщение от {sender.Username} в группе '{groupName}': {message}");

                // Отправляем сообщение всем в группе (кроме отправителя)
                BroadcastToGroup(sender.Username, message, groupName, sender);
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка обработки сообщения от {sender.Username}: {ex.Message}");
            }
        }

        private void BroadcastToGroup(string sender, string message, string groupName, ChatClient excludeSender = null)
        {
            lock (groupsLock)
            {
                if (groups.ContainsKey(groupName))
                {
                    var deadClients = new List<ChatClient>();
                    int sentCount = 0;

                    foreach (var client in groups[groupName])
                    {
                        // Пропускаем отправителя, если указан
                        if (excludeSender != null && client.Username == excludeSender.Username)
                            continue;

                        try
                        {
                            // Проверяем, подключен ли клиент
                            if (client.Client == null || !client.Client.Connected)
                            {
                                deadClients.Add(client);
                                continue;
                            }

                            // Отправляем тип сообщения (5 - текстовое сообщение)
                            client.Stream.Write(BitConverter.GetBytes(5), 0, 4);

                            // Отправляем имя отправителя
                            byte[] senderBytes = Encoding.UTF8.GetBytes(sender);
                            client.Stream.Write(BitConverter.GetBytes(senderBytes.Length), 0, 4);
                            client.Stream.Write(senderBytes, 0, senderBytes.Length);

                            // Отправляем сообщение
                            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                            client.Stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, 4);
                            client.Stream.Write(messageBytes, 0, messageBytes.Length);

                            // Отправляем название группы
                            byte[] groupBytes = Encoding.UTF8.GetBytes(groupName);
                            client.Stream.Write(BitConverter.GetBytes(groupBytes.Length), 0, 4);
                            client.Stream.Write(groupBytes, 0, groupBytes.Length);

                            client.Stream.Flush();
                            sentCount++;

                            AddLog($"Сообщение отправлено клиенту {client.Username}");
                        }
                        catch (Exception ex)
                        {
                            AddLog($"Ошибка отправки сообщения клиенту {client.Username}: {ex.Message}");
                            deadClients.Add(client);
                        }
                    }

                    // Удаляем отключившихся клиентов
                    foreach (var deadClient in deadClients)
                    {
                        groups[groupName].Remove(deadClient);
                        if (userGroups.ContainsKey(deadClient.Username))
                            userGroups.Remove(deadClient.Username);
                    }

                    // Если группа пуста, удаляем ее
                    if (groups[groupName].Count == 0)
                    {
                        groups.Remove(groupName);
                    }

                    AddLog($"Сообщение отправлено {sentCount} клиентам в группе {groupName}");
                }
                else
                {
                    AddLog($"Группа {groupName} не найдена");
                }
            }
        }

        private void HandleGroupJoin(ChatClient client)
        {
            // Получаем флаг создания
            byte[] createFlagBytes = new byte[4];
            if (ReadFull(client.Stream, createFlagBytes, 4) != 4) return;
            bool createNew = BitConverter.ToInt32(createFlagBytes, 0) == 1;

            // Получаем название группы
            byte[] groupLengthBytes = new byte[4];
            if (ReadFull(client.Stream, groupLengthBytes, 4) != 4) return;
            int groupLength = BitConverter.ToInt32(groupLengthBytes, 0);

            byte[] groupBytes = new byte[groupLength];
            if (ReadFull(client.Stream, groupBytes, groupLength) != groupLength) return;
            string groupName = Encoding.UTF8.GetString(groupBytes);

            lock (groupsLock)
            {
                // Удаляем клиента из предыдущей группы
                RemoveClientFromGroup(client);

                // Создаем группу если нужно
                if (!groups.ContainsKey(groupName))
                {
                    if (createNew)
                    {
                        groups[groupName] = new List<ChatClient>();
                        SendGroupInfo(client, $"Группа '{groupName}' создана");
                        AddLog($"Создана новая группа: {groupName}");
                    }
                    else
                    {
                        SendGroupInfo(client, $"Группа '{groupName}' не существует");
                        return;
                    }
                }

                // Добавляем клиента в группу
                groups[groupName].Add(client);
                userGroups[client.Username] = groupName;

                SendGroupInfo(client, $"Вы вошли в группу '{groupName}'");

                // Рассылаем уведомление о входе ВСЕМ участникам группы (включая нового)
                BroadcastSystemMessage($"{client.Username} вошел в группу", groupName);

                // Обновляем список пользователей для всех в группе
                UpdateUserList(groupName);
            }

            AddLog($"Пользователь {client.Username} вошел в группу '{groupName}'");
        }

        private void HandleGroupLeave(ChatClient client)
        {
            lock (groupsLock)
            {
                RemoveClientFromGroup(client);
                SendGroupInfo(client, "Вы вышли из группы");
            }
        }

        private void RemoveClientFromGroup(ChatClient client)
        {
            if (userGroups.ContainsKey(client.Username))
            {
                string groupName = userGroups[client.Username];

                if (groups.ContainsKey(groupName))
                {
                    groups[groupName].Remove(client);
                    BroadcastSystemMessage($"{client.Username} вышел из группы", groupName);

                    // Если группа пуста, удаляем ее
                    if (groups[groupName].Count == 0)
                    {
                        groups.Remove(groupName);
                    }
                    else
                    {
                        UpdateUserList(groupName);
                    }
                }

                userGroups.Remove(client.Username);
            }
        }

        
        private void BroadcastSystemMessage(string message, string groupName)
        {
            lock (groupsLock)
            {
                if (groups.ContainsKey(groupName))
                {
                    var deadClients = new List<ChatClient>();

                    foreach (var client in groups[groupName])
                    {
                        try
                        {
                            // Отправляем тип сообщения (6 - системное сообщение)
                            client.Stream.Write(BitConverter.GetBytes(6), 0, 4);

                            // Отправляем сообщение
                            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                            client.Stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, 4);
                            client.Stream.Write(messageBytes, 0, messageBytes.Length);
                        }
                        catch
                        {
                            deadClients.Add(client);
                        }
                    }

                    // Удаляем отключившихся клиентов
                    foreach (var deadClient in deadClients)
                    {
                        groups[groupName].Remove(deadClient);
                        userGroups.Remove(deadClient.Username);
                    }
                }
            }
        }

        private void UpdateUserList(string groupName)
        {
            lock (groupsLock)
            {
                if (groups.ContainsKey(groupName))
                {
                    string userList = string.Join("|", groups[groupName].Select(c => c.Username));

                    var deadClients = new List<ChatClient>();
                    int updatedCount = 0;

                    foreach (var client in groups[groupName])
                    {
                        try
                        {
                            if (client.Client == null || !client.Client.Connected)
                            {
                                deadClients.Add(client);
                                continue;
                            }

                            // Отправляем тип сообщения (7 - список пользователей)
                            client.Stream.Write(BitConverter.GetBytes(7), 0, 4);

                            // Отправляем список
                            byte[] listBytes = Encoding.UTF8.GetBytes(userList);
                            client.Stream.Write(BitConverter.GetBytes(listBytes.Length), 0, 4);
                            client.Stream.Write(listBytes, 0, listBytes.Length);

                            client.Stream.Flush();
                            updatedCount++;
                        }
                        catch (Exception ex)
                        {
                            AddLog($"Ошибка обновления списка для {client.Username}: {ex.Message}");
                            deadClients.Add(client);
                        }
                    }

                    // Удаляем отключившихся клиентов
                    foreach (var deadClient in deadClients)
                    {
                        groups[groupName].Remove(deadClient);
                        userGroups.Remove(deadClient.Username);
                    }

                    AddLog($"Список пользователей обновлен для {updatedCount} клиентов в группе {groupName}");
                }
            }
        }

        private void SendGroupInfo(ChatClient client, string message)
        {
            try
            {
                // Отправляем тип сообщения (6 - системное сообщение)
                client.Stream.Write(BitConverter.GetBytes(6), 0, 4);

                // Отправляем сообщение
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                client.Stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, 4);
                client.Stream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка отправки информации о группе: {ex.Message}");
            }
        }

        private void StopServer()
        {
            isRunning = false;
            try
            {
                tcpListener?.Stop();

                // Очищаем группы
                lock (groupsLock)
                {
                    groups.Clear();
                    userGroups.Clear();
                }
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка при остановке сервера: {ex.Message}");
            }

            btnStartServer.Text = "Запустить сервер";
            AddLog("Сервер остановлен");
        }

        private void AddLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AddLog), message);
                return;
            }

            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtLog.ScrollToCaret();
        }

        private void FormServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopServer();
        }

        private void ReceiveFile(NetworkStream clientStream)
        {
            try
            {
                // Получаем длину имени файла
                byte[] fileNameLengthBytes = new byte[4];
                int bytesRead = ReadFull(clientStream, fileNameLengthBytes, 4);
                if (bytesRead != 4)
                {
                    AddLog("Ошибка получения длины имени файла");
                    return;
                }

                int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);

                if (fileNameLength <= 0 || fileNameLength > 260)
                {
                    AddLog($"Некорректная длина имени файла: {fileNameLength}");
                    return;
                }

                // Получаем имя файла
                byte[] fileNameBytes = new byte[fileNameLength];
                bytesRead = ReadFull(clientStream, fileNameBytes, fileNameLength);
                if (bytesRead != fileNameLength)
                {
                    AddLog("Ошибка получения имени файла");
                    return;
                }

                string fileName = Encoding.UTF8.GetString(fileNameBytes);
                AddLog($"Прием файла: {fileName}");

                // Получаем длину файла
                byte[] fileLengthBytes = new byte[8];
                bytesRead = ReadFull(clientStream, fileLengthBytes, 8);
                if (bytesRead != 8)
                {
                    AddLog("Ошибка получения длины файла");
                    return;
                }

                long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                if (fileLength < 0)
                {
                    AddLog("Некорректная длина файла");
                    return;
                }

                if (fileLength > 1024 * 1024 * 1024)
                {
                    AddLog($"Файл слишком большой: {fileLength} байт");
                    return;
                }

                // Создаем безопасное имя файла
                string safeFileName = GetSafeFileName(fileName);
                string filePath = Path.Combine(txtServerPath.Text, safeFileName);

                // Получаем данные файла
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[8192];
                    long totalBytesRead = 0;
                    int chunkBytesRead;

                    while (totalBytesRead < fileLength)
                    {
                        int bytesToRead = (int)Math.Min(buffer.Length, fileLength - totalBytesRead);
                        chunkBytesRead = clientStream.Read(buffer, 0, bytesToRead);

                        if (chunkBytesRead == 0)
                        {
                            AddLog("Соединение разорвано при приеме файла");
                            break;
                        }

                        fileStream.Write(buffer, 0, chunkBytesRead);
                        totalBytesRead += chunkBytesRead;

                        // Логируем прогресс для больших файлов
                        if (fileLength > 1024 * 1024 && totalBytesRead % (1024 * 1024) == 0)
                        {
                            AddLog($"Принято {totalBytesRead / 1024 / 1024} МБ из {fileLength / 1024 / 1024} МБ");
                        }
                    }

                    if (totalBytesRead == fileLength)
                    {
                        AddLog($"Файл получен: {safeFileName} ({fileLength} байт)");
                    }
                    else
                    {
                        AddLog($"Файл получен не полностью: {safeFileName} ({totalBytesRead} из {fileLength} байт)");
                        try { File.Delete(filePath); } catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка приема файла: {ex.Message}");
            }
        }

        private void SendFileList(NetworkStream clientStream)
        {
            try
            {
                string[] files = Directory.GetFiles(txtServerPath.Text);

                // Получаем только имена файлов
                var fileNames = files.Select(f => Path.GetFileName(f))
                                    .Where(name => !string.IsNullOrEmpty(name))
                                    .ToArray();

                string fileList = string.Join("|", fileNames);
                byte[] fileListBytes = Encoding.UTF8.GetBytes(fileList);

                // Отправляем длину списка файлов
                clientStream.Write(BitConverter.GetBytes(fileListBytes.Length), 0, 4);

                // Отправляем сам список
                clientStream.Write(fileListBytes, 0, fileListBytes.Length);

                AddLog($"Отправлен список файлов: {fileNames.Length} файлов");
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка отправки списка файлов: {ex.Message}");

                // Отправляем пустой список в случае ошибки
                try
                {
                    byte[] emptyList = BitConverter.GetBytes(0);
                    clientStream.Write(emptyList, 0, 4);
                }
                catch { }
            }
        }

        private void SendFile(NetworkStream clientStream)
        {
            try
            {
                // Получаем длину имени файла
                byte[] fileNameLengthBytes = new byte[4];
                int bytesRead = ReadFull(clientStream, fileNameLengthBytes, 4);
                if (bytesRead != 4)
                {
                    AddLog("Ошибка получения длины имени файла");
                    return;
                }

                int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);

                if (fileNameLength <= 0 || fileNameLength > 260)
                {
                    AddLog($"Некорректная длина имени файла: {fileNameLength}");

                    clientStream.Write(BitConverter.GetBytes((long)-1), 0, 8);
                    return;
                }

                // Получаем имя файла
                byte[] fileNameBytes = new byte[fileNameLength];
                bytesRead = ReadFull(clientStream, fileNameBytes, fileNameLength);
                if (bytesRead != fileNameLength)
                {
                    AddLog("Ошибка получения имени файла");
                    clientStream.Write(BitConverter.GetBytes((long)-1), 0, 8);
                    return;
                }

                string fileName = Encoding.UTF8.GetString(fileNameBytes);
                string filePath = Path.Combine(txtServerPath.Text, fileName);

                AddLog($"Запрос файла: {fileName}");

                if (File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);

                    // Отправляем длину файла
                    byte[] fileLengthBytes = BitConverter.GetBytes(fileInfo.Length);
                    clientStream.Write(fileLengthBytes, 0, 8);

                    // Отправляем файл
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        byte[] buffer = new byte[8192];
                        int bytesReadFromFile;
                        long totalBytesSent = 0;

                        while ((bytesReadFromFile = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            clientStream.Write(buffer, 0, bytesReadFromFile);
                            totalBytesSent += bytesReadFromFile;

                            // Логируем прогресс для больших файлов
                            if (fileInfo.Length > 1024 * 1024 && totalBytesSent % (1024 * 1024) == 0)
                            {
                                AddLog($"Отправлено {totalBytesSent / 1024 / 1024} МБ из {fileInfo.Length / 1024 / 1024} МБ");
                            }
                        }

                        AddLog($"Файл отправлен: {fileName} ({totalBytesSent} байт)");
                    }
                }
                else
                {
                    // Отправляем -1 как признак отсутствия файла
                    clientStream.Write(BitConverter.GetBytes((long)-1), 0, 8);
                    AddLog($"Файл не найден: {fileName}");
                }
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка отправки файла: {ex.Message}");
            }
        }

        private int ReadFull(NetworkStream stream, byte[] buffer, int size)
        {
            int totalBytesRead = 0;
            int bytesRead;

            while (totalBytesRead < size)
            {
                try
                {
                    bytesRead = stream.Read(buffer, totalBytesRead, size - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        // Соединение закрыто
                        AddLog("Клиент отключился");
                        break;
                    }
                    totalBytesRead += bytesRead;
                }
                catch (IOException ex)
                {
                    // Таймаут или разрыв соединения
                    SocketException socketEx = ex.InnerException as SocketException;
                    if (socketEx != null)
                    {
                        if (socketEx.SocketErrorCode == SocketError.TimedOut)
                        {
                            AddLog("Таймаут при чтении данных");
                        }
                        else if (socketEx.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            AddLog("Соединение было разорвано клиентом");
                        }
                        else
                        {
                            AddLog($"Ошибка сокета: {socketEx.SocketErrorCode}");
                        }
                    }
                    else
                    {
                        AddLog($"Ошибка ввода-вывода: {ex.Message}");
                    }
                    break;
                }
                catch (ObjectDisposedException)
                {
                    // Поток закрыт
                    AddLog("Поток был закрыт");
                    break;
                }
                catch (Exception ex)
                {
                    AddLog($"Неожиданная ошибка при чтении: {ex.Message}");
                    break;
                }
            }

            return totalBytesRead;
        }

        private string GetSafeFileName(string fileName)
        {
            // Заменяем недопустимые символы в имени файла
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = new string(fileName.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());

            // Обрезаем длину имени
            if (safeName.Length > 100)
            {
                var extension = Path.GetExtension(safeName);
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(safeName);
                nameWithoutExtension = nameWithoutExtension.Substring(0, Math.Min(100 - extension.Length, nameWithoutExtension.Length));
                safeName = nameWithoutExtension + extension;
            }

            return safeName;
        }

        private void btnBrowsePath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Выберите папку для хранения файлов";
                folderDialog.SelectedPath = txtServerPath.Text;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtServerPath.Text = folderDialog.SelectedPath;
                    if (!Directory.Exists(txtServerPath.Text))
                        Directory.CreateDirectory(txtServerPath.Text);
                }
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(txtServerPath.Text))
                {
                    System.Diagnostics.Process.Start(txtServerPath.Text);
                }
                else
                {
                    MessageBox.Show("Папка не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия папки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }
    }
}
