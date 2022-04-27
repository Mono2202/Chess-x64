#pragma once

// Includes:
#include <iostream>
#include <map>
#include <thread>
#include <fstream>
#include "IRequestHandler.h"
#include "LoginRequestHandler.h"
#include "JsonResponsePacketSerializer.h"
#include "JsonRequestPacketDeserializer.h"
#include <WinSock2.h>
#include "AES.h"
#include "Client.h"

// Defines:
#define CONFIG_FILE "config.txt"
#define IP_INDEX 0
#define PORT_INDEX 1
#define LISTENER_PORT_INDEX 2
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

	// Static Fields:
	static map<SOCKET, Client*> m_clients;

private:
	// Private C'tor:
	Communicator(RequestHandlerFactory& handlerFactory);

	// Fields:
	static Communicator* m_communicatorInstance;
	SOCKET m_serverSocket;
	SOCKET m_serverListener;
	RequestHandlerFactory& m_handlerFactory;
	std::string ip;
	int port;
	int listenerPort;

	// Private Methods:
	void readConfig();
	void bindAndListen();
	void handleNewClient(SOCKET sock);
};
