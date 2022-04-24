#include "Client.h"

// C'tor & D'tor:

Client::Client(IRequestHandler* handler, const string& username, SOCKET listener)
{
	m_handler = handler;
	m_username = username;
	m_listener = listener;
}

Client::~Client()
{
	if (m_handler != nullptr) {
		delete m_handler;
	}
	m_username = "";
	::closesocket(m_listener);
}


// Getters:

IRequestHandler* Client::getHandler() const
{
	return m_handler;
}

string Client::getUsername() const
{
	return m_username;
}

SOCKET Client::getListener() const
{
	return m_listener;
}


// Setters:

void Client::setHandler(IRequestHandler* handler)
{
	m_handler = handler;
}

void Client::setUsername(const string& username)
{
	m_username = username;
}

void Client::setListener(SOCKET listener)
{
	m_listener = listener;
}
