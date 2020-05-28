using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TFragment))]
public class TFragmentEditor : Editor {

  public override void OnInspectorGUI() {
    TFragment fragment = (TFragment) target;
    EditorGUILayout.LabelField("Detail Factor", fragment.detailFactor.ToString());
  }

  void OnSceneGUI() {
    TFragment fragment = (TFragment) target;
    Handles.DrawWireCube(fragment.center, Vector3.one);
  }

}
