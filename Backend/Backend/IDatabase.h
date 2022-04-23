#pragma once

// Includes:
#include <string>
#include <vector>
#include <algorithm> 

// Using:
using std::string;
using std::vector;

// Defines:
#define WON_GAME 0
#define LOST_GAME 1
#define TIED_GAME 2


// IDatabase Class:
class IDatabase
{
public:
	// D'tors:
	virtual ~IDatabase() = default;

	// DB Functions:
	virtual bool open() = 0;
	virtual void close() = 0;


	// Users Table:
	virtual bool doesUserExist(const string& username) = 0;
	virtual bool doesPasswordMatch(const string& username, const string& password) = 0;
	virtual void addNewUser(const string& username, const string& password, const string& email) = 0;

	// Statistics Table:
	virtual int getNumOfPlayerWins(const string& username) = 0;
	virtual int getNumOfPlayerLosses(const string& username) = 0;
	virtual int getNumOfPlayerTies(const string& username) = 0;
	virtual int getNumOfPlayerGames(const string& username) = 0;
	virtual int getPlayerElo(const string& username) = 0;
	virtual vector<string> getHighScores() = 0;
	virtual void addStatistics(const string& username, int gameStatus) = 0;

	// Games Table:
	virtual void addGame(const string& whiteUsername, const string& blackUsername, const string& game,
		const string& wonUsername, const string& date) = 0;
	virtual vector<string> getGames(const string& username) = 0;
};
