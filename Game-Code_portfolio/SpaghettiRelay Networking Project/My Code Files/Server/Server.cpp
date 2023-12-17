#include "Server.h"

int Server::init(uint16_t port)
{
	/*Create a socket (socket().)
	Bind the socket to the specified port (bind().)
	Set up a listening queue for connections (listen().)
	Wait for and accept a single connection from a client (accept().)*/
	listenSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (listenSocket == INVALID_SOCKET) {
		return SETUP_ERROR;
	}
	server.sin_family = AF_INET;
	server.sin_addr.S_un.S_addr = INADDR_ANY;
	server.sin_port = htons(port);

	int result = bind(listenSocket, (SOCKADDR*)&server, sizeof(server));
	if (result == SOCKET_ERROR) {
		return BIND_ERROR;
	}
	result = listen(listenSocket, 1);
	if (result == SOCKET_ERROR) {
		return SETUP_ERROR;
	}
	ComSocket = accept(listenSocket, NULL, NULL);
	if (ComSocket == INVALID_SOCKET) {
		if (WSAGetLastError() == WSAESHUTDOWN) {
			return SHUTDOWN;
		}
		return CONNECT_ERROR;
	}
	return SUCCESS;
}
int Server::readMessage(char* buffer, int32_t size){
	int total = 0;
	int ret = 0;
	ret = recv(ComSocket, buffer + total, size - total, 0);
	if (ret < 1) {
		if (WSAGetLastError() == WSAESHUTDOWN)
			return SHUTDOWN;
		else
			return DISCONNECT;
	}
	do
	{
		int ret1 = ret;
		ret = recv(ComSocket, buffer + total, size - total, 0);
		if (ret1 > ret)
			return PARAMETER_ERROR;
		if (ret < 1) {
			if (WSAGetLastError() == WSAESHUTDOWN)
				return SHUTDOWN;
			else
				return DISCONNECT;
		}
		else
			total += ret;

	} while (total < ret);
	return SUCCESS;
}
int Server::sendMessage(char* data, int32_t length)
{
	if (length < 0 || length > 255) {
		return PARAMETER_ERROR;
	}
	int result;
	result = send(ComSocket, (char*)&length, 1, 0);
	if (result <= 0) {
		if (WSAGetLastError() == WSAESHUTDOWN)
			return SHUTDOWN;
		else
			return DISCONNECT;
	}

	result = send(ComSocket, data, length, 0);
	if (result <= 0) {
		if (WSAGetLastError() == WSAESHUTDOWN)
			return SHUTDOWN;
		else
			return DISCONNECT;
	}

	return SUCCESS;
}
void Server::stop()
{
	shutdown(ComSocket, SD_BOTH);
	closesocket(ComSocket);
	shutdown(listenSocket, SD_BOTH);
	closesocket(listenSocket);
}