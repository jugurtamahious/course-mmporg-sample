using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab; // Préfabs des joueurs
    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    // public void SpawnPlayer(int playerId, bool isLocalPlayer, Vector3 startPosition)
    // {
    //     GameObject newPlayer = Instantiate(playerPrefab, startPosition, Quaternion.identity);
        
    //     if (isLocalPlayer)
    //     {
    //         newPlayer.GetComponent<PlayerController>().enabled = true;
    //     }
    //     else
    //     {
    //         newPlayer.GetComponent<PlayerController>().enabled = false;
    //     }

    //     players.Add(playerId, newPlayer);
    // }

    public void UpdatePlayerPosition(int playerId, Vector3 newPos)
    {
        if (players.ContainsKey(playerId))
        {
            //players[playerId].GetComponent<PlayerController>().UpdatePositionFromServer(newPos);
        }
    }

    public void RemovePlayer(int playerId)
    {
        if (players.ContainsKey(playerId))
        {
            Destroy(players[playerId]);
            players.Remove(playerId);
        }
    }
}
