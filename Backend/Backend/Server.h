#pragma once

// Includes:
#include "Communicator.h"
#include "SqliteDataBase.h"
#include <string>

// Server Class:
class Server
{
public:
	// Static C'tor:
	static Server* getInstance(IDatabase* database);

	// D'tor:
	~Server();

	// Methods:
	void run();

private:
	// Private C'tor:
	Server(IDatabase* database);

	// Fields:
	static Server* m_serverInstance;
	Communicator* m_communicator;
	IDatabase* m_database;
	RequestHandlerFactory m_handlerFactory;
};
