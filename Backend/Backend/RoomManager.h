#pragma once

// Includes:
#include "Room.h"
#include <map>

// Using:
using std::map;

// RoomManager Class:
class RoomManager
{
public:
	// Static C'tor:
	static RoomManager* getInstance();

	// D'tor:
	~RoomManager();

	// Methods:
	void createRoom(LoggedUser user, RoomData data);
	void deleteRoom(int ID);
	unsigned int getRoomState(int ID) const;
	vector<RoomData> getRooms() const;
	Room* getRoom(int ID);
	RoomData getEloRoom() const;

private:
	// Private C'tor:
	RoomManager() = default;

	// Fields:
	static RoomManager* m_roomManagerInstance;
	map<unsigned int, Room> m_rooms;
};
