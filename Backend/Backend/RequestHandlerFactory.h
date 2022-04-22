#pragma once

// Includes:
#include "IDatabase.h"
#include "LoginManager.h"
#include "LoginRequestHandler.h"
#include "MenuRequestHandler.h"
#include "RoomRequestHandler.h"

// Class Declaration:
class LoginRequestHandler;
class MenuRequestHandler;
class RoomRequestHandler;
class RoomAdminRequestHandler;
class RoomMemberRequestHandler;
class GameRequestHandler;

// RequestHandlerFactory Class:
class RequestHandlerFactory
{
public:
    // C'tors & D'tors:
    RequestHandlerFactory(const LoginManager& loginManager, IDatabase* database, const RoomManager& roomManager, const StatisticsManager& statisticsManager);
    ~RequestHandlerFactory();

    // Methods:
    LoginRequestHandler* createLoginRequestHandler();
    MenuRequestHandler* createMenuRequestHandler(LoggedUser& user);
    RoomRequestHandler* createRoomRequestHandler(LoggedUser& user, Room& room);
    LoginManager& getLoginManager();
    RoomManager& getRoomManager();
    StatisticsManager& getStatisticsManager();

    // Fields:
    IDatabase* m_database;

private:
    // Fields:
    LoginManager m_loginManager;
    RoomManager m_roomManager;
    StatisticsManager m_statisticsManager;
};
