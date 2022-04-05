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
	vector<string> getHighScore();

private:
	// Private C'tor:
	StatisticsManager(IDatabase* database);

	// Fields:
	static StatisticsManager* m_statisticsManagerInstance;
	IDatabase* m_database;
};
