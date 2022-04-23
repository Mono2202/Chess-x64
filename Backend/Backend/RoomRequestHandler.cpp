#define _CRT_SECURE_NO_WARNINGS
#include "RoomRequestHandler.h"
#include "RequestHandlerFactory.h"
#include <iostream>

// C'tor:

RoomRequestHandler::RoomRequestHandler(Room& room, LoggedUser& user, RoomManager& roomManager, RequestHandlerFactory& handlerFactory) : m_room(room), m_user(user),
m_roomManager(roomManager), m_handlerFactory(handlerFactory)
{}

/*
Checking whether the request is relevant or not
Input : request - the request info
Output: true	- relevant request
        false	- otherwise
*/
bool RoomRequestHandler::isRequestRelevant(RequestInfo request)
{
    // Condition: relevant request
    if (request.buffer[0] >= GET_ROOM_STATE_REQUEST && request.buffer[0] <= SUBMIT_MOVE_REQUEST) {
        return true;
    }

    // Condition: irrelevant request
    return false;
}

/*
Handling the request
Input : request - the request info
Output: result  - the request result
*/
RequestResult RoomRequestHandler::handleRequest(RequestInfo request)
{
    switch (request.buffer[0])
    {
    case GET_ROOM_STATE_REQUEST:    return getRoomState(request);
    case LEAVE_ROOM_REQUEST:        return leaveRoom(request);
    case SUBMIT_MOVE_REQUEST:       return submitMove(request);
    }
}


// Private Methods:

/*
Getting Room State
Input : request - the closeRoom request
Output: result  - the request result
*/
RequestResult RoomRequestHandler::getRoomState(RequestInfo request)
{
    // Inits:
    RequestResult result;

    // Updating room:
    m_room = *(m_roomManager.getRoom(m_room.getRoomData().id));
    RoomData data = m_room.getRoomData();

    // Creating response:
    GetRoomStateResponse response;
    response.isActive = data.isActive;
    response.players = m_room.getAllUsers();
    response.currentMove = data.currentMove;
    response.gameMode = data.gameMode;
    response.status = SUCCESS_STATUS;

    // Creating result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = nullptr;
    return result;
}

/*
Leaving the Room
Input : request - the leaveRoom request
Output: result  - the request result
*/
RequestResult RoomRequestHandler::leaveRoom(RequestInfo request)
{
    // Inits:
    RequestResult result;

    // Removing the current user from the room:
    m_roomManager.getRoom(m_room.getRoomData().id)->removeUser(m_user);

    // Condition: there is a user in the room
    if (m_roomManager.getRoom(m_room.getRoomData().id)->getAllUsers().size() > 0 &&
        m_roomManager.getRoom(m_room.getRoomData().id)->getIsActive()) {
        // Updating the room:
        m_roomManager.getRoom(m_room.getRoomData().id)->setCurrentMove("OPPONENT LEFT");
        m_roomManager.getRoom(m_room.getRoomData().id)->setIsActive(false);

        // Getting the other player:
        string otherUser = (m_room.getAllUsers()[0] != m_user.getUsername()) ? m_room.getAllUsers()[0] : m_room.getAllUsers()[1];

        // Adding the stats:
        m_roomManager.getDatabase()->addStatistics(m_user.getUsername(), LOST_GAME);
        m_roomManager.getDatabase()->addStatistics(otherUser, WON_GAME);
        m_roomManager.getRoom(m_room.getRoomData().id)->setWinner(otherUser);
        std::cout << "Win\n";
    }

    // Condition: 0 users in the room
    else if (m_roomManager.getRoom(m_room.getRoomData().id)->getAllUsers().size() == 0) {
        // Getting the current date:
        auto t = std::time(nullptr);
        auto tm = *std::localtime(&t);
        std::ostringstream oss;
        oss << std::put_time(&tm, "%d/%m/%Y");
        string date = oss.str();

        // Adding the game:
        m_handlerFactory.m_database->addGame(m_roomManager.getRoom(m_room.getRoomData().id)->getUsernames()[0],
            m_roomManager.getRoom(m_room.getRoomData().id)->getUsernames()[1], m_roomManager.getRoom(m_room.getRoomData().id)->getMoves(),
            m_roomManager.getRoom(m_room.getRoomData().id)->getWinner(), date);
        
        // Deleting the room:
        m_roomManager.deleteRoom(m_room.getRoomData().id);
        std::cout << "Delete\n";
    }

    // Creating response:
    LeaveRoomResponse response;
    response.status = true;

    // Creating result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = m_handlerFactory.createMenuRequestHandler(m_user);
    return result;
}

/*
Submiting Move
Input : request - the submitMove request
Output: result  - the request result
*/
RequestResult RoomRequestHandler::submitMove(RequestInfo request)
{
    // Inits:
    RequestResult result;
    SubmitMoveRequest deserializedRequest = JsonRequestPacketDeserializer::deserializeSubmitMoveRequest(request.buffer);

    // Creating Response:
    m_roomManager.getRoom(m_room.getRoomData().id)->setCurrentMove(deserializedRequest.move);
    m_roomManager.getRoom(m_room.getRoomData().id)->addMove(deserializedRequest.move);
    SubmitMoveResponse response = { SUCCESS_STATUS };

    // Checking if the game has ended by win:
    if (deserializedRequest.move.find('#') != std::string::npos)
    {
        // Adding the stats:
        m_roomManager.getDatabase()->addStatistics(m_user.getUsername(), WON_GAME);
        m_roomManager.getRoom(m_room.getRoomData().id)->setWinner(m_user.getUsername());
        string otherUser = (m_room.getAllUsers()[0] != m_user.getUsername()) ? m_room.getAllUsers()[0] : m_room.getAllUsers()[1];
        m_roomManager.getDatabase()->addStatistics(otherUser, LOST_GAME);
        m_roomManager.getRoom(m_room.getRoomData().id)->setIsActive(false);
    }

    // Checking if the game has ended by tie:
    else if (deserializedRequest.move.find('%') != std::string::npos)
    {
        // Adding the stats:
        m_roomManager.getDatabase()->addStatistics(m_user.getUsername(), TIED_GAME);
        m_roomManager.getRoom(m_room.getRoomData().id)->setWinner("!TIE!");
        string otherUser = (m_room.getAllUsers()[0] != m_user.getUsername()) ? m_room.getAllUsers()[0] : m_room.getAllUsers()[1];
        m_roomManager.getDatabase()->addStatistics(otherUser, TIED_GAME);
        m_roomManager.getRoom(m_room.getRoomData().id)->setIsActive(false);
    }

    // Creating result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = nullptr;
    return result;
}
