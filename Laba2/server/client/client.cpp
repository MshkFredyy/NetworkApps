#define WIN32_LEAN_AND_MEAN
#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>

#pragma comment(lib, "Ws2_32.lib")

int main()
{
    setlocale(LC_ALL, "ru");
    WSADATA wsaData;
    int iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (iResult != 0)
    {
        std::cerr << "Ошибка WSAStartup: " << iResult << "\n";
        return 1;
    }

    SOCKET ConnectSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (ConnectSocket == INVALID_SOCKET)
    {
        std::cerr << "Ошибка создания сокета\n";
        WSACleanup();
        return 1;
    }

    sockaddr_in clientService;
    clientService.sin_family = AF_INET;
    clientService.sin_port = htons(27015);
    inet_pton(AF_INET, "127.0.0.1", &clientService.sin_addr);

    iResult = connect(ConnectSocket, (SOCKADDR*)&clientService, sizeof(clientService));
    if (iResult == SOCKET_ERROR) {
        std::cerr << "Ошибка соединения\n";
        closesocket(ConnectSocket);
        WSACleanup();
        return 1;
    }

    const char* sendbuf = "Привет от клиента сервера!";
    send(ConnectSocket, sendbuf, (int)strlen(sendbuf), 0);

    char recvbuf[512];
    iResult = recv(ConnectSocket, recvbuf, 512, 0);
    if (iResult > 0) {
        recvbuf[iResult] = '\0';
        std::cout << "Ответ сервера: " << recvbuf << "\n";
    }

    closesocket(ConnectSocket);
    WSACleanup();
    return 0;
}

