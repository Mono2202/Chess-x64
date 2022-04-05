#include "LoggedUser.h"

// C'tors:

LoggedUser::LoggedUser(const string& username) : m_username(username) {   }


// D'tors:

LoggedUser::~LoggedUser() { m_username = ""; }


// Getters:

string LoggedUser::getUsername() const { return m_username; }


// Operators:

bool operator==(const LoggedUser& a, const LoggedUser& b) { return a.m_username == b.m_username; }

bool operator<(const LoggedUser& a, const LoggedUser& b) { return a.m_username.compare(b.m_username) < 0; }
