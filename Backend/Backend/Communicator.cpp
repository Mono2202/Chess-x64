#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include "Communicator.h"

Communicator* Communicator::m_communicatorInstance = nullptr;
map<SOCKET, Client*> Communicator::m_clients;

// C'tors:

Communicator::Communicator(RequestHandlerFactory& handlerFactory) : m_handlerFactory(handlerFactory)
{
	// Inits:
	ip = "";
	port = 0;
	listenerPort = 0;

	// Creating the socket:
	m_serverSocket = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	
	// Condition: error while creating the socket
	if (m_serverSocket == INVALID_SOCKET)
		throw std::exception("Could not create server socket\n");

	// Creating the socket:
	m_serverListener = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	// Condition: error while creating the socket
	if (m_serverListener == INVALID_SOCKET)
		throw std::exception("Could not create server socket\n");
}

Communicator* Communicator::getInstance(RequestHandlerFactory& handlerFactory)
{
	if (m_communicatorInstance == nullptr) {
		m_communicatorInstance = new Communicator(handlerFactory);
	}

	return m_communicatorInstance;
}

// D'tors:

Communicator::~Communicator()
{
	// Trying to close the socket:
	try {
		::closesocket(m_serverSocket);
	}

	// Catching exceptions:
	catch (...) {   }

	// Trying to close the socket:
	try {
		::closesocket(m_serverListener);
	}

	// Catching exceptions:
	catch (...) {}

	for (auto& client : m_clients)
	{
		try {
			delete client.second;
		} catch (...) {	}
	}

	delete m_communicatorInstance;

	// Resetting fields:
	ip = "";
	port = 0;
	listenerPort = 0;
}


// Methods:

/*
Starting to handle client requests
Input : < None >
Output: < None >
*/
void Communicator::startHandleRequests()
{
	// Binding and listening to the client socket:
	bindAndListen();

	while (true)
	{
		// Creating a new client socket:
		SOCKET newClientSock = accept(m_serverSocket, NULL, NULL);
		SOCKET newClientListener = accept(m_serverListener, NULL, NULL);

		// Condition: new socket creation failed
		if (newClientSock == INVALID_SOCKET) {
			throw std::exception("Can't connect to new client socket");
		}

		// Condition: new socket creation failed
		if (newClientListener == INVALID_SOCKET) {
			throw std::exception("Can't connect to new client socket");
		}

		// Inserting the new client to the client map:
		m_clients.insert(std::pair<SOCKET, Client*>(newClientSock, new Client(m_handlerFactory.createLoginRequestHandler(), "", newClientListener)));
		
		// Creating the client thread:
		thread newClientThread(&Communicator::handleNewClient, this, newClientSock);

		// Detaching the client thread:
		newClientThread.detach();
	}
}


// Private Methods:

/*
Reading the config file
Input : < None >
Output: < None >
*/
void Communicator::readConfig()
{
	// Inits:
	vector<string> lines;
	string line;
	std::ifstream configFile;
	configFile.open(CONFIG_FILE);

	// Reading the config file line by line:
	while (std::getline(configFile, line)) {
		lines.push_back(line);
	}

	// Setting the fields:
	ip = lines[IP_INDEX].substr(4);
	port = stoi(lines[PORT_INDEX].substr(6));
	listenerPort = stoi(lines[LISTENER_PORT_INDEX].substr(14));
	
	// Printing the server settings:
	std::cout << "IP: " << ip << std::endl;
	std::cout << "PORT: " << port << std::endl;
	std::cout << "LISTENER PORT: " << listenerPort << std::endl;
}

/*
Binding and listening to the client socket
Input : < None >
Output: < None >
*/
void Communicator::bindAndListen()
{
	// Inits:
	struct sockaddr_in sa = { 0 };
	struct sockaddr_in saListener = { 0 };
	
	// Getting the socket settings:
	readConfig();

	// Socket address inits:
	sa.sin_port = htons(port);
	sa.sin_addr.s_addr = inet_addr(ip.c_str());
	sa.sin_family = AF_INET;

	// Socket address inits:
	saListener.sin_port = htons(listenerPort);
	saListener.sin_addr.s_addr = inet_addr(ip.c_str());
	saListener.sin_family = AF_INET;

	// Binding the socket:
	if (::bind(m_serverSocket, (struct sockaddr*)&sa, sizeof(sa)) == SOCKET_ERROR) {
		throw std::exception("Failed in server socket binding\n");
	}

	// Listening to the socket:
	if (::listen(m_serverSocket, SOMAXCONN) == SOCKET_ERROR) {
		throw std::exception("Failed in initiating server socket listening\n");
	}

	// Binding the socket:
	if (::bind(m_serverListener, (struct sockaddr*)&saListener, sizeof(saListener)) == SOCKET_ERROR) {
		throw std::exception("Failed in server socket binding\n");
	}

	// Listening to the socket:
	if (::listen(m_serverListener, SOMAXCONN) == SOCKET_ERROR) {
		throw std::exception("Failed in initiating server socket listening\n");
	}
}

/*
Handling the new client
Input : sock - the client socket
Output: < None >
*/
void Communicator::handleNewClient(SOCKET sock)
{
	// Setting random seed:
	srand(time(0));

	try
	{
		while (true)
		{
			// Inits:
			Buffer buffer(SIZE);
			RequestInfo rqInfo;
			RequestResult rqResult;
			int bufBytes = 0;

			// Receiving message into buffer:
			bufBytes = recv(sock, (char*)&buffer[0], SIZE, 0);

			// Condition: error while receiving message from client
			if (!bufBytes) {
				throw std::exception("Could not receive message from client");
			}

			// Creating the RequestInfo struct:
			Buffer decryptedBuffer = AES::decrypt(Buffer(buffer.begin(), buffer.end()));
			rqInfo.buffer = decryptedBuffer;
			rqInfo.id = decryptedBuffer[0];
			rqInfo.receivalTime = time(NULL);

			// Condition: relevant request
			if (m_clients.at(sock)->getHandler()->isRequestRelevant(rqInfo)) {
				// Building the RequestResult struct:
				try {
					std::cout << "ID: " << rqInfo.id << std::endl;
					rqResult = m_clients.at(sock)->getHandler()->handleRequest(rqInfo);

					// Inserting the client with the name:
					if (rqInfo.id == LOGIN_REQUEST) {
						m_clients[sock]->setUsername(JsonRequestPacketDeserializer::deserializeLoginRequest(rqInfo.buffer).username);
					}
				}

				// Catching Request Error:
				catch (const std::exception& e) {
					// Inits:
					ErrorResponse response = { 0, e.what() };
					rqResult.buffer = JsonResponsePacketSerializer::serializeResponse(response);
					rqResult.newHandler = nullptr;
				}
			}

			// Condition: irrelevant request
			else {
				// Creating the error response:
				ErrorResponse response = { 0, "ERROR: Irrelevant request" };
				rqResult.buffer = JsonResponsePacketSerializer::serializeResponse(response);
				rqResult.newHandler = nullptr;
				throw std::exception("Irrelevant request");
			}

			// Condition: updating the handler
			if (rqResult.newHandler != nullptr) {
				delete m_clients.find(sock)->second->getHandler();
				m_clients.find(sock)->second->setHandler(rqResult.newHandler);
			}

			// Sending the a message to the client:
			if (!send(sock, (char*)&AES::encrypt(rqResult.buffer)[0], AES::encrypt(rqResult.buffer).size(), 0)) {
				throw std::exception("Could not send message back to client");
			}
		}
	}

	// Catching exceptions:
	catch (const std::exception& e) {
		std::cerr << "ERROR: " << e.what() << std::endl;
		
		// Inits:
		RequestResult requestResult;

		// Condition: Leave Room
		if (dynamic_cast<RoomRequestHandler*>(m_clients.find(sock)->second->getHandler())) {
			// Inits:
			RequestInfo rqInfo;
			int i = 0;

			// Creating the RequestInfo struct:
			rqInfo.buffer = Buffer();
			rqInfo.buffer.push_back(LEAVE_ROOM_REQUEST);
			rqInfo.id = LEAVE_ROOM_REQUEST;
			rqInfo.receivalTime = time(NULL);

			// Leaving the room:
			requestResult = m_clients.at(sock)->getHandler()->handleRequest(rqInfo);

			// Changing handlers:
			delete m_clients.find(sock)->second->getHandler();
			m_clients.find(sock)->second->setHandler(requestResult.newHandler);
		}

		// Condition: Logging-out
		if (dynamic_cast<MenuRequestHandler*>(m_clients.find(sock)->second->getHandler())) {
			// Inits:
			RequestInfo rqInfo;
			int i = 0;

			// Creating the RequestInfo struct:
			rqInfo.buffer = Buffer();
			rqInfo.buffer.push_back(LOGOUT_REQUEST);
			rqInfo.id = LOGOUT_REQUEST;
			rqInfo.receivalTime = time(NULL);

			// Logging-out:
			m_clients.at(sock)->getHandler()->handleRequest(rqInfo);
			delete m_clients.find(sock)->second->getHandler();
			m_clients.find(sock)->second = nullptr;
		}

		// Condition: no handler
		if (m_clients.find(sock)->second != nullptr)
			delete m_clients.find(sock)->second;
	}

	// Deleting the client:
	m_clients.erase(sock);
	::closesocket(sock);
}
