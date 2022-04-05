#pragma once

// Includes:
#include "IRequestHandler.h"
#include "LoginManager.h"
#include "RequestHandlerFactory.h"

// Class Declaration:
class RequestHandlerFactory;

// LoginRequestHandler Class:
class LoginRequestHandler : public IRequestHandler
{
public:
	// C'tors & D'tors:
	LoginRequestHandler(LoginManager& loginManager, RequestHandlerFactory& handlerFactory);
	~LoginRequestHandler() = default;

	// Virtual Methods:
	virtual bool isRequestRelevant(RequestInfo request);
	virtual RequestResult handleRequest(RequestInfo request);

private:
	// Fields:
	LoginManager& m_loginManager;
	RequestHandlerFactory& m_handlerFactory;

	// Private Methods:
	RequestResult login(RequestInfo request);
	RequestResult signup(RequestInfo request);
};
