#pragma once

// Includes:
#include "IDatabase.h"
#include "LoggedUser.h"
#include <vector>
#include <regex>

// Using:
using std::vector;

// LoginManager Class:
class LoginManager
{
public:
	// Static C'tor:
	static LoginManager* getInstance(IDatabase* database);

	// D'tor:
	~LoginManager();

	// Methods:
	void signup(const string& username, const string& password, const string& email) const;
	void login(const string& username, const string& password);
	void logout(const string& username);

private:
	// Private C'tor:
	LoginManager(IDatabase* database);

	// Fields:
	static LoginManager* m_loginManagerInstace;
	IDatabase* m_database;
	vector<LoggedUser> m_loggedUsers;
};
