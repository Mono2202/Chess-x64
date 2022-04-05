#include "RoomRequestHandler.h"
#include "RequestHandlerFactory.h"

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
    if (m_room.getAllUsers().size()) {
        m_roomManager.getRoom(m_room.getRoomData().id)->setCurrentMove("WIN");
        m_roomManager.getRoom(m_room.getRoomData().id)->setIsActive(false);
    }

    // Condition: 0 users in the room
    else {
        m_roomManager.deleteRoom(m_room.getRoomData().id);
    }

    // Creating response:
    LeaveRoomResponse response;
    response.status = true;

    // Creating result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = m_handlerFactory.createMenuRequestHandler(m_user);;
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

    // Creating Response:
    m_roomManager.getRoom(m_room.getRoomData().id)->setCurrentMove(JsonRequestPacketDeserializer::deserializeSubmitMoveRequest(request.buffer).move);
    SubmitMoveResponse response = { SUCCESS_STATUS };

    // TODO: FINISH GAME, ADD STATS

    // Creating result;
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = nullptr;
    return result;
}
