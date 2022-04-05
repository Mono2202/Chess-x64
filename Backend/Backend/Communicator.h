#pragma once

// Includes:
#include <iostream>
#include <map>
#include <thread>
#include "IRequestHandler.h"
#include "LoginRequestHandler.h"
#include "JsonResponsePacketSerializer.h"
#include "JsonRequestPacketDeserializer.h"
#include <WinSock2.h>

// Defines:
#define PORT 54321
#define SIZE 4096

// Using:
using std::string;
using std::map;
using std::thread;

// Communicator Class:
class Communicator
{
public:
	// Static C'tor:
	static Communicator* getInstance(RequestHandlerFactory& handlerFactory);

	// D'tor:
	~Communicator();

	// Methods:
	void startHandleRequests();

private:
	// Private C'tor:
	Communicator(RequestHandlerFactory& handlerFactory);

	// Fields:
	static Communicator* m_communicatorInstance;
	SOCKET m_serverSocket;
	map<SOCKET, IRequestHandler*> m_clients;
	RequestHandlerFactory& m_handlerFactory;

	// Private Methods:
	void bindAndListen();
	void handleNewClient(SOCKET sock);
};
