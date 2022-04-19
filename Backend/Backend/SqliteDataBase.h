#pragma once

// Includes:
#include <iostream>
#include <time.h>
#include "IDatabase.h"
#include "sqlite3.h"
#include "io.h"

// Defines:
#define DB_DOESNT_EXIST -1


// SqliteDataBase Class:
class SqliteDataBase : public IDatabase
{
public:
	// Static C'tor:
	static SqliteDataBase* getInstance();

	// D'tors:
	virtual ~SqliteDataBase();

	// DB Functions:
	virtual bool open();
	virtual void close();


	/* Users Table */

	// Queries:
	virtual bool doesUserExist(const string& username);
	virtual bool doesPasswordMatch(const string& username, const string& password);

	// Actions:
	virtual void addNewUser(const string& username, const string& password, const string& email);


	/* Statistics Table */

	// Queries:
	virtual int getNumOfPlayerWins(const string& username);
	virtual int getNumOfPlayerLosses(const string& username);
	virtual int getNumOfPlayerTies(const string& username);
	virtual int getNumOfPlayerGames(const string& username);
	virtual int getPlayerElo(const string& username);
	virtual vector<string> getHighScores();
	virtual void addStatistics(const string& username, int gameStatus);

private:
	// Private C'tor:
	SqliteDataBase() = default;

	// Fields:
	static SqliteDataBase* m_sqliteDataBaseInstance;
	sqlite3* m_db;

	// Callback Functions:
	static int outputExistsCallback(void* data, int argc, char** argv, char** azColName);
	static int intNumCallback(void* data, int argc, char** argv, char** azColName);
	static int scoreCallback(void* data, int argc, char** argv, char** azColName);
};
