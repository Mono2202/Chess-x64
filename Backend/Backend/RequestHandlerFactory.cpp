#include "RequestHandlerFactory.h"

// C'tors:

RequestHandlerFactory::RequestHandlerFactory(const LoginManager& loginManager, IDatabase* database, const RoomManager& roomManager, const StatisticsManager& statisticsManager) :
    m_loginManager(loginManager), m_database(database), m_roomManager(roomManager), m_statisticsManager(statisticsManager) {  }


// D'tors:

RequestHandlerFactory::~RequestHandlerFactory() { m_database = nullptr; }


// Methods:

/*
Creating a login request handler
Input : < None >
Output: the login request handler
*/
LoginRequestHandler* RequestHandlerFactory::createLoginRequestHandler() { return new LoginRequestHandler(m_loginManager, *this); }

/*
Creating a menu request handler
Input : < None >
Output: the menu request handler
*/
MenuRequestHandler* RequestHandlerFactory::createMenuRequestHandler(LoggedUser& user) { return new MenuRequestHandler(m_roomManager, m_statisticsManager, user, *this); }

/*
Creating a new room member request handler
Input : < None >
Output: the room member request handler
*/
RoomRequestHandler* RequestHandlerFactory::createRoomRequestHandler(LoggedUser& user, Room& room) { return new RoomRequestHandler(room, user, m_roomManager, *this); }

/*
Returning the login manager
Input : < None >
Output: the login manager
*/
LoginManager& RequestHandlerFactory::getLoginManager() { return m_loginManager; }

/*
Returning the room manager
Input : < None >
Output: the room manager
*/
RoomManager& RequestHandlerFactory::getRoomManager() { return m_roomManager; }

/*
Returning the statistics manager
Input : < None >
Output: the statistics manager
*/
StatisticsManager& RequestHandlerFactory::getStatisticsManager() { return m_statisticsManager; }
