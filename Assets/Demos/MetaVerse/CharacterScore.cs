using UnityEngine;

public class CharacterScore : MonoBehaviour
{
  public int Score = 0;
  public TMPro.TMP_Text TxtScore; // Optional player-specific UI

  private ScoreManager scoreManager;

  void Start()
  {
    scoreManager = FindObjectOfType<ScoreManager>();
    if (TxtScore != null)
    {
      TxtScore.text = Score.ToString();
    }
  }

  public void AddScore(int points)
  {
    Score += points;
    if (TxtScore != null)
    {
      TxtScore.text = Score.ToString(); // Update local UI
    }

    // Notify ScoreManager to refresh the central UI
    scoreManager?.RefreshScores();
  }
}
