using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneratorTestTerrain))]
public class GeneratorTestTerrainEditor : Editor {

  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    // base.OnInspectorGUI();
    if(GUILayout.Button("Update")) {
      ((GeneratorTestTerrain) target).Generate();
    }
  }

}
