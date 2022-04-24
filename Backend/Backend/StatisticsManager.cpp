#include "StatisticsManager.h"
StatisticsManager* StatisticsManager::m_statisticsManagerInstance = nullptr;

// C'tors:

StatisticsManager::StatisticsManager(IDatabase* database) : m_database(database) {	}

StatisticsManager* StatisticsManager::getInstance(IDatabase* database)
{
	if (m_statisticsManagerInstance == nullptr) {
		m_statisticsManagerInstance = new StatisticsManager(database);
	}

	return m_statisticsManagerInstance;
}


// D'tor:

StatisticsManager::~StatisticsManager()
{
	delete m_statisticsManagerInstance;
}


// Methods:

/*
Getting the user statistics
Input : username - the user's username
Output: stats	 - the user's statistics
*/
vector<string> StatisticsManager::getUserStatistics(const string& username)
{
	// Condition: user doesn't exist
	if (!m_database->doesUserExist(username)) {
		throw std::exception("User doesn't exist\n");
	}

	// Inits:
	vector<string> stats;

	// Building the user's statistics vector
	stats.push_back(std::to_string(m_database->getNumOfPlayerGames(username)));
	stats.push_back(std::to_string(m_database->getNumOfPlayerWins(username)));
	stats.push_back(std::to_string(m_database->getNumOfPlayerLosses(username)));
	stats.push_back(std::to_string(m_database->getNumOfPlayerTies(username)));
	stats.push_back(std::to_string(m_database->getPlayerElo(username)));
	return stats;
}

/*
Adding stats
Input : username   - the username
		gameStatus - whether won / lost / tied
Output: < None >
*/
void StatisticsManager::addUserStatistics(const string& username, int gameStatus)
{
	m_database->addStatistics(username, gameStatus);
}

/*
Getting the user games
Input : username - the user's username
Output: games	 - the user's games
*/
vector<string> StatisticsManager::getGames(const string& username)
{
	// Condition: user doesn't exist
	if (!m_database->doesUserExist(username)) {
		throw std::exception("User doesn't exist\n");
	}

	return m_database->getGames(username);
}

/*
Adding a game to the DB
Input : whiteUsername - the white username
		blackUsername - the black username
		game		  - the game string
		wonUsername   - the username of the winning player
		date		  - the date the game took place
Output: < None >
*/
void StatisticsManager::addGame(const string& whiteUsername, const string& blackUsername, const string& game, const string& wonUsername, const string& date)
{
	// Adding the game to the DB:
	m_database->addGame(whiteUsername, blackUsername, game, wonUsername, date);
}

/*
Getting the highscores
Input : < None >
Output: the highscores
*/
vector<string> StatisticsManager::getHighScore()
{
	return m_database->getHighScores();
}
