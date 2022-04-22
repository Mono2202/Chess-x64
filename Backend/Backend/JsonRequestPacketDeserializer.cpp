#include "JsonRequestPacketDeserializer.h"

// Static Methods:

LoginRequest JsonRequestPacketDeserializer::deserializeLoginRequest(Buffer& buffer)
{
    // Inits:
    LoginRequest request;
    json data = getJsonData(buffer);

    // Building the request:
    request.username = data["Username"];
    request.password = data["Password"];

    return request;
}

SignupRequest JsonRequestPacketDeserializer::deserializeSignupRequest(Buffer& buffer)
{
    // Inits:
    SignupRequest request;
    json data = getJsonData(buffer);

    // Building the request:
    request.username = data["Username"];
    request.password = data["Password"];
    request.email = data["Email"];

    return request;
}

GetPlayersInRoomRequest JsonRequestPacketDeserializer::deserializeGetPlayersInRoomRequest(Buffer& buffer)
{
    // Inits:
    GetPlayersInRoomRequest request;
    json data = getJsonData(buffer);

    // Building the request:
    request.roomID = data["RoomID"];

    return request;
}

JoinRoomRequest JsonRequestPacketDeserializer::deserializeJoinRoomRequest(Buffer& buffer)
{
    // Inits:
    JoinRoomRequest request;
    json data = getJsonData(buffer);

    // Building the request:
    request.roomID = data["RoomID"];

    return request;
}

CreateRoomRequest JsonRequestPacketDeserializer::deserializeCreateRoomRequest(Buffer& buffer)
{
    // Inits:
    CreateRoomRequest request;
    json data = getJsonData(buffer);

    // Building the request:
    request.gameMode = data["GameMode"];

    return request;
}

SubmitMoveRequest JsonRequestPacketDeserializer::deserializeSubmitMoveRequest(Buffer& buffer)
{
    // Inits:
    SubmitMoveRequest request;
    json data = getJsonData(buffer);

    // Building the request:
    request.move = data["Move"];

    return request;
}

SearchPrivateRoomRequest JsonRequestPacketDeserializer::deserializeSearchPrivateRoomRequest(Buffer& buffer)
{
    // Inits:
    SearchPrivateRoomRequest request;
    json data = getJsonData(buffer);

    // Building the request:
    request.roomCode = data["RoomCode"];

    return request;
}


// Private Static Methods:

/*
Converts the buffer to a JSON object
Input : buffer - the data
Output: the JSON parsed object
*/
json JsonRequestPacketDeserializer::getJsonData(Buffer& buffer)
{
    // Inits:
    string data = "";
    int i = 0;

    // Reading the data:
    for (i = DATA_STARTING_BYTE; i < buffer.size(); i++) {
        data += buffer[i];
    }

    // Returning the parsed data:
    return json::parse(data);
}
