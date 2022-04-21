#include "Room.h"

// C'tors:

Room::Room(const RoomData& data) : m_metadata(data) {	}


// Methods:

void Room::setIsActive(unsigned int flag)
{
	this->m_metadata.isActive = flag;
}

bool Room::getIsActive() const
{
	return this->m_metadata.isActive;
}

/*
Setting the current move
Input : move - the move
Output: < None >
*/
void Room::setCurrentMove(string move)
{
	// Condition: choosing players
	if (move == "random") {
		// Inits:
		vector<string> users = this->getAllUsers();
		int i = 0;

		// Shuffling the users vector: TODO: CHECK IF ACTUALLY RANDOM
		std::random_shuffle(users.begin(), users.end());

		// Choosing the starting color for each player
		this->m_metadata.currentMove = users[0] + "&&&W&&&" + users[1] + "&&&B";	
	}

	else {
		this->m_metadata.currentMove = move;
	}
}

/*
Getting the current move
Input : move - the move
Output: < None >
*/
string Room::getCurrentMove() const
{
	return this->m_metadata.currentMove;
}

/*
Adds a user to the users vector
Input : user - the user
Output: < None >
*/
void Room::addUser(LoggedUser user)
{
	// Condition: full capacity room
	if (m_users.size() >= MAX_PLAYERS) {
		throw std::exception("Room is at full capacity\n");
	}

	// Inits:
	vector<LoggedUser>::iterator userFound = std::find(m_users.begin(), m_users.end(), user);

	// Condition: user doesn't exist
	if (userFound == m_users.end()) {
		// Adds the user:
		m_users.push_back(user);
	}

	else {
		throw std::exception("User already in room\n");
	}
}

/*
Removes a user from the users vector
Input : user - the user
Output: < None >
*/
void Room::removeUser(LoggedUser user)
{
	// Inits:
	vector<LoggedUser>::iterator userFound = std::find(m_users.begin(), m_users.end(), user);

	// Condition: user exists
	if (userFound != m_users.end()) {
		// Removing the user:
		m_users.erase(userFound);
	}
}

/*
Returns the usernames vector
Input : < None >
Output: usernames - the users list
*/
vector<string> Room::getAllUsers() const
{
	// Inits:
	vector<string> usernames;

	// Getting all of the usernames:
	for (int i = 0; i < m_users.size(); i++) {
		usernames.push_back(m_users[i].getUsername());
	}

	// Returning the usernames vector:
	return usernames;
}

/*
Returning the room's data
Input : < None >
Output: m_metadata - the room's data
*/
RoomData Room::getRoomData() const
{
	// Returning the room's data:
	return m_metadata;
}
