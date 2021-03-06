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
class GetPersonalStatsRequest 
{
    public string Username { get; set; }
}

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

// SearchPrivateRoomRequest Class:
class SearchPrivateRoomRequest
{
    public string RoomCode { get; set; }
}

// GetMatchHistoryRequest Class:
class GetMatchHistoryRequest
{
    public string Username { get; set; }
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
    public const int SEARCH_PRIVATE_ROOM_REQUEST = 114;
    public const int GET_MATCH_HISTORY_REQUEST = 115;

    /*
     * Turning a Request class to a message
     * Input : request     - the Request class
     *         requestCode - the Request code
     * Output: the message to send to the server
     */
    public static string SerializeRequest<T>(T request, int requestCode)
    {
        return (char)requestCode + JsonConvert.SerializeObject(request);
    }
}