#pragma once

// Includes:
#include "IRequestHandler.h"
#include "RoomManager.h"

// Class Declaration:
class RequestHandlerFactory;
class RoomMemberRequestHandler;
class RoomAdminRequestHandler;

class RoomRequestHandler : public IRequestHandler
{
public:
	// C'tor & D'tor:
	RoomRequestHandler(Room& room, LoggedUser& user, RoomManager& roomManager, RequestHandlerFactory& handlerFactory);
	~RoomRequestHandler() = default;

	// Pure Virtual Methods:
	bool isRequestRelevant(RequestInfo request);
	RequestResult handleRequest(RequestInfo request);

private:
	// Fields:
	Room m_room;
	LoggedUser m_user;
	RoomManager& m_roomManager;
	RequestHandlerFactory& m_handlerFactory;

	// Protected Methods:
	RequestResult getRoomState(RequestInfo request);
	RequestResult leaveRoom(RequestInfo request);
	RequestResult submitMove(RequestInfo request);
};
