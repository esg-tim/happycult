using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(CharacterRule), true)]
public class CharacterRulePropertyDrawer : PropertyDrawer 
{
	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		var obj = new SerializedObject(property.objectReferenceValue);

		var height = 0f;
		foreach (var prop in GetProperties(obj))
		{
			height += EditorGUI.GetPropertyHeight(prop);
		}

		return height;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		var obj = new SerializedObject(property.objectReferenceValue);

		var rect = new Rect(position);
		rect.height = 0f;
		foreach (var prop in GetProperties(obj))
		{
			rect.height = EditorGUI.GetPropertyHeight(prop);
			EditorGUI.PropertyField(rect, prop);
			rect.y += rect.height;
		}

		obj.ApplyModifiedProperties();
	}

	private IEnumerable<SerializedProperty> GetProperties(SerializedObject obj)
	{
		var iterator = obj.GetIterator();
		var enterChildren = true;
		while (iterator.NextVisible(enterChildren))
		{
			enterChildren = false;
			yield return iterator;
		}
	}
}
