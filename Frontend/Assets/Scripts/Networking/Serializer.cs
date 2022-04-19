using Newtonsoft.Json;

// LoginRequest Class:
class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

// SignupRequest Class:
class SignupRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}

// LogoutRequest Class:
class LogoutRequest { }

// GetRoomsRequest Class:
class GetRoomsRequest { }

// GetPlayersInRoomRequest Class:
class GetPlayersInRoomRequest
{
    public int RoomID { get; set; }
}

// GetHighScoreRequest Class:
class GetHighScoreRequest { }

// GetPersonalStatsRequest Class:
class GetPersonalStatsRequest { }

// JoinRoomRequest Class:
class JoinRoomRequest
{
    public int RoomID { get; set; }
}

// CreateRoomRequest Class:
class CreateRoomRequest
{
    public string GameMode { get; set; }
}

// GetRoomStateRequest Class:
class SearchEloRoomRequest { }

// GetRoomStateRequest Class:
class GetRoomStateRequest { }

// LeaveRoomRequest Class:
class LeaveRoomRequest { }

// SubmitMoveRequest Class:
class SubmitMoveRequest 
{ 
    public string Move { get; set; }
}

static class Serializer
{
    // Constants:
    public const int LOGIN_REQUEST = 101;
    public const int SIGNUP_REQUEST = 102;
    public const int LOGOUT_REQUEST = 103;
    public const int GET_ROOMS_REQUEST = 104;
    public const int GET_PLAYERS_IN_ROOM_REQUEST = 105;
    public const int GET_HIGH_SCORE_REQUEST = 106;
    public const int GET_PERSONAL_STATS_REQUEST = 107;
    public const int JOIN_ROOM_REQUEST = 108;
    public const int CREATE_ROOM_REQUEST = 109;
    public const int SEARCH_ELO_ROOM_REQUEST = 110;
    public const int GET_ROOM_STATE_REQUEST = 111;
    public const int LEAVE_ROOM_REQUEST = 112;
    public const int SUBMIT_MOVE_REQUEST = 113;


    // Methods:

    public static string SerializeRequest<T>(T request, int requestCode)
    {
        // Returning the buffer:
        return (char)requestCode + JsonConvert.SerializeObject(request);
    }
}