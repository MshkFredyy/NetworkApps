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
        std::cerr << "WSAStartup ошибка: " << iResult << "\n";
        return 1;
    }

    SOCKET ListenSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (ListenSocket == INVALID_SOCKET)
    {
        std::cerr << "Ошибка создания сокета\n";
        WSACleanup();
        return 1;
    }

    sockaddr_in service;
    service.sin_family = AF_INET;
    service.sin_addr.s_addr = INADDR_ANY;
    service.sin_port = htons(27015);

    if (bind(ListenSocket, (SOCKADDR*)&service, sizeof(service)) == SOCKET_ERROR)
    {
        std::cerr << "Ошибка привязки\n";
        closesocket(ListenSocket);
        WSACleanup();
        return 1;
    }

    if (listen(ListenSocket, SOMAXCONN) == SOCKET_ERROR)
    {
        std::cerr << "Ошибка прослушивания\n";
        closesocket(ListenSocket);
        WSACleanup();
        return 1;
    }

    std::cout << "Ожидание соединения с клиентом...\n";

    SOCKET ClientSocket = accept(ListenSocket, NULL, NULL);
    if (ClientSocket == INVALID_SOCKET)
    {
        std::cerr << "Ошибка присоединения\n";
        closesocket(ListenSocket);
        WSACleanup();
        return 1;
    }
    closesocket(ListenSocket);

    char recvbuf[512];
    int recvbuflen = 512;

    iResult = recv(ClientSocket, recvbuf, recvbuflen, 0);
    if (iResult > 0)
    {
        recvbuf[iResult] = '\0';
        std::cout << "Получено: " << recvbuf << "\n";

        const char* sendbuf = "Сообщение получено: ";
        send(ClientSocket, sendbuf, (int)strlen(sendbuf), 0);
    }

    closesocket(ClientSocket);
    WSACleanup();
    return 0;
}

