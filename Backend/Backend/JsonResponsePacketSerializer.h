#pragma once

// Includes:
#include "json.hpp"
#include <sstream>
#include <string>
#include <iomanip>
#include "Room.h"

// Response Codes Enum:
enum ResponseCode
{
	ERROR_RESPONSE = 2,
	LOGIN_RESPONSE,
	SIGNUP_RESPONSE,
	LOGOUT_RESPONSE,
	GET_ROOMS_RESPONSE,
	GET_PLAYERS_IN_ROOM_RESPONSE,
	GET_HIGH_SCORE_RESPONSE,
	GET_PERSONAL_STATS_RESPONSE,
	JOIN_ROOM_RESPONSE,
	CREATE_ROOM_RESPONSE,
	SEARCH_ELO_ROOM_RESPONSE,
	GET_ROOM_STATE_RESPONSE,
	LEAVE_ROOM_RESPONSE,
	SUBMIT_MOVE_RESPONSE,
	SEARCH_PRIVATE_ROOM_RESPONSE,
	GET_MATCH_HISTORY_RESPONSE
};

// Using:
using std::string;
using std::vector;
using std::map;
using json = nlohmann::json;

// Typedef:
typedef vector<unsigned char> Buffer;

// ErrorResponse Struct:
typedef struct ErrorResponse {
	unsigned int status;
	string message;
} ErrorResponse;

// LoginResponse Struct:
typedef struct LoginResponse {
	unsigned int status;
} LoginResponse;

// SignupResponse Struct:
typedef struct SignupResponse {
	unsigned int status;
} SignupResponse;

// LogoutResponse Struct:
typedef struct LogoutResponse {
	unsigned int status;
} LogoutResponse;

// GetRoomsResponse Struct:
typedef struct GetRoomsResponse {
	unsigned int status;
	vector<RoomData> rooms;
} GetRoomsResponse;

// GetPlayersInRoomResponse Struct:
typedef struct GetPlayersInRoomResponse {
	unsigned int status;
	vector<string> players;
} GetPlayersInRoomResponse;

// GetHighScoreResponse Struct:
typedef struct GetHighScoreResponse {
	unsigned int status;
	vector<string> statistics;
} GetHighScoreResponse;

// GetPersonalStatsResponse Struct:
typedef struct GetPersonalStatsResponse {
	unsigned int status;
	vector<string> statistics;
} GetPersonalStatsResponse;

// JoinRoomResponse Struct:
typedef struct JoinRoomResponse {
	unsigned int status;
} JoinRoomResponse;

// CreateRoomResponse Struct:
typedef struct CreateRoomResponse {
	unsigned int status;
	unsigned int roomID;
} CreateRoomResponse;

// SearchEloRoomResponse Struct:
typedef struct SearchEloRoomResponse {
	unsigned int status;
	RoomData roomData;
} SearchEloRoomResponse;

// GetRoomStateResponse Struct:
typedef struct GetRoomStateResponse {
	unsigned int status;
	bool isActive;
	vector<string> players;
	string currentMove;
	string gameMode;
} GetRoomStateResponse;

// LeaveRoomResponse Struct:
typedef struct LeaveRoomResponse {
	unsigned int status;
} LeaveRoomResponse;

// SubmitMoveResponse Struct:
typedef struct SubmitMoveResponse {
	unsigned int status;
} SubmitMoveResponse;

// SearchPrivateRoomResponse Struct:
typedef struct SearchPrivateRoomResponse {
	unsigned int status;
	RoomData roomData;
} SearchPrivateRoomResponse;

// GetMatchHistoryResponse Struct:
typedef struct GetMatchHistoryResponse {
	unsigned int status;
	vector<string> games;
} GetMatchHistoryResponse;

// JsonResponsePacketSerializer Class:
class JsonResponsePacketSerializer
{
public:
	// Static Methods:
	static Buffer serializeResponse(const ErrorResponse& response);
	static Buffer serializeResponse(const LoginResponse& response);
	static Buffer serializeResponse(const SignupResponse& response);
	static Buffer serializeResponse(const LogoutResponse& response);
	static Buffer serializeResponse(const GetRoomsResponse& response);
	static Buffer serializeResponse(const GetPlayersInRoomResponse& response);
	static Buffer serializeResponse(const GetHighScoreResponse& response);
	static Buffer serializeResponse(const GetPersonalStatsResponse& response);
	static Buffer serializeResponse(const JoinRoomResponse& response);
	static Buffer serializeResponse(const CreateRoomResponse& response);
	static Buffer serializeResponse(const SearchEloRoomResponse& response);
	static Buffer serializeResponse(const GetRoomStateResponse& response);
	static Buffer serializeResponse(const LeaveRoomResponse& response);
	static Buffer serializeResponse(const SubmitMoveResponse& response);
	static Buffer serializeResponse(const SearchPrivateRoomResponse& response);
	static Buffer serializeResponse(const GetMatchHistoryResponse& response);

private:
	// Private Static Methods:
	static Buffer insertData(json data, ResponseCode responseCode, unsigned int status);
};
