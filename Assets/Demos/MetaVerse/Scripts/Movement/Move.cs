// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;

// public class PlayerMovementReplayer : MonoBehaviour
// {
//     public string filePath; // Chemin vers le fichier JSON
//     private List<PlayerData> replayData; // Liste des positions à rejouer
//     private int currentIndex = 0; // Index de la position actuelle
//     public float replayInterval = 0.5f; // Intervalle de temps entre chaque mouvement

//     private bool isReplaying = false;

//     void Start()
//     {
//         // Définir le chemin du fichier JSON
//         filePath = Path.Combine(Application.dataPath, "PlayerDataHistory.json");

//         // Charger les données du fichier JSON
//         LoadReplayData();

//         if (replayData != null && replayData.Count > 0)
//         {
//             Debug.Log("Data loaded successfully.");
//         }

//         //StartReplay();
//     }

//     void LoadReplayData()
//     {
//         try
//         {
//             if (File.Exists(filePath))
//             {
//                 string json = File.ReadAllText(filePath);
//                 PlayerDataList dataList = JsonUtility.FromJson<PlayerDataList>(json);
//                 replayData = dataList.Data;
//             }
//             else
//             {
//                 Debug.LogError($"File not found: {filePath}");
//             }
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError($"Error reading JSON: {ex.Message}");
//         }
//     }

//     public void StartReplay()
//     {
//         if (replayData != null && replayData.Count > 0)
//         {
//             isReplaying = true;
//             currentIndex = 0;
//             StartCoroutine(ReplayPositions());
//         }
//         else
//         {
//             Debug.LogWarning("No data to replay.");
//         }
//     }

//     IEnumerator ReplayPositions()
//     {
//         while (isReplaying && currentIndex < replayData.Count)
//         {
//             PlayerData currentData = replayData[currentIndex];

//             // Vérifiez si les données correspondent à Player1
//             if (currentData.PlayerID == "Player1")
//             {
//                 // Appliquer la position
                
//                 transform.position = new Vector3(currentData.Position.x, currentData.Position.y, currentData.Position.z);
//                 transform.rotation = new Quaternion(currentData.Rotation.x, currentData.Rotation.y, currentData.Rotation.z, currentData.Rotation.w);
//                 Debug.Log($"Moved to position: {transform.position}  {transform.rotation}");
//             }
//             yield return new WaitForSeconds(currentData.DeltaTime);
//             currentIndex++;
        
//         }

//         isReplaying = false;
//         Debug.Log("Replay finished.");
//     }
// }
