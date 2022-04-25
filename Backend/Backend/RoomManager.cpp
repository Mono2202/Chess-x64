#include "RoomManager.h"
#include <iostream>
RoomManager* RoomManager::m_roomManagerInstance = nullptr;

// C'tor:

RoomManager::RoomManager(IDatabase* database)
{
	// Setting the database:
	m_database = database;
}

RoomManager* RoomManager::getInstance(IDatabase* database)
{
	if (m_roomManagerInstance == nullptr) {
		m_roomManagerInstance = new RoomManager(database);
	}
	
	return m_roomManagerInstance;
}


// D'tor:

RoomManager::~RoomManager()
{
	delete m_roomManagerInstance;
}


// Methods:

/*
Creates a room
Input : user - the user
		data - the room's data
Output: < None >
*/
void RoomManager::createRoom(LoggedUser user, RoomData data)
{
	// Condition: room already exists
	for (auto const& it : m_rooms) {
		if (it.second.getRoomData().id == data.id) {
			throw std::exception("Room already exists\n");
		}
	}

	// Creates the room:
	Room newRoom(data);

	// Adding the user to the room:
	newRoom.addUser(user);

	// Inserting the room to the room map:
	m_rooms.emplace(data.id, newRoom);
}

/*
Deletes a room
Input : ID - the room's ID
Output: < None >
*/
void RoomManager::deleteRoom(int ID)
{
	// Deletes the room from the room map:
	m_rooms.erase(ID);
}

/*
Getting a room's state
Input : ID - the room's ID
Output: the room's state
*/
unsigned int RoomManager::getRoomState(int ID) const
{
	// Returning the room's state:
	return m_rooms.at(ID).getRoomData().isActive;
}

/*
Getting the rooms vector
Input : < None >
Output: roomsData - the rooms vector
*/
vector<RoomData> RoomManager::getRooms() const
{
	// Inits:
	vector<RoomData> roomsData;

	// Getting the rooms' data:
	for (auto const& it : m_rooms) {
		if (!it.second.getRoomData().isActive) {
			roomsData.push_back(it.second.getRoomData());
		}
	}

	// Returning the rooms vector:
	return roomsData;
}

/*
Getting a specific room by ID
Input : ID - the room's ID
Output: the room
*/
Room* RoomManager::getRoom(int ID)
{
	if (m_rooms.find(ID) == m_rooms.end())
		throw std::exception("Room not found\n");
	return &m_rooms.at(ID);
}

/*
Getting a room by ELO points
Input : < None >
Output: the room's data
*/
RoomData RoomManager::getEloRoom() const
{
	// Inits:
	RoomData currentRoomData = { 0, false, "", "F"};

	// Going through the rooms:
	for (auto& room : m_rooms) {
		// Condition: game found by ELO points
		if (room.second.getRoomData().gameMode[0] == ELO_GAME_MODE &&
			!room.second.getRoomData().isActive &&
			room.second.getRoomData().currentMove != "OPPONENT LEFT") {
			return room.second.getRoomData();
		}
	}
	// TODO: ADD ELO CHECK

	// Condition: no game was found
	return currentRoomData;
}

/*
Getting a private room
Input : roomCode - the room code
Output: the room's data
*/
RoomData RoomManager::getPrivateRoom(const string& roomCode) const
{
	// Inits:
	RoomData currentRoomData = { 0, false, "", "F" };

	// Going through the rooms:
	for (auto& room : m_rooms) {
		// Condition: game found by room code
		if (room.second.getRoomData().gameMode[0] == PRIVATE_GAME_MODE &&
			!room.second.getRoomData().isActive &&
			std::to_string(room.second.getRoomData().id) == roomCode) {
			return room.second.getRoomData();
		}
	}

	// Condition: no game was found
	return currentRoomData;
}
