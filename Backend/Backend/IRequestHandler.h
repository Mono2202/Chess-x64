#pragma once

// Includes:
#include "JsonResponsePacketSerializer.h"
#include "JsonRequestPacketDeserializer.h"
#include <ctime>

// Class Declaration:
class IRequestHandler;

// Using:
using std::vector;

// Status Defines:
#define SUCCESS_STATUS 1

// Request Codes Enum:
enum RequestCodes
{
	LOGIN_REQUEST = 101,
	SIGNUP_REQUEST,
	LOGOUT_REQUEST,
	GET_ROOMS_REQUEST,
	GET_PLAYERS_IN_ROOM_REQUEST,
	GET_HIGH_SCORE_REQUEST,
	GET_PERSONAL_STATS_REQUEST,
	JOIN_ROOM_REQUEST,
	CREATE_ROOM_REQUEST,
	SEARCH_ELO_ROOM_REQUEST,
	GET_ROOM_STATE_REQUEST,
	LEAVE_ROOM_REQUEST,
	SUBMIT_MOVE_REQUEST
};

// RequestInfo Struct:
typedef struct RequestInfo {
	int id;
	std::time_t receivalTime;
	Buffer buffer;
} RequestInfo;

// RequestResult Struct:
typedef struct RequestResult {
	Buffer buffer;
	IRequestHandler* newHandler;
} RequestResult;

// IRequestHandler Class:
class IRequestHandler
{
public:
	// Pure Virtual Methods:
	virtual bool isRequestRelevant(RequestInfo request) = 0;
	virtual RequestResult handleRequest(RequestInfo request) = 0;
};
