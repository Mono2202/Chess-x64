#include "JsonResponsePacketSerializer.h"

// Static Methods:

Buffer JsonResponsePacketSerializer::serializeResponse(const ErrorResponse& response)
{
	// Building the data:
	json data;
	data["Message"] = response.message;

	// Building the buffer:
	return insertData(data, ERROR_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const LoginResponse& response)
{
	// Building the buffer:
	return insertData(json({}), LOGIN_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const SignupResponse& response)
{
	// Building the buffer:
	return insertData(json({}), SIGNUP_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const LogoutResponse& response)
{
	// Building the buffer:
	return insertData(json({}), LOGOUT_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const GetRoomsResponse& response)
{
	// Inits:
	json data;
	string roomList = "";
	int i = 0;

	// Concatenating the rooms vector to a string:
	for (i = 0; i < response.rooms.size(); i++) {
		roomList += std::to_string(response.rooms[i].id) + ", ";
	}

	// Building the data:
	data["Rooms"] = roomList.substr(0, roomList.size() - 2);

	// Building the buffer:
	return insertData(data, GET_ROOMS_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const GetPlayersInRoomResponse& response)
{
	// Inits:
	json data;
	string playersList = "";
	int i = 0;

	// Concatenating the players vector to a string:
	for (i = 0; i < response.players.size(); i++) {
		playersList += response.players[i] + ", ";
	}

	// Building the data:
	data["PlayersInRoom"] = playersList.substr(0, playersList.size() - 2);

	// Building the buffer:
	return insertData(data, GET_PLAYERS_IN_ROOM_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const GetHighScoreResponse& response)
{
	// Inits:
	json data;
	string highScoresList = "";
	int i = 0;

	// Concatenating the highscores vector to a string:
	for (i = 0; i < response.statistics.size(); i++) {
		highScoresList += response.statistics[i] + ", ";
	}

	// Building the data:
	data["HighScores"] = highScoresList.substr(0, highScoresList.size() - 2);

	// Building the buffer:
	return insertData(data, GET_HIGH_SCORE_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const GetPersonalStatsResponse& response)
{
	// Inits:
	Buffer buffer;
	json data;
	string personalStatsList = "";
	int i = 0;

	// Concatenating the players statistics vector to a string:
	for (i = 0; i < response.statistics.size(); i++) {
		personalStatsList += response.statistics[i] + ", ";
	}

	// Building the data:
	data["Statistics"] = personalStatsList.substr(0, personalStatsList.size() - 2);

	// Building the buffer:
	return insertData(data, GET_PERSONAL_STATS_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const JoinRoomResponse& response)
{
	// Building the buffer:
	return insertData(json({}), JOIN_ROOM_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const CreateRoomResponse& response)
{
	// Building the data:
	json data;
	data["RoomID"] = response.roomID;

	// Building the buffer:
	return insertData(data, CREATE_ROOM_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const SearchEloRoomResponse& response)
{
	// Inits:
	json data;
	int i = 0;
	
	// Building the data:
	data["RoomID"] = response.roomData.id;

	// Building the buffer:
	return insertData(data, SEARCH_ELO_ROOM_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const GetRoomStateResponse& response)
{
	// Inits:
	json data;
	string playersList = "";
	int i = 0;

	// Concatenating the players vector to a string:
	for (i = 0; i < response.players.size(); i++) {
		playersList += response.players[i] + ", ";
	}

	// Building the data:
	data["IsActive"] = response.isActive;
	data["Players"] = playersList.substr(0, playersList.size() - 2);
	data["CurrentMove"] = response.currentMove;
	data["GameMode"] = response.gameMode;

	// Building the buffer:
	return insertData(data, GET_ROOM_STATE_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const LeaveRoomResponse& response)
{
	// Building the buffer:
	return insertData(json({}), LEAVE_ROOM_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const SubmitMoveResponse& response)
{
	// Building the buffer:
	return insertData(json({}), SUBMIT_MOVE_RESPONSE, response.status);
}

Buffer JsonResponsePacketSerializer::serializeResponse(const SearchPrivateRoomResponse& response)
{
	// Inits:
	json data;
	int i = 0;

	// Building the data:
	data["RoomID"] = response.roomData.id;

	// Building the buffer:
	return insertData(data, SEARCH_PRIVATE_ROOM_RESPONSE, response.status);
}


// Private Static Methods:

/*
Inserting the data to the buffer
Input : data		 - the json data
		responseCode - the response code
		status		 - the response status
Output: buffer		 - the data buffer
*/
Buffer JsonResponsePacketSerializer::insertData(json data, ResponseCode responseCode, unsigned int status)
{
	// Inits:
	Buffer buffer;
	data["Status"] = status;
	string dataStr = data.dump();
	int i = 0;

	// Inserting the data:
	buffer.push_back(responseCode);
	for (i = 0; i < dataStr.length(); i++) {
		// Pushing the current data char byte:
		buffer.push_back(dataStr[i]);
	}

	return buffer;
}
