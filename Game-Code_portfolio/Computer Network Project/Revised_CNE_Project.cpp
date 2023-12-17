#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <WinSock2.h>
#pragma comment(lib, "Ws2_32.lib")
#include "stdint.h"
#include <iostream>
#include<string>
#include <vector>



class Machine {
public:
	timeval timer;
	SOCKET ComSocket;
	SOCKET Udp;
	sockaddr_in server;
	sockaddr_in broadcast;
	std::string ip;
	int port;
	bool isActive;
	bool test;
	char buffer[1024];
	char bc_buffer[1024];

	Machine(bool x = true) {
		timer.tv_sec = 1;
		if (x){
			test = true;
		}
		else {
			test = false;
		}
		isActive = true;
	}

	virtual int SendData(SOCKET _ComSocket, char* buffer, int length) {
		int result = 0;
		int bytesSent = 0;
		result = send(_ComSocket, (char*)&length, 1, 0);
		if (result <= 0) {
			if (WSAGetLastError() == WSAESHUTDOWN) {
				printf("ERROR: Server has shutdown\n");
				return -1;
			}
			else {
				printf("ERROR: there was a problem sending your message\n");
				return -1;
			}
		}
		result = send(_ComSocket, buffer, length, 0);
		if (result <= 0) {
			if (WSAGetLastError() == WSAESHUTDOWN) {
				printf("ERROR: Server has shutdown\n");
				return -1;
			}
			else {
				printf("ERROR: there was a problem sending your message\n");
				return -1;
			}
		}
		return result;
	}
	virtual int ReadData(SOCKET _ComSocket, char* buffer, int size) {
		int ret = 0;
		ret = recv(_ComSocket, buffer, size, 0);
		if (ret < 1) {
			if (WSAGetLastError() == WSAESHUTDOWN) {
				printf("ERROR: A client has disconnected ungracefully\n");
				return -1;
			}
		}
		return ret;
	}
};
class Client : public Machine {
public:
	char name[1024];

	Client(bool x = true) : Machine(x) {
		
	}

	void Code() {
		Init();
		Run();
	}
	void Init() {
		ComSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
		if (ComSocket == INVALID_SOCKET) {
			printf("ERROR: there was a problem creating the ComSocket for this Machine\n");
			isActive = false;
			return;
		}

		if (test) {
			port = 31337;
			ip = "127.0.0.1";
			server.sin_family = AF_INET;
			server.sin_addr.S_un.S_addr = inet_addr(ip.c_str());
			server.sin_port = htons(port);
		}
		else {
			system("cls");
			printf("What is the IP address of the server you want to connect to?\n");
			std::cin >> ip;
			system("cls");
			printf("What is the port of the destination Machine's Apllication\n");
			std::cin >> port;
			server.sin_family = AF_INET;
			server.sin_addr.S_un.S_addr = inet_addr(ip.c_str());
			server.sin_port = htons(port);
		}

		int result = connect(ComSocket, (SOCKADDR*)&server, sizeof(server));
		if (result == SOCKET_ERROR) {
			if (WSAGetLastError() == WSAESHUTDOWN) {
				printf("ERROR: device shutdown has occurred\n");
				isActive = false;
				return;
			}
			else {
				printf("ERROR: failed to connect to the desired server\n");
				int x = GetLastError();
				isActive = false;
				return;
			}
		}

	}
	void Run() {
		std::cin.ignore(INT_MAX, '\n');
		while (isActive) {
			//UDP();
			printf("Send: ");
			std::cin.getline(buffer, 1024);
			int result = SendData(ComSocket, buffer, sizeof(buffer));
			if (result != -1) {
				printf("[message sent]\n");
				if (buffer[0] == '$') {
					ProcessCommand(buffer);
				}
				memset(buffer, 0, sizeof(buffer));
			}
		}
	}
	void ProcessCommand(char* command){
		if (strcmp(command, "$exit") == 0) {
			// Logic to handle $getlog command
			Stop();
		}
		else{
			printf("Unknown command: %s\n", command);
		}
	}
	void UDP() {
		int x = 1;
		char* optval = reinterpret_cast<char*>(&x);
		Udp = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
		setsockopt(Udp, SOL_SOCKET, SO_BROADCAST, optval, x);
		broadcast.sin_family = AF_INET;
		broadcast.sin_addr.S_un.S_addr = INADDR_ANY;
		broadcast.sin_port = htons(port + 1);
		int result = recvfrom(Udp, buffer, 1024, 0, (SOCKADDR*)&broadcast, NULL);
		if (result < 1) {
			printf("Broadcast error\n");
		}
		else
			printf("You've got mail\n");

	}
	void Stop() {
		isActive = false;
		shutdown(ComSocket, SD_BOTH);
		closesocket(ComSocket);
		printf("\n\n\nGracefully Disconnected from the server\n");
	}
};
class Server : public Machine {
public:
	SOCKET listenSocket;
	fd_set masterSet;
	fd_set readySet;
	int client_count;
	int client_max;

	Server(int _client_max, bool x = true) : Machine(x) {
		client_max = _client_max;
	}

	void Code() {
		Init();
		Run();
	}
	void Init() {
		listenSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
		if (listenSocket == INVALID_SOCKET) {
			printf("There was a problem creating the listening socket\n");
			int x = WSAGetLastError();
			isActive = false;
			return;
		}

		if (test) {
			port = 31337;
			server.sin_family = AF_INET;
			server.sin_addr.S_un.S_addr = INADDR_ANY;
			server.sin_port = htons(port);
		}
		else {
			printf("Type the IP address of your server:\n");
			std::cin >> ip;
			printf("Type the port for your server application:\n");
			std::cin >> port;
			std::cin.ignore(INT_MAX, '\n');
			std::cin.clear();
			server.sin_family = AF_INET;
			server.sin_addr.S_un.S_addr = inet_addr(ip.c_str());
			server.sin_port = htons(port);
		}

		int result = bind(listenSocket, (SOCKADDR*)&server, sizeof(server));
		if (result == SOCKET_ERROR) {
			printf("ERROR: the listenSocket failed to bind\n");
			isActive = false;
			return;
		}


		result = listen(listenSocket, 1);
		if (result == SOCKET_ERROR) {
			printf("ERROR: there was a problem with listen()\n");
			isActive = false;
			return;
		}
		else
			printf("Listening...\n");

		FD_ZERO(&masterSet);
		FD_ZERO(&readySet);
		FD_SET(listenSocket, &masterSet);
	}
	void Run() {
		while (isActive) {
			readySet = masterSet;
			int ready = select(10, &readySet, NULL, NULL, NULL);
			//UDP();
			if (FD_ISSET(listenSocket, &readySet)) {
				ComSocket = accept(listenSocket, NULL, NULL);
				if (ComSocket == INVALID_SOCKET) {
					printf("ERROR: there was a problem accepting a client\n");
					continue;
				}
				else{
					printf("A client has connected with the server\n");
					FD_SET(ComSocket, &masterSet);
				}
			}
			for (int i = 0; i < readySet.fd_count; i++)	{
				SOCKET currentSocket = readySet.fd_array[i];
				if (readySet.fd_array[i] == listenSocket)
					continue;
				else {
					int result = recv(readySet.fd_array[i], buffer, 1, 0);
					if (result < 1 && result != 0) {
						printf("ERROR: there was a problem receiving the message length from the client\n");
					}
					result = ReadData(readySet.fd_array[i], buffer, sizeof(buffer));
					if (result > 0) {
						printf("Received: % s\n", buffer);
					}
					else if (result == 0) {
						printf("A client has successfully disconnected from the server\n");
						FD_CLR(readySet.fd_array[i], &masterSet);
					}
					else
						fprintf(stderr, "ERROR: there was a problem receiving data from the client\n");
					
				}
			}
		}
	}
	void UDP() {
		int x = 1;
		char* optval = reinterpret_cast<char*>(&x);
		Udp = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
		setsockopt(Udp, SOL_SOCKET, SO_BROADCAST, optval, x);
		broadcast.sin_family = AF_INET;
		broadcast.sin_addr.S_un.S_addr = INADDR_BROADCAST;
		broadcast.sin_port = htons(port + 1);
		strcpy_s(bc_buffer, "IP: 127.0.0.1\nPort:31337");
		int result = sendto(Udp, bc_buffer, 1024, 0, (SOCKADDR*)&broadcast, timer.tv_sec);
		if (result < 1) {
			int y = WSAGetLastError();
			printf("Broadcast failed\n");
		}
		else {
			printf("Broadcast sent\n");
		}
	}
	void Stop() {

	}
};






int main() {
	WSADATA wsadata;
	WSAStartup(WINSOCK_VERSION, &wsadata);
	int x = 0;
	Server _Server(10);
	Client _Client;
	while (true){
		system("cls");
		printf("Choose a Machine type:\n");
		printf("1> Server\n");
		printf("2> Client\n");
		std::cin >> x;
		if (x == 1) {
			printf("this is a server\n");
			_Server.Code();
			break;
		}
		else if (x == 2) {
			printf("this is a client\n");
			_Client.Code();
			break;
		}
	}
	return WSACleanup();
}

