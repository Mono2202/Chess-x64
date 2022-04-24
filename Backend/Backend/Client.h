#pragma once

// Includes:
#include "IRequestHandler.h"
#include <WinSock2.h>

class Client
{
public:
	// C'tor & D'tor:
	Client(IRequestHandler* handler, const string& username, SOCKET listener);
	~Client();

	// Getters:
	IRequestHandler* getHandler() const;
	string getUsername() const;
	SOCKET getListener() const;

	// Setters:
	void setHandler(IRequestHandler* handler);
	void setUsername(const string& username);
	void setListener(SOCKET listener);

private:
	// Fields:
	IRequestHandler* m_handler;
	string m_username;
	SOCKET m_listener;
};

