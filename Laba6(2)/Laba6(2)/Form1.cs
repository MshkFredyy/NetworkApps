using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;


namespace Laba6_2_
{
    public partial class Form1 : Form
    {
        private string serverIP;
        private int serverPort;
        private TcpClient chatClient;
        private NetworkStream chatStream;
        private Thread chatListenerThread;
        private bool isConnectedToChat = false;
        private string currentGroup = "";
        private string username = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void FormClient_Load(object sender, EventArgs e)
        {
            txtClientPath.Text = Path.Combine(Application.StartupPath, "ClientFiles");
            if (!Directory.Exists(txtClientPath.Text))
                Directory.CreateDirectory(txtClientPath.Text);

            // Генерируем случайное имя пользователя
            username = "User" + new Random().Next(1000, 9999);
            txtUsername.Text = username;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                serverIP = txtServerIP.Text;
                serverPort = int.Parse(txtServerPort.Text);

                // Проверяем подключение
                using (TcpClient testClient = new TcpClient())
                {
                    testClient.Connect(serverIP, serverPort);
                }

                AddLog("Подключение к серверу успешно");
                btnRefreshFiles.Enabled = true;
                btnUpload.Enabled = true;
                btnDownload.Enabled = true;

                // Подключаемся к чату
                ConnectToChat();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }

        private void ConnectToChat()
        {
            try
            {
                chatClient = new TcpClient();
                chatClient.Connect(serverIP, serverPort);
                chatStream = chatClient.GetStream();

                // Отправляем тип сообщения (4 - подключение к чату)
                chatStream.Write(BitConverter.GetBytes(4), 0, 4);

                // Отправляем имя пользователя
                username = string.IsNullOrEmpty(txtUsername.Text) ? "User" + new Random().Next(1000, 9999) : txtUsername.Text;
                byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
                chatStream.Write(BitConverter.GetBytes(usernameBytes.Length), 0, 4);
                chatStream.Write(usernameBytes, 0, usernameBytes.Length);

                isConnectedToChat = true;

                // Запускаем поток для прослушивания сообщений
                chatListenerThread = new Thread(new ThreadStart(ListenForChatMessages));
                chatListenerThread.IsBackground = true;
                chatListenerThread.Start();

                btnSendMessage.Enabled = true;
                btnCreateGroup.Enabled = true;
                btnJoinGroup.Enabled = true;
                btnLeaveGroup.Enabled = true;

                AddChatLog("Подключение к чату успешно");
            }
            catch (Exception ex)
            {
                AddChatLog($"Ошибка подключения к чату: {ex.Message}");
            }
        }

        private void ListenForChatMessages()
        {
            while (isConnectedToChat && chatClient != null && chatClient.Connected)
            {
                try
                {
                    byte[] messageTypeBytes = new byte[4];
                    int bytesRead = ReadFull(chatStream, messageTypeBytes, 4);

                    if (bytesRead == 0) break;
                    if (bytesRead != 4) continue;

                    int messageType = BitConverter.ToInt32(messageTypeBytes, 0);

                    switch (messageType)
                    {
                        case 5: // Текстовое сообщение
                            ReceiveChatMessage();
                            break;
                        case 6: // Информация о группе
                            ReceiveGroupInfo();
                            break;
                        case 7: // Список пользователей
                            ReceiveUserList();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (isConnectedToChat)
                        AddChatLog($"Ошибка приема сообщения: {ex.Message}");
                    break;
                }
            }
        }

        private void ReceiveChatMessage()
        {
            try
            {
                // Получаем длину имени отправителя
                byte[] senderLengthBytes = new byte[4];
                if (ReadFull(chatStream, senderLengthBytes, 4) != 4) return;
                int senderLength = BitConverter.ToInt32(senderLengthBytes, 0);

                // Получаем имя отправителя
                byte[] senderBytes = new byte[senderLength];
                if (ReadFull(chatStream, senderBytes, senderLength) != senderLength) return;
                string sender = Encoding.UTF8.GetString(senderBytes);

                // Получаем длину сообщения
                byte[] messageLengthBytes = new byte[4];
                if (ReadFull(chatStream, messageLengthBytes, 4) != 4) return;
                int messageLength = BitConverter.ToInt32(messageLengthBytes, 0);

                // Получаем сообщение
                byte[] messageBytes = new byte[messageLength];
                if (ReadFull(chatStream, messageBytes, messageLength) != messageLength) return;
                string message = Encoding.UTF8.GetString(messageBytes);

                // Получаем длину названия группы
                byte[] groupLengthBytes = new byte[4];
                if (ReadFull(chatStream, groupLengthBytes, 4) != 4) return;
                int groupLength = BitConverter.ToInt32(groupLengthBytes, 0);

                string group = "";
                if (groupLength > 0)
                {
                    byte[] groupBytes = new byte[groupLength];
                    if (ReadFull(chatStream, groupBytes, groupLength) == groupLength)
                    {
                        group = Encoding.UTF8.GetString(groupBytes);
                    }
                }

                
                if (sender != username)
                {
                    AddChatLog($"[{group}] {sender}: {message}");
                }
            }
            catch (Exception ex)
            {
                AddChatLog($"Ошибка приема сообщения: {ex.Message}");
            }
        }

        private void ReceiveGroupInfo()
        {
            // Получаем длину сообщения
            byte[] messageLengthBytes = new byte[4];
            if (ReadFull(chatStream, messageLengthBytes, 4) != 4) return;
            int messageLength = BitConverter.ToInt32(messageLengthBytes, 0);

            // Получаем сообщение
            byte[] messageBytes = new byte[messageLength];
            if (ReadFull(chatStream, messageBytes, messageLength) != messageLength) return;
            string message = Encoding.UTF8.GetString(messageBytes);

            AddChatLog($"Система: {message}");
        }

        private void ReceiveUserList()
        {
            // Получаем длину списка
            byte[] listLengthBytes = new byte[4];
            if (ReadFull(chatStream, listLengthBytes, 4) != 4) return;
            int listLength = BitConverter.ToInt32(listLengthBytes, 0);

            if (listLength == 0) return;

            // Получаем список
            byte[] listBytes = new byte[listLength];
            if (ReadFull(chatStream, listBytes, listLength) != listLength) return;
            string userList = Encoding.UTF8.GetString(listBytes);

            UpdateUserList(userList);
        }

        private void UpdateUserList(string userList)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateUserList), userList);
                return;
            }

            lstUsers.Items.Clear();
            string[] users = userList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string user in users)
            {
                lstUsers.Items.Add(user);
            }
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            SendChatMessage();
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendChatMessage();
                e.Handled = true;
            }
        }

        private void SendChatMessage()
        {
            if (!isConnectedToChat || string.IsNullOrEmpty(txtMessage.Text)) return;

            try
            {
                // Отправляем тип сообщения (5 - текстовое сообщение)
                chatStream.Write(BitConverter.GetBytes(5), 0, 4);

                // Отправляем сообщение
                string message = txtMessage.Text;
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                chatStream.Write(BitConverter.GetBytes(messageBytes.Length), 0, 4);
                chatStream.Write(messageBytes, 0, messageBytes.Length);

                // Отправляем название группы (если есть)
                byte[] groupBytes = Encoding.UTF8.GetBytes(currentGroup);
                chatStream.Write(BitConverter.GetBytes(groupBytes.Length), 0, 4);
                chatStream.Write(groupBytes, 0, groupBytes.Length);

                AddChatLog($"Я: {message}");
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                AddChatLog($"Ошибка отправки сообщения: {ex.Message}");
            }
        }

        private void btnCreateGroup_Click(object sender, EventArgs e)
        {
            string groupName = ShowInputDialog("Введите название группы:", "Создание группы", "General");
            if (!string.IsNullOrEmpty(groupName))
            {
                JoinGroup(groupName, true);
            }
        }

        private void btnJoinGroup_Click(object sender, EventArgs e)
        {
            string groupName = ShowInputDialog("Введите название группы:", "Вход в группу", "General");
            if (!string.IsNullOrEmpty(groupName))
            {
                JoinGroup(groupName, false);
            }
        }

        private string ShowInputDialog(string text, string caption, string defaultValue = "")
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 250 };
            TextBox textBox = new TextBox() { Left = 20, Top = 45, Width = 250, Text = defaultValue };
            Button confirmation = new Button() { Text = "OK", Left = 20, Width = 100, Top = 75, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Отмена", Left = 130, Width = 100, Top = 75, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void JoinGroup(string groupName, bool createNew)
        {
            if (!isConnectedToChat) return;

            try
            {
                // Отправляем тип сообщения (6 - вход/создание группы)
                chatStream.Write(BitConverter.GetBytes(6), 0, 4);

                // Отправляем флаг создания (1 - создать, 0 - войти)
                chatStream.Write(BitConverter.GetBytes(createNew ? 1 : 0), 0, 4);

                // Отправляем название группы
                byte[] groupBytes = Encoding.UTF8.GetBytes(groupName);
                chatStream.Write(BitConverter.GetBytes(groupBytes.Length), 0, 4);
                chatStream.Write(groupBytes, 0, groupBytes.Length);

                currentGroup = groupName;
                lblCurrentGroup.Text = $"Текущая группа: {groupName}";
            }
            catch (Exception ex)
            {
                AddChatLog($"Ошибка входа в группу: {ex.Message}");
            }
        }

        private void btnLeaveGroup_Click(object sender, EventArgs e)
        {
            if (!isConnectedToChat || string.IsNullOrEmpty(currentGroup)) return;

            try
            {
                // Отправляем тип сообщения (7 - выход из группы)
                chatStream.Write(BitConverter.GetBytes(7), 0, 4);

                currentGroup = "";
                lblCurrentGroup.Text = "Текущая группа: нет";
                AddChatLog("Вы вышли из группы");
            }
            catch (Exception ex)
            {
                AddChatLog($"Ошибка выхода из группы: {ex.Message}");
            }
        }

        private void AddChatLog(string message) // Логи для чата
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AddChatLog), message);
                return;
            }

            txtChatLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtChatLog.ScrollToCaret();
        }

        private void AddLog(string message) // Логи для файлов
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AddLog), message);
                return;
            }

            txtClientLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isConnectedToChat = false;
            try
            {
                chatStream?.Close();
                chatClient?.Close();
            }
            catch { }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    UploadFile(openFileDialog.FileName);
                }
            }
        }

        private void UploadFile(string filePath)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(serverIP, serverPort);
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.ReadTimeout = 30000;
                        stream.WriteTimeout = 30000;

                        FileInfo fileInfo = new FileInfo(filePath);
                        string fileName = Path.GetFileName(filePath);

                        // Отправляем тип сообщения (1 - загрузка файла)
                        stream.Write(BitConverter.GetBytes(1), 0, 4);

                        // Отправляем имя файла
                        byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
                        stream.Write(BitConverter.GetBytes(fileNameBytes.Length), 0, 4);
                        stream.Write(fileNameBytes, 0, fileNameBytes.Length);

                        // Отправляем длину файла
                        stream.Write(BitConverter.GetBytes(fileInfo.Length), 0, 8);

                        // Отправляем данные файла
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buffer = new byte[8192];
                            int bytesRead;
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                stream.Write(buffer, 0, bytesRead);
                            }
                        }

                        AddLog($"Файл отправлен: {fileName}");

                        // Автоматически обновляем список файлов после загрузки
                        RefreshFileList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки файла: {ex.Message}");
            }
        }

        private void btnRefreshFiles_Click(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void RefreshFileList()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(serverIP, serverPort);
                    using (NetworkStream stream = client.GetStream())
                    {
                        // Устанавливаем таймауты
                        stream.ReadTimeout = 10000;
                        stream.WriteTimeout = 10000;

                        // Отправляем тип сообщения (2 - запрос списка файлов)
                        stream.Write(BitConverter.GetBytes(2), 0, 4);

                        // Получаем длину списка файлов
                        byte[] listLengthBytes = new byte[4];
                        int bytesRead = ReadFull(stream, listLengthBytes, 4);
                        if (bytesRead != 4)
                        {
                            MessageBox.Show("Ошибка получения длины списка файлов");
                            return;
                        }

                        int listLength = BitConverter.ToInt32(listLengthBytes, 0);

                        if (listLength == 0)
                        {
                            lstServerFiles.Items.Clear();
                            AddLog("На сервере нет файлов");
                            return;
                        }

                        // Получаем список файлов
                        byte[] listBytes = new byte[listLength];
                        bytesRead = ReadFull(stream, listBytes, listLength);
                        if (bytesRead != listLength)
                        {
                            MessageBox.Show("Ошибка получения списка файлов");
                            return;
                        }

                        string fileList = System.Text.Encoding.UTF8.GetString(listBytes);

                        // Обновляем список в UI
                        UpdateFileList(fileList);
                    }
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Сетевая ошибка: {ex.SocketErrorCode}. Проверьте подключение к серверу.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения списка файлов: {ex.Message}");
            }
        }

        private void UpdateFileList(string fileList)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateFileList), fileList);
                return;
            }

            lstServerFiles.Items.Clear();
            if (!string.IsNullOrEmpty(fileList))
            {
                string[] files = fileList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string file in files)
                {
                    // Извлекаем только имя файла из полного пути
                    string fileName = Path.GetFileName(file);
                    if (!string.IsNullOrEmpty(fileName))
                        lstServerFiles.Items.Add(fileName);
                }
            }

            AddLog($"Получено файлов: {lstServerFiles.Items.Count}");
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (lstServerFiles.SelectedItem != null)
            {
                string fileName = lstServerFiles.SelectedItem.ToString();
                DownloadFile(fileName);
            }
            else
            {
                MessageBox.Show("Выберите файл для скачивания");
            }
        }

        private void DownloadFile(string fileName)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(serverIP, serverPort);
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.ReadTimeout = 30000;
                        stream.WriteTimeout = 30000;

                        // Отправляем тип сообщения (3 - запрос файла)
                        stream.Write(BitConverter.GetBytes(3), 0, 4);

                        // Отправляем имя файла
                        byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
                        stream.Write(BitConverter.GetBytes(fileNameBytes.Length), 0, 4);
                        stream.Write(fileNameBytes, 0, fileNameBytes.Length);

                        // Получаем длину файла
                        byte[] fileLengthBytes = new byte[8];
                        int bytesRead = ReadFull(stream, fileLengthBytes, 8);
                        if (bytesRead != 8)
                        {
                            MessageBox.Show("Ошибка получения размера файла");
                            return;
                        }

                        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                        if (fileLength == -1)
                        {
                            MessageBox.Show("Файл не найден на сервере");
                            return;
                        }

                        // Скачиваем файл
                        string filePath = Path.Combine(txtClientPath.Text, fileName);
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            byte[] buffer = new byte[8192];
                            long totalBytesRead = 0;
                            int chunkBytesRead;

                            while (totalBytesRead < fileLength)
                            {
                                int bytesToRead = (int)Math.Min(buffer.Length, fileLength - totalBytesRead);
                                chunkBytesRead = stream.Read(buffer, 0, bytesToRead);

                                if (chunkBytesRead == 0)
                                    break;

                                fileStream.Write(buffer, 0, chunkBytesRead);
                                totalBytesRead += chunkBytesRead;
                            }
                        }

                        // Проверяем результат
                        FileInfo downloadedFile = new FileInfo(filePath);
                        if (downloadedFile.Exists && downloadedFile.Length == fileLength)
                        {
                            AddLog($"Файл скачан: {fileName} ({fileLength} байт)");
                        }
                        else
                        {
                            AddLog($"Файл скачан не полностью: {fileName} ({downloadedFile.Length} из {fileLength} байт)");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка скачивания: {ex.Message}");
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
                        AddLog("Соединение с сервером закрыто");
                        break;
                    }
                    totalBytesRead += bytesRead;
                }
                catch (IOException ex)
                {
                    // Таймаут или разрыв соединения
                    AddLog($"Ошибка чтения: {ex.Message}");
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
    }
}
