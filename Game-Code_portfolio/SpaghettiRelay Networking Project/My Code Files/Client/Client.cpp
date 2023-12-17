#include "Client.h"
int Client::init(uint16_t port, char* address)
{
	// TO DO: Create a socket
	ComSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if(ComSocket == INVALID_SOCKET) {
		return SETUP_ERROR;
	}



	// TO DO: Convert the address and port into a
	//			sock_addr structure
	server.sin_family = AF_INET;
	server.sin_addr.S_un.S_addr = inet_addr(address);
	server.sin_port = htons(port);
	if (server.sin_addr.S_un.S_addr == INADDR_NONE) {
		return ADDRESS_ERROR;
	}



	// TO DO: Connect the socket to the sock_addr
	int result = connect(ComSocket, (SOCKADDR*)&server, sizeof(server));
	if (result == SOCKET_ERROR) {
		if (WSAGetLastError() == WSAESHUTDOWN) {
			return SHUTDOWN;
		}
		else
			return CONNECT_ERROR;
	}

	
	// IMPORTANT TO DO: Error Check each function call from winsock
	return SUCCESS;
}
int Client::readMessage(char* buffer, int32_t size)
{
	int total = 0;
	int ret = 0;
	ret = recv(ComSocket, buffer + total, size - total, 0);
	if (ret > size)
		return PARAMETER_ERROR;
	if (ret < 1) {
		if (WSAGetLastError() == WSAESHUTDOWN)
			return SHUTDOWN;
		else
			return DISCONNECT;
	}
	do
	{
		ret = recv(ComSocket, buffer + total, size - total, 0);
		if (ret > size)
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
int Client::sendMessage(char* data, int32_t length)
{
	if (length < 0 || length > 255)
		return PARAMETER_ERROR;
	int result;
	int bytesSent = 0;
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
void Client::stop()
{
	shutdown(ComSocket, SD_BOTH);
	closesocket(ComSocket);
}

//int Server::sendMessage(char* data, int32_t length)
//{
//	if (length < 0 || length > 255) {
//		return PARAMETER_ERROR;
//	}
//	int result;
//	int bytesSent = 0;
//	while (bytesSent < length) {
//		result = send(ComSocket, (const char*)data + bytesSent, length - bytesSent, 0);
//		if (result <= 0) {
//			if (WSAGetLastError() == WSAESHUTDOWN)
//				return SHUTDOWN;
//			else
//				return DISCONNECT;
//		}
//		bytesSent += result;
//	}
//	return SUCCESS;
//}