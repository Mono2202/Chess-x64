#include "MenuRequestHandler.h"

int MenuRequestHandler::currId = STARTING_ID;

// C'tors:

MenuRequestHandler::MenuRequestHandler(RoomManager& roomManager, StatisticsManager& statisticsManager, LoggedUser& user, RequestHandlerFactory& handlerFactory) :
    m_roomManager(roomManager), m_statisticsManager(statisticsManager), m_user(user), m_handlerFactory(handlerFactory) { }


// Virtual Methods:

/*
Checking whether the request is relevant or not
Input : request - the request info
Output: true	- relevant request
        false	- otherwise
*/
bool MenuRequestHandler::isRequestRelevant(RequestInfo request)
{
    // Condition: relevant request
    if ((request.buffer[0] >= LOGOUT_REQUEST && request.buffer[0] <= SEARCH_ELO_ROOM_REQUEST) ||
        request.buffer[0] == SEARCH_PRIVATE_ROOM_REQUEST || request.buffer[0] == GET_MATCH_HISTORY_REQUEST) {
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
RequestResult MenuRequestHandler::handleRequest(RequestInfo request)
{
    switch (request.buffer[0])
    {
    case LOGOUT_REQUEST:                return signout(request);
    case GET_ROOMS_REQUEST:             return getRooms(request);
    case GET_PLAYERS_IN_ROOM_REQUEST:   return getPlayersInRoom(request);
    case GET_HIGH_SCORE_REQUEST:        return getHighScore(request);
    case GET_PERSONAL_STATS_REQUEST:    return getPersonalStats(request);
    case JOIN_ROOM_REQUEST:             return joinRoom(request);
    case CREATE_ROOM_REQUEST:           return createRoom(request);
    case SEARCH_ELO_ROOM_REQUEST:       return searchEloRoom(request);
    case SEARCH_PRIVATE_ROOM_REQUEST:   return searchPrivateRoom(request);
    case GET_MATCH_HISTORY_REQUEST:     return getMatchHistory(request);
    }
}

// Private Methods:

/*
Signing-out from an account
Input : request - the signout request
Output: result  - the request result
*/
RequestResult MenuRequestHandler::signout(RequestInfo request)
{
    // Inits:
    RequestResult result;

    // Creating Response:
    m_handlerFactory.getLoginManager().logout(m_user.getUsername());
    LogoutResponse response = { SUCCESS_STATUS };

    // Creating Result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = m_handlerFactory.createLoginRequestHandler();
    return result;
}

/*
Getting rooms
Input : request - the get rooms request
Output: result  - the request result
*/
RequestResult MenuRequestHandler::getRooms(RequestInfo request)
{
    // Inits:
    RequestResult result;
    GetRoomsResponse response = { SUCCESS_STATUS, m_roomManager.getRooms() };

    // Creating Result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = nullptr;
    return result;
}

/*
Getting the players in a room
Input : request - the get players in room request
Output: result  - the request result
*/
RequestResult MenuRequestHandler::getPlayersInRoom(RequestInfo request)
{
    // Inits:
    RequestResult result;
    GetPlayersInRoomRequest getPlayersRequest = JsonRequestPacketDeserializer::deserializeGetPlayersInRoomRequest(request.buffer);

    // Creating Response:
    GetPlayersInRoomResponse response;
    response.players = m_roomManager.getRoom(getPlayersRequest.roomID)->getAllUsers();

    // Creating Result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = nullptr;
    return result;
}

/*
Getting personal statistics
Input : request - the get personal stats request
Output: result  - the request result
*/
RequestResult MenuRequestHandler::getPersonalStats(RequestInfo request)
{
    // Inits:
    RequestResult result;
    GetPersonalStatsResponse response;
    GetPersonalStatsRequest statsRequest = JsonRequestPacketDeserializer::deserializeGetPersonalStatsRequest(request.buffer);

    // Creating Response:
    response.status = SUCCESS_STATUS;
    response.statistics = m_statisticsManager.getUserStatistics(statsRequest.username);

    // Creating Result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = nullptr;
    return result;
}

/*
Getting highscores
Input : request - the get highscores request
Output: result  - the request result
*/
RequestResult MenuRequestHandler::getHighScore(RequestInfo request)
{
    // Inits:
    RequestResult result;
    GetHighScoreResponse response;

    // Creating Response:
    response.status = SUCCESS_STATUS;
    response.statistics = m_statisticsManager.getHighScore();

    // Creating Result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = nullptr;
    return result;
}

/*
Joining a room
Input : request - the join room request
Output: result  - the request result
*/
RequestResult MenuRequestHandler::joinRoom(RequestInfo request)
{
    // Inits:
    RequestResult result;
    JoinRoomRequest joinRequest = JsonRequestPacketDeserializer::deserializeJoinRoomRequest(request.buffer);

    // Creating Response:
    JoinRoomResponse response;
    m_roomManager.getRoom(joinRequest.roomID)->addUser(m_user);
    m_roomManager.getRoom(joinRequest.roomID)->setIsActive(true);
    m_roomManager.getRoom(joinRequest.roomID)->setCurrentMove("random");
    response.status = SUCCESS_STATUS;

    // Creating Result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = m_handlerFactory.createRoomRequestHandler(m_user, *m_roomManager.getRoom(joinRequest.roomID));
    return result;
}

/*
Creating a room
Input : request - the create room request
Output: result  - the request result
*/
RequestResult MenuRequestHandler::createRoom(RequestInfo request)
{
    // Inits:
    RequestResult result;
    CreateRoomRequest createRequest = JsonRequestPacketDeserializer::deserializeCreateRoomRequest(request.buffer);

    // Creating Response:
    CreateRoomResponse response;
    RoomData data;
    data.id = currId++;
    data.isActive = 0;
    data.currentMove = "";
    data.gameMode = createRequest.gameMode;
    m_roomManager.createRoom(m_user, data);
    response.status = SUCCESS_STATUS;

    // Setting the room ID:
    response.roomID = data.id;

    // Creating Result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = m_handlerFactory.createRoomRequestHandler(m_user, *m_roomManager.getRoom(data.id));
    return result;
}

/*
Searching for an ELO room
Input : request - the search ELO room request
Output: result  - the request result
*/
RequestResult MenuRequestHandler::searchEloRoom(RequestInfo request)
{
    // Inits:
    RequestResult result;
    SearchEloRoomResponse response = { SUCCESS_STATUS, m_roomManager.getEloRoom() };

    // Creating Result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = nullptr;
    return result;
}

/*
Searching for a private room
Input : request - the search private room request
Output: result  - the request result
*/
RequestResult MenuRequestHandler::searchPrivateRoom(RequestInfo request)
{
    // Inits:
    RequestResult result;
    SearchPrivateRoomRequest searchRequest = JsonRequestPacketDeserializer::deserializeSearchPrivateRoomRequest(request.buffer);
    SearchPrivateRoomResponse response = { SUCCESS_STATUS, m_roomManager.getPrivateRoom(searchRequest.roomCode) };

    // Creating Result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = nullptr;
    return result;
}

/*
Getting match history of a user
Input : request - the get match history request
Output: result  - the request result
*/
RequestResult MenuRequestHandler::getMatchHistory(RequestInfo request)
{
    // Inits:
    RequestResult result;
    GetMatchHistoryResponse response;
    GetMatchHistoryRequest statsRequest = JsonRequestPacketDeserializer::deserializeGetMatchHistoryRequest(request.buffer);

    // Creating Response:
    response.status = SUCCESS_STATUS;
    response.games = m_statisticsManager.getGames(statsRequest.username);

    // Creating Result:
    result.buffer = JsonResponsePacketSerializer::serializeResponse(response);
    result.newHandler = nullptr;
    return result;
}
