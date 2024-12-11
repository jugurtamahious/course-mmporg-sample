using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
  public List<CharacterScore> playersList; // List of players
  public List<TMP_Text> scoreDisplays; // UI Texts for scores
  // public UDPClient udpClient;

  public UDPClient udpClient;


  void Start()
  {
    // Find the UDPClient instance in the scene
    udpClient = FindObjectOfType<UDPClient>();
    if (udpClient == null)
    {
      Debug.LogError("UDPClient instance not found in the scene.");
    }
  }

  // Public method to update scores
  public void RefreshScores()
  {
    for (int i = 0; i < playersList.Count; i++)
    {
      if (i < scoreDisplays.Count && playersList[i] != null)
      {
        scoreDisplays[i].text = $"Player {i + 1}: {playersList[i].Score}";
      }
    }
  }

  // Method to update the list of players
  public void UpdatePlayerList()
  {
    if (udpClient == null)
    {
      udpClient = FindObjectOfType<UDPClient>();
      if (udpClient)
      {
        return;
      }
    }

    Dictionary<string, GameObject> players = udpClient.players;
    playersList.Clear();
    foreach (var player in players.Values)
    {
      // Find the child GameObject named "Score"
      Transform scoreTransform = player.transform.Find("Score");
      if (scoreTransform != null)
      {
        CharacterScore characterScore = scoreTransform.GetComponent<CharacterScore>();
        characterScore.enabled = true;
        if (characterScore != null)
        {
          playersList.Add(characterScore);
        }
      }
    }
    RefreshScores();

  }
}
