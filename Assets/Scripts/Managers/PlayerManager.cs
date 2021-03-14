using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class PlayerManager
{
    List<PlayerData> players;

    public Player[] PlayerPrefabs;

    public PlayerManager()
    {
        players = new List<PlayerData>();
    }

    
    public void AddPlayer(PlayerData data)
    {
        players.Add(data);
    }
    public void PlayerHit(Player player)
    {
        var data = GetPlayerData(player);
        data.Lives--;
    }
    public void AddScore(Player player, int score)
    {
        GetPlayerData(player).Score += score;
    }
    public Player SpawnPlayer()
    {
        var count = players.Count;
        if (count == 2)
        {
            return null;
        }

        var playerData = new PlayerData
        {
            Lives = GameDefaults.MaxLives,
            Name = "xxxx",
            SystemName = $"Player-{count+1}",
            Score = 0,
            playerObject = GameObject.Instantiate<Player>(PlayerPrefabs[count], GameDefaults.PlayerStartPositions[count], Quaternion.identity)
        };

        AddPlayer(playerData);
        return playerData.playerObject;
    }
    public void DeletePlayers()
    {
        foreach (var player in players)
        {
            GameObject.Destroy(player.playerObject.gameObject);
        }
        players.Clear();
    }
    public void RemovePlayer(Player player)
    {
        var pData = GetPlayerData(player);
        players.Remove(pData);
    }
    public int PlayersAlive()
    {
        return players.Count(p => p.Lives > 0);
    }
    public PlayerData GetPlayerData(Player player)
    {
        return players.Single(p => p.playerObject == player);
    }
    
}

[System.Serializable]
public class PlayerData
{
    public string SystemName { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public int Lives { get; set; }
    public Player playerObject { get; set; }

}
