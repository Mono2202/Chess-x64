#pragma once

// Includes:
#include <string>
#include <vector>
#include <algorithm> 

// Using:
using std::string;
using std::vector;


// IDatabase Class:
class IDatabase
{
public:
	// D'tors:
	virtual ~IDatabase() = default;

	// DB Functions:
	virtual bool open() = 0;
	virtual void close() = 0;


	/* Users Table */

	// Queries:
	virtual bool doesUserExist(const string& username) = 0;
	virtual bool doesPasswordMatch(const string& username, const string& password) = 0;

	// Actions:
	virtual void addNewUser(const string& username, const string& password, const string& email) = 0;

	/* Statistics Table */

	// Queries:
	virtual int getNumOfPlayerWins(const string& username) = 0;
	virtual int getNumOfPlayerLosses(const string& username) = 0;
	virtual int getNumOfPlayerGames(const string& username) = 0;
	virtual vector<string> getHighScores() = 0;
	virtual void addStatistics(const string& username, bool wonGame) = 0;
};
