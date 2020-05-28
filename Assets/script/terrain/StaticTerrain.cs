using UnityEngine;

public class StaticTerrain : TerrainBase {

  public float scaleRatio = 1;

  public override void Generate() {
    transform.localScale = Vector3.one * scaleRatio * gfield.boundingRadius;
  }

}
