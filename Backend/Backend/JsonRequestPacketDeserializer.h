#pragma once

// Includes:
#include "json.hpp"
#include <sstream>
#include <string>
#include <iomanip>

// General Defines:
#define DATA_STARTING_BYTE 1

// Using:
using std::string;
using std::vector;
using json = nlohmann::json;

// Typedef:
typedef vector<unsigned char> Buffer;

// LoginRequest Struct:
typedef struct LoginRequest {
	string username;
	string password;
} LoginRequest;

// SignupRequest Struct:
typedef struct SignupRequest {
	string username;
	string password;
	string email;
} SignupRequest;

// GetPlayersInRoomRequest Struct:
typedef struct GetPlayersInRoomRequest {
	unsigned int roomID;
} GetPlayersInRoomRequest;

// JoinRoomRequest Struct:
typedef struct JoinRoomRequest {
	unsigned int roomID;
} JoinRoomRequest;

// CreateRoomRequest Struct:
typedef struct CreateRoomRequest {
	string gameMode;
} CreateRoomRequest;

// SubmitMoveRequest Struct:
typedef struct SubmitMoveRequest {
	string move;
} SubmitMoveRequest;

// SearchPrivateRoomRequest Struct:
typedef struct SearchPrivateRoomRequest {
	string roomCode;
} SearchPrivateRoomRequest;

// JsonRequestPacketDeserializer Class:
class JsonRequestPacketDeserializer
{
public:
	// Static Methods:
	static LoginRequest deserializeLoginRequest(Buffer& buffer);
	static SignupRequest deserializeSignupRequest(Buffer& buffer);
	static GetPlayersInRoomRequest deserializeGetPlayersInRoomRequest(Buffer& buffer);
	static JoinRoomRequest deserializeJoinRoomRequest(Buffer& buffer);
	static CreateRoomRequest deserializeCreateRoomRequest(Buffer& buffer);
	static SubmitMoveRequest deserializeSubmitMoveRequest(Buffer& buffer);
	static SearchPrivateRoomRequest deserializeSearchPrivateRoomRequest(Buffer& buffer);

private:
	// Private Static Methods:
	static json getJsonData(Buffer& buffer);
};
