using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
  public List<CharacterScore> players; // List of players
  public List<TMP_Text> scoreDisplays; // UI Texts for scores

  // Public method to update scores
  public void RefreshScores()
  {
    for (int i = 0; i < players.Count; i++)
    {
      if (i < scoreDisplays.Count && players[i] != null)
      {
        scoreDisplays[i].text = $"Player {i + 1}: {players[i].Score}";
      }
    }
  }
}
