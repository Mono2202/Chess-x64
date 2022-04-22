using Newtonsoft.Json;

// ErrorResponse Class:
class ErrorResponse
{
    public int Status { get; set; }
    public string Message { get; set; }
}

// LoginResponse Class:
class LoginResponse
{
    public int Status { get; set; }
}

// SignupResponse Class:
class SignupResponse
{
    public int Status { get; set; }
}

// LogoutResponse Class:
class LogoutResponse
{
    public int Status { get; set; }
}

// GetRoomsResponse Class:
class GetRoomsResponse
{
    public int Status { get; set; }
    public string Rooms { get; set; }
}

// GetPlayersInRoomResponse Class:
class GetPlayersInRoomResponse
{
    public int Status { get; set; }
    public string PlayersInRoom { get; set; }
}

// GetHighScoreResponse Class:
class GetHighScoreResponse
{
    public int Status { get; set; }
    public string HighScores { get; set; }
}

// GetPersonalStatsResponse Class:
class GetPersonalStatsResponse
{
    public int Status { get; set; }
    public string Statistics { get; set; }
}

// JoinRoomResponse Class:
class JoinRoomResponse
{
    public int Status { get; set; }
}

// CreateRoomResponse Class:
class CreateRoomResponse
{
    public int Status { get; set; }
    public int RoomID { get; set; }
}

// SearchEloRoomResponse Class:
class SearchEloRoomResponse
{
    public int Status { get; set; }
    public int RoomID { get; set; }
}

// GetRoomStateResponse Class:
class GetRoomStateResponse
{
    public int Status { get; set; }
    public bool IsActive { get; set; }
    public string Players { get; set; }
    public string CurrentMove { get; set; }
    public string GameMode { get; set; }
}

// LeaveRoomResponse Class:
class LeaveRoomResponse
{
    public int Status { get; set; }
}

// SubmitMoveResponse Class:
class SubmitMoveResponse
{
    public int Status { get; set; }
}

// SearchPrivateRoomResponse Class:
class SearchPrivateRoomResponse
{
    public int Status { get; set; }
    public int RoomID { get; set; }
}

// GetMatchHistoryResponse Class:
class GetMatchHistoryResponse
{
    public int Status { get; set; }
    public string Games { get; set; }
}

class Deserializer
{
    // Constants:
    public const int ERROR_RESPONSE = 2;

    // Methods:
    public static T DeserializeResponse<T>(string buffer)
    {
        // Building the response:
        return JsonConvert.DeserializeObject<T>(buffer.Substring(1));
    }
}