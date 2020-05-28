using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		MinMaxAttribute attribute = (MinMaxAttribute) this.attribute;
		bool isInt = property.type == "IntRange";
		float lineHeight = EditorGUIUtility.singleLineHeight;
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		EditorGUI.BeginProperty(position, label, property);
		Rect topRect = position;
		topRect.height = lineHeight;
		Rect bottomRect = topRect;
		bottomRect.y += lineHeight;
		RangeDrawer.DrawRangeField(topRect, property, label);
		float startMin, startMax, min, max;
		SerializedProperty minProp, maxProp;
		minProp = property.FindPropertyRelative("min");
		maxProp = property.FindPropertyRelative("max");
		if (isInt) {
			min = minProp.intValue;
			max = maxProp.intValue;
		}
		else {
			min = minProp.floatValue;
			max = maxProp.floatValue;
		}
		startMin = min;
		startMax = max;
		EditorGUI.MinMaxSlider(bottomRect, ref min, ref max, attribute.min, attribute.max);
		if (startMin != min) {
			if (isInt) {
				minProp.intValue = (int) min;
			}
			else {
				minProp.floatValue = min;
			}
		}
		if (startMax != max) {
			if (isInt) {
				maxProp.intValue = (int) max;
			}
			else {
				maxProp.floatValue = max;
			}
		}
		EditorGUI.EndProperty();
		EditorGUI.indentLevel = indent;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return EditorGUIUtility.singleLineHeight * 2;
	}

}

[CustomPropertyDrawer(typeof(FloatRange))]
[CustomPropertyDrawer(typeof(IntRange))]
public class RangeDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		DrawRangeField(position, property, label);
		EditorGUI.EndProperty();
	}

	public static void DrawRangeField(Rect position, SerializedProperty property, GUIContent label) {
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		SerializedProperty minProp, maxProp;
		minProp = property.FindPropertyRelative("min");
		maxProp = property.FindPropertyRelative("max");
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		EditorGUIUtility.labelWidth = 32;
		Rect minRect = position;
		minRect.width /= 2;
		Rect maxRect = minRect;
		maxRect.x += maxRect.width;
		EditorGUI.PropertyField(minRect, minProp, new GUIContent("Min"));
		EditorGUI.PropertyField(maxRect, maxProp, new GUIContent("Max"));
		EditorGUIUtility.labelWidth = 0;
		EditorGUI.indentLevel = indent;
	}

}
