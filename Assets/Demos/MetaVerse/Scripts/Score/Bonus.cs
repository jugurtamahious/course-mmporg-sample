using System.Net.Sockets;
using UnityEngine;

public class Bonus : MonoBehaviour
{
  public LayerMask CollisionLayers;
  public int Points = 1;

  public UDPServer server;
  public UDPClient client;

  public bool Car = false;

  private string donutID;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    donutID = gameObject.name;

    client.OnDonutUpdate += UpdateDonut;
  }

  // Update is called once per frame
  void Update()
  {
    if (!Car && Globals.IsServer)
    {
      SendDonutUpdate(donutID, true);
    }
  }

  public void SendDonutUpdate(string donutID, bool isEaten)
  {
    DonutUpdate donutUpdate = new DonutUpdate
    {
      donutID = donutID,
      isEaten = true
    };


    string json = JsonUtility.ToJson(donutUpdate);

    server.BroadcastDonutState(json);
  }

  public void UpdateDonut(string donutID, bool isEaten)
  {
    if (donutID == this.donutID && isEaten)
    {
      Destroy(gameObject);
    }

  }

  private bool ShouldHandleObject(Collider other)
  {
    return (CollisionLayers.value & (1 << other.gameObject.layer)) > 0;
  }

  void OnTriggerEnter(Collider other)
  {
    if (!ShouldHandleObject(other)) { return; }

    CharacterScore cScore = other.gameObject.GetComponentInChildren<CharacterScore>();
    if (cScore != null)
    {
      cScore.AddScore(Points);
    }

    if (Car != true)
    {
      SendDonutUpdate(donutID, false);
      Destroy(gameObject);
    }
  }
}
