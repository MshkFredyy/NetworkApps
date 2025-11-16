#define WIN32_LEAN_AND_MEAN
#include <iostream>
#include <windows.h>
#include <winsock2.h>
#include <fstream>
#include <string>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

int main()
{
    setlocale(LC_ALL, "ru");

    WSADATA wsaData;
    SOCKET listenSocket, clientSocket;
    sockaddr_in serverAddr{};

    WSAStartup(MAKEWORD(2, 2), &wsaData);

    listenSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(8080);

    bind(listenSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));
    listen(listenSocket, 1);

    std::cout << "Ожидание соединения...\n";

    clientSocket = accept(listenSocket, nullptr, nullptr);

    char buffer[1024];
    int recvSize = recv(clientSocket, buffer, sizeof(buffer) - 1, 0);
    if (recvSize > 0) {
        buffer[recvSize] = '\0';
        std::cout << "Запрос:\n" << buffer << "\n";

        // открываем файл для чтения
        std::ifstream file("index.html", std::ios::binary);
        if (file) {
            // читаем файл в строку
            std::string content((std::istreambuf_iterator<char>(file)), std::istreambuf_iterator<char>());

            // формируем заголовки ответа
            std::string response =
                "HTTP/1.1 200 OK\r\n"
                "Content-Type: text/html\r\n"
                "Content-Length: " + std::to_string(content.size()) + "\r\n"
                "Connection: close\r\n"
                "\r\n" +
                content;

            // Отправляем ответ клиенту
            send(clientSocket, response.c_str(), (int)response.size(), 0);
        }
        else {
            const char* notFound =
                "HTTP/1.1 404 Not Found\r\n"
                "Content-Length: 13\r\n"
                "Connection: close\r\n"
                "\r\n"
                "404 Not Found";
            send(clientSocket, notFound, (int)strlen(notFound), 0);
        }
    }

    closesocket(clientSocket);
    closesocket(listenSocket);
    WSACleanup();

    return 0;
}

