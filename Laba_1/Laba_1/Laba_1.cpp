#include <iostream>
#include <string>
#include <cstring>
#include <winsock2.h>
#include <ws2tcpip.h>
#define WIN32_LEAN_AND_MEAN
#include <windows.h>

//соединение с библиотекой
#pragma comment(lib, "ws2_32.lib")

using namespace std;

//http://localhost:8080z

int main()
{
    setlocale(LC_ALL, "ru");
    WSADATA wsaData;
    SOCKET listenSocket, clientSocket;
    sockaddr_in serverAddr{};

    //инициализируем библиотеку
    WSAStartup(MAKEWORD(2, 2), &wsaData);

    //Создаём сокет для прослушки
    listenSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

    //настраиваем прослушку по порту 8080
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(8080);

    //привязываем сокет к указанному адресу и порту
    bind(listenSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));
    listen(listenSocket, 1);

    std::cout << "Ожидание соединения...\n";

    //Принимаем соединение
    clientSocket = accept(listenSocket, nullptr, nullptr);

    //получаем запрос от клиента
    char buffer[1024];

    //получаем данные из сокета
    int recvSize = recv(clientSocket, buffer, sizeof(buffer) - 1, 0);
    if (recvSize > 0) {
        buffer[recvSize] = '\0';
        std::cout << "Request:\n" << buffer << "\n";

        const char* response =
            "HTTP/1.1 200 OK\r\n"               //статус
            "Content-Type: text/plain\r\n"      //тип
            "Content-Length: 14\r\n"            //длина
            "\r\n"                              
            "Привет сервер\n";                  //ответ
        
        //отправляем ответ клиенту
        send(clientSocket, response, (int)strlen(response), 0);
    }

    closesocket(clientSocket);
    closesocket(listenSocket);
    WSACleanup();
    return 0;
}

