using UnityEngine;
using UnityEngine.Networking;

public class Planet : MonoBehaviour {

  public GField gfield { get; private set; }
  public TerrainBase terrain { get; private set; }

  void Awake() {
    gfield = GetComponent<GField>();
    terrain = GetComponent<TerrainBase>();
  }

  void Start() {
    terrain.Generate();
  }

}
