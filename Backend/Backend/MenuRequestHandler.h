#pragma once

// Includes:
#include "IRequestHandler.h"
#include "RoomManager.h"
#include "StatisticsManager.h"
#include "RequestHandlerFactory.h"

// Defines:
#define STARTING_ID 1234

// Class Declaration:
class RequestHandlerFactory;

// MenuRequestHandler Class:
class MenuRequestHandler : public IRequestHandler
{
public:
	// Static Variable:
	static int currId;

	// C'tor & D'tor:
	MenuRequestHandler(RoomManager& roomManager, StatisticsManager& statisticsManager, LoggedUser& user, RequestHandlerFactory& handlerFactory);
	~MenuRequestHandler() = default;

	// Virtual Methods:
	virtual bool isRequestRelevant(RequestInfo request);
	virtual RequestResult handleRequest(RequestInfo request);

private:
	// Fields:
	LoggedUser m_user;
	RoomManager& m_roomManager;
	StatisticsManager& m_statisticsManager;
	RequestHandlerFactory& m_handlerFactory;

	// Private Methods:
	RequestResult signout(RequestInfo request);
	RequestResult getRooms(RequestInfo request);
	RequestResult getPlayersInRoom(RequestInfo request);
	RequestResult getPersonalStats(RequestInfo request);
	RequestResult getHighScore(RequestInfo request);
	RequestResult joinRoom(RequestInfo request);
	RequestResult createRoom(RequestInfo request);
	RequestResult searchEloRoom(RequestInfo request);
	RequestResult searchPrivateRoom(RequestInfo request);
	RequestResult getMatchHistory(RequestInfo request);
};
