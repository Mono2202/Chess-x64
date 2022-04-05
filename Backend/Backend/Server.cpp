#include "Server.h"
Server* Server::m_serverInstance = nullptr;

// C'tors:

Server::Server(IDatabase* database) :
	m_database(database), m_handlerFactory(*LoginManager::getInstance(database), database, *RoomManager::getInstance(), *StatisticsManager::getInstance(database))
{
	m_communicator = Communicator::getInstance(m_handlerFactory);
}

Server* Server::getInstance(IDatabase* database)
{
	if (m_serverInstance == nullptr) {
		m_serverInstance = new Server(database);
	}

	return m_serverInstance;
}


// D'tors:

Server::~Server()
{
	if (m_database != nullptr)
		m_database->close();
	m_database = nullptr;

	delete m_serverInstance;
}


// Methods:

/*
Running the server
Input : < None >
Output: < None >
*/
void Server::run()
{
	// Inits:
	std::string consoleIn = "";

	// Creating the server thread:
	thread t_connector(&Communicator::startHandleRequests, m_communicator);

	// Detaching the server thread:
	t_connector.detach();

	// Keeping the server running:
	while (consoleIn != "EXIT") {
		std::getline(std::cin, consoleIn);
	}
}
