#pragma comment (lib, "ws2_32.lib")

// Includes:
#include "WSAInitializer.h"
#include "Server.h"
#include "RSA.h"
#include <iostream>
#include <fstream>

// main function:
int main()
{
	try
	{
		// Server Inits:
		WSAInitializer wsa_init;
		SqliteDataBase* database = SqliteDataBase::getInstance();
		Server* server = Server::getInstance(database);

		// Running the server:
		server->run();
	}

	// Catching exceptions:
	catch (const std::exception& e) {
		std::cout << "Exception was thrown in function: " << e.what() << std::endl;
	}

	catch (...) {
		std::cout << "Unknown exception in main!" << std::endl;
	}

	return 0;
}
