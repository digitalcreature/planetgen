using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(DetailFactorCascades))]
public class DetailFactorCascadesDrawer : PropertyDrawer {

  const float labelPadding = 8;
  const float displayHeight = 80;
  const float padding = 4;

  public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label) {
    float line = EditorGUIUtility.singleLineHeight;
    pos.y += labelPadding;
    pos.height = line;
    EditorGUI.LabelField(pos, label, EditorStyles.boldLabel);
    pos.y += pos.height + padding;
    // deserialize cascades
    var cascadesProp = property.FindPropertyRelative("cascades");
    var cascades = new List<DetailFactorCascades.Cascade>(cascadesProp.arraySize);
    for (int i = 0; i < cascadesProp.arraySize; i ++) {
      var cascadeProp = cascadesProp.GetArrayElementAtIndex(i);
      cascades.Add(new DetailFactorCascades.Cascade(
        cascadeProp.FindPropertyRelative("factorIn").floatValue,
        cascadeProp.FindPropertyRelative("factorOut").floatValue
      ));
    }
    // if there are no cascades, add one
    if (cascades.Count == 0) {
      cascades.Add(new DetailFactorCascades.Cascade(0, 1));
    }
    // draw display, and validate values
    pos.height = displayHeight;
    GUI.Box(pos, "");
    for (int i = 0; i < cascades.Count; i ++) {
      var cascade = cascades[i];
      Rect p = new Rect();
      p.y = 1 - cascade.factorOut;
      p.height = cascade.factorOut;
      if (i == 0) {
        cascade.factorIn = 0;
      }
      p.x = cascade.factorIn;
      if (i < cascades.Count - 1) {
        p.width = cascades[i + 1].factorIn - cascade.factorIn;
      }
      else {
        p.width = 1 - cascade.factorIn;
      }
      p.x = pos.x + p.x * pos.width;
      p.y = pos.y + p.y * pos.height;
      p.width *= pos.width;
      p.height *= pos.height;
      GUI.Box(p, "" + i);
      cascades[i] = cascade;
    }
    pos.y += pos.height + padding;
    pos.height = line;
    // draw sliders
    for (int i = 0; i < cascades.Count; i ++ ) {
      var cascade = cascades[i];
      Rect half = pos;
      half.width /= 2;
      half.width -= padding;
      if (i > 0) {
        cascade.factorIn = Slider(half, cascade.factorIn);
      }
      else {
        Rect quarter = half;
        quarter.width /=2;
        if (cascades.Count > 1) {
          if (GUI.Button(quarter, "-")) {
            cascades.RemoveAt(cascades.Count - 1);
          }
        }
        quarter.x += quarter.width;
        if(GUI.Button(quarter, "+")) {
          cascades.Add(new DetailFactorCascades.Cascade(1, 1));
        }
      }
      half.x += half.width + padding * 2;
      cascade.factorOut = Slider(half, cascade.factorOut);
      pos.y += pos.height + padding;
      cascades[i] = cascade;
    }
    // reserialize cascades
    cascadesProp.arraySize = cascades.Count;
    for (int i = 0; i < cascades.Count; i ++) {
      var cascade = cascades[i];
      var cascadeProp = cascadesProp.GetArrayElementAtIndex(i);
      cascadeProp.FindPropertyRelative("factorIn").floatValue = cascade.factorIn;
      cascadeProp.FindPropertyRelative("factorOut").floatValue = cascade.factorOut;
    }
    // save changes
    property.serializedObject.ApplyModifiedProperties();
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    int count = property.FindPropertyRelative("cascades").arraySize;
    return (count + 1) * (EditorGUIUtility.singleLineHeight + padding) + displayHeight + padding * 2 + labelPadding;
  }

  static float Slider(Rect position, float value) {
    Rect fieldPos = position;
    fieldPos.width = EditorGUIUtility.singleLineHeight * 2;
    Rect sliderPos = position;
    sliderPos.x += fieldPos.width + padding;
    sliderPos.width -= fieldPos.width + padding;
    value = EditorGUI.FloatField(fieldPos, GUIContent.none, value);
    float value2 = GUI.HorizontalSlider(sliderPos, value, 0, 1);
    if (value2 != value) {
      GUI.FocusControl(null);
    }
    return value2;
  }

}
