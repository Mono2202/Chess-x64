#pragma once

// Includes:
#include "Room.h"
#include "IDatabase.h"
#include <map>

// Using:
using std::map;

// RoomManager Class:
class RoomManager
{
public:
	// Static C'tor:
	static RoomManager* getInstance(IDatabase* database);

	// D'tor:
	~RoomManager();

	// Methods:
	void createRoom(LoggedUser user, RoomData data);
	void deleteRoom(int ID);
	unsigned int getRoomState(int ID) const;
	vector<RoomData> getRooms() const;
	Room* getRoom(int ID);
	RoomData getEloRoom() const; // TODO: GET PLAYER'S ELO TO GET A CORRECT ROOM
	RoomData getPrivateRoom(const string& roomCode) const;

private:
	// Private C'tor:
	RoomManager(IDatabase* database);

	// Fields:
	static RoomManager* m_roomManagerInstance;
	map<unsigned int, Room> m_rooms;
	IDatabase* m_database;
};
