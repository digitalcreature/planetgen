using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    if (!property.serializedObject.isEditingMultipleObjects) {
      EditorGUI.BeginProperty(position, label, property);
      int layer = property.intValue;
      layer = EditorGUI.LayerField(position, label, layer);
      property.intValue = layer;
      EditorGUI.EndProperty();
    }
    else {
      EditorGUI.LabelField(position, label);
    }
  }
}
