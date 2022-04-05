#pragma once

// Includes:
#include <string>

// Using:
using std::string;

// LoggedUser Class:
class LoggedUser
{
public:
	// C'tors & D'tors:
	LoggedUser(const string& username);
	~LoggedUser();

	// Getters:
	string getUsername() const;

	// Operators:
	friend bool operator==(const LoggedUser& a, const LoggedUser& b);
	friend bool operator<(const LoggedUser& a, const LoggedUser& b);

private:
	// Fields:
	string m_username;
};
