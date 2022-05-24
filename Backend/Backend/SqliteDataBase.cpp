#include "SqliteDatabase.h"
SqliteDatabase* SqliteDatabase::m_SqliteDatabaseInstance = nullptr;

// C'tor:

SqliteDatabase* SqliteDatabase::getInstance()
{
	if (m_SqliteDatabaseInstance == nullptr) {
		m_SqliteDatabaseInstance = new SqliteDatabase();
	}

	return m_SqliteDatabaseInstance;
}


// D'tor:

SqliteDatabase::~SqliteDatabase()
{
	delete m_SqliteDatabaseInstance;
}


// DB Functions:

/*
Opening the DB
Input : < None >
Output: true  - opening the DB was successfull
		false - otherwise
*/
bool SqliteDatabase::open()
{
	// Inits:
	string query = "CREATE TABLE USERS(username TEXT NOT NULL PRIMARY KEY, password TEXT NOT NULL, email TEXT NOT NULL);\n\
					CREATE TABLE STATISTICS(username TEXT NOT NULL PRIMARY KEY,\
					totalGames INTEGER NOT NULL, gamesWon INTEGER NOT NULL, gamesLost INTEGER NOT NULL, gamesTied INTEGER NOT NULL, elo INTEGER NOT NULL);\n\
					CREATE TABLE GAMES(whiteUsername TEXT NOT NULL, blackUsername TEXT NOT NULL, game TEXT NOT NULL, wonUsername TEXT NOT NULL,\
					date TEXT NOT NULL);";
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
void SqliteDatabase::close()
{
	// Closing the DB:
	if (m_db != nullptr)
	{
		sqlite3_close(m_db);
		m_db = nullptr;
	}
}


// Users Table:

/*
Checking whether a user exists
Input : username - the username
Output: found	 - whether the user exists
*/
bool SqliteDatabase::doesUserExist(const string& username)
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
bool SqliteDatabase::doesPasswordMatch(const string& username, const string& password)
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

/*
Adding a new user to the DB
Input : username - the username
		password - the password
		email	 - the email
Output: < None >
*/
void SqliteDatabase::addNewUser(const string& username, const string& password, const string& email)
{
	// Inits:
	string usersQuery = "INSERT INTO USERS VALUES ('" + username + "', '" + RSA::encrypt(password) + "', '" + RSA::encrypt(email) + "');";
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


// Statistics Table:

/*
Getting a player's number of games won
Input : username - the player's username
Output: avgTime  - the player's number of games won
*/
int SqliteDatabase::getNumOfPlayerWins(const string& username)
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
int SqliteDatabase::getNumOfPlayerLosses(const string& username)
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
int SqliteDatabase::getNumOfPlayerTies(const string& username)
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
int SqliteDatabase::getNumOfPlayerGames(const string& username)
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
int SqliteDatabase::getPlayerElo(const string& username)
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
vector<string> SqliteDatabase::getHighScores()
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
Input : username   - the username
		gameStatus - whether won / lost / tied
Output: < None >
*/
void SqliteDatabase::addStatistics(const string& username, int gameStatus)
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

/*
Adding a game to the DB
Input : whiteUsername - the white username
		blackUsername - the black username
		game		  - the game string
		wonUsername   - the username of the winning player
		date		  - the date the game took place
Output: < None >
*/
void SqliteDatabase::addGame(const string& whiteUsername, const string& blackUsername, const string& game,
	const string& wonUsername, const string& date)
{
	// Inits
	string query = "INSERT INTO GAMES VALUES('" + whiteUsername + "', '" + blackUsername + "', '" + game + "', '" + wonUsername +
		"', '" + date + "');";
	char* errMsg = NULL;

	// Adding the game:
	if (sqlite3_exec(m_db, query.c_str(), nullptr, nullptr, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}
}

/*
Getting user's games
Input : username - the username
Output: 
*/
vector<string> SqliteDatabase::getGames(const string& username)
{
	// Inits:
	string query = "SELECT * FROM GAMES WHERE whiteUsername='" + username + "' OR blackUsername='" + username + "';";
	char* errMsg = NULL;
	vector<string> games;

	// Getting the player's games:
	if (sqlite3_exec(m_db, query.c_str(), gamesCallback, &games, &errMsg)) {
		std::cerr << "SQL ERROR: " << errMsg << "\n";
	}

	// Returning the result:
	return games;
}


// Callback Functions:

/*
Callback for checking whether an item exists in the DB
*/
int SqliteDatabase::outputExistsCallback(void* data, int argc, char** argv, char** azColName)
{
	// Inits:
	bool* output = static_cast<bool*>(data);

	*output = true;

	return 0;
}

/*
Callback to get integer value from STATISTICS table
*/
int SqliteDatabase::intNumCallback(void* data, int argc, char** argv, char** azColName)
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
int SqliteDatabase::scoreCallback(void* data, int argc, char** argv, char** azColName)
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
int SqliteDatabase::getPasswordCallback(void* data, int argc, char** argv, char** azColName)
{
	// Inits:
	string* password = static_cast<string*>(data);

	if (argc > 0) {
		*password = std::string(argv[0]);
	}

	return 0;
}

/*
Callback to get games from GAMES table
*/
int SqliteDatabase::gamesCallback(void* data, int argc, char** argv, char** azColName)
{
	// Inits:
	vector<string>* games = static_cast<vector<string>*>(data);

	// Adding current game:
	if (argc > 0) {
		games->push_back(std::string(argv[0]) + "&&&" + std::string(argv[1]) + "&&&" + std::string(argv[3]) +
			"&&&" + std::string(argv[4]));
	}

	return 0;
}
