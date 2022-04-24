#pragma once

// Includes:
#include "IDatabase.h"

// StatisticsManager Class:
class StatisticsManager
{
public:
	// Static C'tor:
	static StatisticsManager* getInstance(IDatabase* database);

	// D'tor:
	~StatisticsManager();

	// Methods:
	vector<string> getUserStatistics(const string& username);
	void addUserStatistics(const string& username, int gameStatus);
	vector<string> getGames(const string& username);
	void addGame(const string& whiteUsername, const string& blackUsername, const string& game,
		const string& wonUsername, const string& date);
	vector<string> getHighScore();

private:
	// Private C'tor:
	StatisticsManager(IDatabase* database);

	// Fields:
	static StatisticsManager* m_statisticsManagerInstance;
	IDatabase* m_database;
};
