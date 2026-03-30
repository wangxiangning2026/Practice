using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LoginRequest
{
    public string playerId;
    public string deviceId;
}

[Serializable]
public class LoginResponse
{
    public bool success;
    public PlayerData data;
    public bool isNew;
    public string message;
}

[Serializable]
public class PlayerData
{
    public string playerId;
    public string name;
    public int score;
    public int level;
}

[Serializable]
public class ScoreRequest
{
    public string playerId;
    public int score;
}

[Serializable]
public class ScoreResponse
{
    public bool success;
    public int rank;
    public string message;
}

[Serializable]
public class LeaderboardResponse
{
    public bool success;
    public LeaderboardEntry[] data;
}

[Serializable]
public class LeaderboardEntry
{
    public int rank;
    public string name;
    public int score;
}

[Serializable]
public class HeartbeatRequest
{
    public string playerId;
    public long timestamp;
}
