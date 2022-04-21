#include "SqliteDataBase.h"
SqliteDataBase* SqliteDataBase::m_sqliteDataBaseInstance = nullptr;

// C'tor:

SqliteDataBase* SqliteDataBase::getInstance()
{
	if (m_sqliteDataBaseInstance == nullptr) {
		m_sqliteDataBaseInstance = new SqliteDataBase();
	}

	return m_sqliteDataBaseInstance;
}


// D'tor:

SqliteDataBase::~SqliteDataBase()
{
	delete m_sqliteDataBaseInstance;
}


// DB Functions:

/*
Opening the DB
Input : < None >
Output: true  - opening the DB was successfull
		false - otherwise
*/
bool SqliteDataBase::open()
{
	// Inits:
	string query = "CREATE TABLE USERS(username TEXT NOT NULL PRIMARY KEY, password TEXT NOT NULL, email TEXT NOT NULL);\n\
					CREATE TABLE STATISTICS(username TEXT NOT NULL PRIMARY KEY,\
					totalGames INTEGER NOT NULL, gamesWon INTEGER NOT NULL, gamesLost INTEGER NOT NULL, gamesTied INTEGER NOT NULL, elo INTEGER NOT NULL);";
	string dbFile = "ChessDB.db";
	char* errMsg = NULL;
	int res = _access(dbFile.c_str(), 0);

	// Condition: opening the DB failed
	if (sqlite3_open(dbFile.c_str(), &m_db) != SQLITE_OK)
	{
		m_db = nullptr;
		std::cerr << "DB Connection Failed!\n";
		return false;
	}

	// Condition: DB file doesn't exist
	if (res == DB_DOESNT_EXIST && sqlite3_exec(m_db, query.c_str(), nullptr, nullptr, &errMsg))
	{
		std::cerr << "SQL ERROR: " << errMsg << "\n";
		close();
		return false;
	}

	// Condition: DB opened successfully
	return true;
}

/*
Closing the DB
Input : < None >
Output: < None >
*/
void SqliteDataBase::close()
{
	// Closing the DB:
	if (m_db != nullptr)
	{
		sqlite3_close(m_db);
		m_db = nullptr;
	}
}


/* Users Table */

// Queries:

/*
Checking whether a user exists
Input : username - the username
Output: found	 - whether the user exists
*/
bool SqliteDataBase::doesUserExist(const string& username)
{
	// Inits:
	string query = "SELECT * FROM USERS WHERE username='" + username + "';";
	char* errMsg = NULL;
	bool found = false;

	// Checking whether the user exists:
	if (sqlite3_exec(m_db, query.c_str(), outputExistsCallback, &found, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Returning the result:
	return found;
}

/*
Checking whether a password matches to the username
Input : username - the username
		password - the password
Output: found	 - whether the password matches to the username
*/
bool SqliteDataBase::doesPasswordMatch(const string& username, const string& password)
{
	// Inits:
	string query = "SELECT password FROM USERS WHERE username='" + username + "';";
	char* errMsg = NULL;
	string dbPassword;

	// Checking whether the password matches to the username:
	if (sqlite3_exec(m_db, query.c_str(), getPasswordCallback, &dbPassword, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Condition: matching passwords
	if (RSA::decrypt(dbPassword) == password) {
		return true;
	}

	// Condition: password doesn't match
	return false;
}

// Actions:

/*
Adding a new user to the DB
Input : username - the username
		password - the password
		email	 - the email
Output: < None >
*/
void SqliteDataBase::addNewUser(const string& username, const string& password, const string& email)
{
	// Inits:
	string usersQuery = "INSERT INTO USERS VALUES ('" + username + "', '" + RSA::encrypt(password) + "', '" + email + "');";
	string statsQuery = "INSERT INTO STATISTICS VALUES ('" + username + "', 0, 0, 0, 0, 1000);";
	char* errMsg = NULL;

	// Adding a new user to the DB:
	if (sqlite3_exec(m_db, usersQuery.c_str(), nullptr, nullptr, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Adding new stats to the DB stats:
	if (sqlite3_exec(m_db, statsQuery.c_str(), nullptr, nullptr, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}
}


/* Statistics Table */

// Queries:

/*
Getting a player's number of games won
Input : username - the player's username
Output: avgTime  - the player's number of games won
*/
int SqliteDataBase::getNumOfPlayerWins(const string& username)
{
	// Inits:
	string query = "SELECT gamesWon FROM STATISTICS WHERE username='" + username + "';";
	char* errMsg = NULL;
	int totalGames = 0;

	// Getting the player's number of total games won:
	if (sqlite3_exec(m_db, query.c_str(), intNumCallback, &totalGames, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Returning the result:
	return totalGames;
}

/*
Getting a player's number of games lost
Input : username - the player's username
Output: avgTime  - the player's number of games lost
*/
int SqliteDataBase::getNumOfPlayerLosses(const string& username)
{
	// Inits:
	string query = "SELECT gamesLost FROM STATISTICS WHERE username='" + username + "';";
	char* errMsg = NULL;
	int totalGames = 0;

	// Getting the player's number of total games lost:
	if (sqlite3_exec(m_db, query.c_str(), intNumCallback, &totalGames, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Returning the result:
	return totalGames;
}

/*
Getting a player's number of games tied
Input : username - the player's username
Output: avgTime  - the player's number of games tied
*/
int SqliteDataBase::getNumOfPlayerTies(const string& username)
{
	// Inits:
	string query = "SELECT gamesTied FROM STATISTICS WHERE username='" + username + "';";
	char* errMsg = NULL;
	int totalGames = 0;

	// Getting the player's number of total games tied:
	if (sqlite3_exec(m_db, query.c_str(), intNumCallback, &totalGames, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Returning the result:
	return totalGames;
}

/*
Getting a player's number of games
Input : username - the player's username
Output: avgTime  - the player's number of games
*/
int SqliteDataBase::getNumOfPlayerGames(const string& username)
{
	// Inits:
	string query = "SELECT totalGames FROM STATISTICS WHERE username='" + username + "';";
	char* errMsg = NULL;
	int totalGames = 0;

	// Getting the player's number of games:
	if (sqlite3_exec(m_db, query.c_str(), intNumCallback, &totalGames, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Returning the result:
	return totalGames;
}

/*
Getting a player's ELO
Input : username - the player's username
Output: avgTime  - the player's ELO
*/
int SqliteDataBase::getPlayerElo(const string& username)
{
	// Inits:
	string query = "SELECT elo FROM STATISTICS WHERE username='" + username + "';";
	char* errMsg = NULL;
	int elo = 0;

	// Getting the player's ELO:
	if (sqlite3_exec(m_db, query.c_str(), intNumCallback, &elo, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Returning the result:
	return elo;
}

/*
Getting the highscores
Input : < None >
Output: topScores - the highscores
*/
vector<string> SqliteDataBase::getHighScores()
{
	// Inits:
	string query = "SELECT username, gamesWon FROM STATISTICS ORDER BY gamesWon DESC LIMIT 5;";
	char* errMsg = NULL;
	vector<string> topScores;

	// Getting the highscores:
	if (sqlite3_exec(m_db, query.c_str(), scoreCallback, &topScores, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Returning the result:
	return topScores;
}

/*
Adding stats
Input : < None >
Output: username	   - the username
		averageTime	   - the avg time
		correctAnswers - amount of correct answers
		totalAnswers   - total answers count
*/
void SqliteDataBase::addStatistics(const string& username, int gameStatus)
{
	// Inits:
	string query = "SELECT username FROM STATISTICS WHERE username='" + username + "';";
	char* errMsg = NULL;
	bool doesUsernameExist = false;
	int originalWonGames = getNumOfPlayerWins(username);
	int originalLostGames = getNumOfPlayerLosses(username);
	int originalTiedGames = getNumOfPlayerTies(username);
	int originalTotalGames = getNumOfPlayerGames(username);
	int originalElo = getPlayerElo(username);

	// Getting whether the user exists:
	if (sqlite3_exec(m_db, query.c_str(), outputExistsCallback, &doesUsernameExist, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Updating:
	// TODO: CHANGE ELO GAIN
	switch (gameStatus)
	{
		case WON_GAME: originalWonGames++; originalElo += 25;  break;
		case LOST_GAME: originalLostGames++; originalElo -= 25; break;
		case TIED_GAME: originalTiedGames++; break;
		default: break;
	}
	originalTotalGames++;

	// Updating the current stats:
	query = "UPDATE STATISTICS SET totalGames=" + std::to_string(originalTotalGames) + ", gamesWon=" + std::to_string(originalWonGames) +
		", gamesLost=" + std::to_string(originalLostGames) + ", gamesTied=" + std::to_string(originalTiedGames) + ", elo=" + std::to_string(originalElo) + 
		" WHERE username='" + username + "';";
	
	if (sqlite3_exec(m_db, query.c_str(), nullptr, nullptr, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}
}


// Callback Functions:

/*
Callback for checking whether an item exists in the DB
*/
int SqliteDataBase::outputExistsCallback(void* data, int argc, char** argv, char** azColName)
{
	// Inits:
	bool* output = static_cast<bool*>(data);

	*output = true;

	return 0;
}

/*
Callback to get integer value from STATISTICS table
*/
int SqliteDataBase::intNumCallback(void* data, int argc, char** argv, char** azColName)
{
	// Inits:
	int* num = static_cast<int*>(data);

	if (argc > 0) {
		*num = std::stoi(argv[0]);
	}

	return 0;
}

/*
Callback to add score to scores vector from STATISTICS table
*/
int SqliteDataBase::scoreCallback(void* data, int argc, char** argv, char** azColName)
{
	// Inits:
	vector<string>* score = static_cast<vector<string>*>(data);

	// Adding username with score:
	if (argc > 1) {
		score->push_back(std::string(argv[0]) + " : " + std::string(argv[1]));
	}

	return 0;
}

/*
Callback to get password from USERS table
*/
int SqliteDataBase::getPasswordCallback(void* data, int argc, char** argv, char** azColName)
{
	// Inits:
	string* password = static_cast<string*>(data);

	if (argc > 0) {
		*password = std::string(argv[0]);
	}

	return 0;
}
