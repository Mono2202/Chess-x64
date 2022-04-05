#include "LoginRequestHandler.h"

// C'tors:

LoginRequestHandler::LoginRequestHandler(LoginManager& loginManager, RequestHandlerFactory& handlerFactory) :
	m_loginManager(loginManager), m_handlerFactory(handlerFactory) {   }


// Virtual Methods:

/*
Checking whether the request is relevant or not
Input : request - the request info
Output: true	- login or signup request
		false	- otherwise
*/
bool LoginRequestHandler::isRequestRelevant(RequestInfo request)
{
	// Condition: login or signup request initiated
	if (request.buffer[0] == LOGIN_REQUEST || request.buffer[0] == SIGNUP_REQUEST) {
		return true;
	}

	// Condition: otherwise
	return false;
}

/*
Handling the request
Input : request - the request info
Output: result  - the request result
*/
RequestResult LoginRequestHandler::handleRequest(RequestInfo request)
{

	// Condition: login request
	if (request.buffer[0] == LOGIN_REQUEST) {
		return login(request);
	}

	// Condition: signup request
	else {
		return signup(request);
	}
}


// Private Methods:

/*
Logging-in to an account
Input : request - the login request
Output: result  - the request result
*/
RequestResult LoginRequestHandler::login(RequestInfo request)
{
	// Inits:
	RequestResult result;
	LoginRequest loginRequest = JsonRequestPacketDeserializer::deserializeLoginRequest(request.buffer);

	// Trying to login:
	m_loginManager.login(loginRequest.username, loginRequest.password);
	LoginResponse response = { SUCCESS_STATUS };
	result.buffer = JsonResponsePacketSerializer::serializeResponse(response);

	// Returning the result:
	LoggedUser user(loginRequest.username);
	result.newHandler = m_handlerFactory.createMenuRequestHandler(user);
	return result;
}

/*
Signing-up a new account
Input : request - the signup request
Output: result  - the request result
*/
RequestResult LoginRequestHandler::signup(RequestInfo request)
{
	// Inits:
	RequestResult result;
	SignupRequest signupRequest = JsonRequestPacketDeserializer::deserializeSignupRequest(request.buffer);

	// Trying to signup:
	m_loginManager.signup(signupRequest.username, signupRequest.password, signupRequest.email);
	SignupResponse response = { SUCCESS_STATUS };
	result.buffer = JsonResponsePacketSerializer::serializeResponse(response);

	// Returning the result:
	result.newHandler = nullptr;
	return result;
}
