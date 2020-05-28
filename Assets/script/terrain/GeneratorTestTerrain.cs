using UnityEngine;

public class GeneratorTestTerrain : TerrainBase {

  public Mesh baseMesh;

  public MeshFilter filter { get; private set; }

  protected override void Awake() {
    base.Awake();
    filter = GetComponent<MeshFilter>();
  }

  [ContextMenu("Generate")]
  public override void Generate() {
    Awake();
    generator.Initialize();
    filter.mesh = generator.Generate(new TMesh(baseMesh)).ToMesh();
  }

}
