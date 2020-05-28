using UnityEngine;
using UnityEngine.Networking;

public class SpawnShipAtRClick : KittyNetworkBehaviour {

  public bool spawned = false;
  public Ship prefab;
  public float spawnHeight = 1;

  public Player player { get; private set; }

  protected override void Awake() {
    base.Awake();
    player = GetComponent<Player>();
  }

  void Update() {
    if (isLocalPlayer) {
      if ((Input.GetMouseButtonDown((int) MouseButton.Right)) && !spawned) {
        InteractionManager im = InteractionManager.instance;
        RaycastHit hit = im.targetHit;
        if (im.isTargetValid) {
          Vector3 point = hit.point + (hit.normal.normalized * spawnHeight);
          CmdSpawnPrefab(point);
          spawned = true;
        }
      }
    }
  }

  [Command] void CmdSpawnPrefab(Vector3 point) {
    Ship ship = Instantiate(prefab);
    ship.transform.position = point;
    NetworkServer.SpawnWithClientAuthority(ship.gameObject, player.gameObject);
    ship.SetOwner(player);
  }

}
