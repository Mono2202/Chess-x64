#pragma once

// Includes:
#include "LoggedUser.h"
#include <vector>
#include <algorithm>

// Defines:
#define MAX_PLAYERS 2
#define ELO_GAME_MODE 'E'
#define PRIVATE_GAME_MODE 'P'

// Using:
using std::string;
using std::vector;

// RoomData Struct:
typedef struct RoomData {
	unsigned int id;
	bool isActive;
	string currentMove;
	string gameMode;
} RoomData;

// Room Class:
class Room
{
public:
	// C'tors & D'tors:
	Room();
	Room(const RoomData& data);
	~Room() = default;

	// Methods:
	void setIsActive(unsigned int flag);
	bool getIsActive() const;
	void setCurrentMove(string move);
	string getCurrentMove() const;
	void addMove(const string& move);
	string getMoves() const;
	vector<string> getUsernames() const;
	void setWinner(const string& username);
	string getWinner() const;
	void addUser(LoggedUser user);
	void removeUser(LoggedUser user);
	vector<string> getAllUsers() const;
	RoomData getRoomData() const;

private:
	// Fields:
	RoomData m_metadata;
	vector<LoggedUser> m_users;
	string moves;
	string whiteUsername;
	string blackUsername;
	string wonUsername;
};
