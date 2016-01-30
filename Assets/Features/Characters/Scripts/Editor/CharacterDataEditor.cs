using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System;

[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : Editor 
{
	private ReorderableList ruleList;

	public override void OnInspectorGUI ()
	{
		Editor.DrawPropertiesExcluding(serializedObject);

		if (ruleList == null)
		{
			ruleList = new ReorderableList(serializedObject, serializedObject.FindProperty("rules"));
			ruleList.drawHeaderCallback += (rect) => {
				EditorGUI.LabelField(rect, "Rules");
			};

			ruleList.elementHeightCallback += (index) => {
				var elementProperty = ruleList.serializedProperty.GetArrayElementAtIndex(index);
				return EditorGUI.GetPropertyHeight(elementProperty);
			};

			ruleList.drawElementCallback += (rect, index, isActive, isFocused) => {
				var elementProperty = ruleList.serializedProperty.GetArrayElementAtIndex(index);
				var value = elementProperty.objectReferenceValue;
				EditorGUI.PropertyField(rect, elementProperty);
			};

			ruleList.onAddDropdownCallback += (buttonRect, list) => {
				var genMenu = new GenericMenu();

				var ruleTypes = (from ass in AppDomain.CurrentDomain.GetAssemblies()
					from type in ass.GetTypes()
					where type.IsSubclassOf(typeof(CharacterRule))
					select type);

				foreach (var type in ruleTypes)
				{
					var ruleType = type;
					genMenu.AddItem(new GUIContent(type.Name), false, () => { 
						var elIndex = list.count;
						list.serializedProperty.InsertArrayElementAtIndex(elIndex); 
						var el = list.serializedProperty.GetArrayElementAtIndex(elIndex);
						var obj = CharacterRule.Create(ruleType);
						obj.hideFlags = HideFlags.HideInHierarchy;
						AssetDatabase.AddObjectToAsset(obj, el.serializedObject.targetObject);
						el.objectReferenceValue = obj;
						el.serializedObject.ApplyModifiedProperties();
					});
				}

				genMenu.DropDown(buttonRect);
			};

			ruleList.onRemoveCallback += (list) => {
				var index = ruleList.index;
				if (list.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue != null)
				{
					list.serializedProperty.DeleteArrayElementAtIndex(index);
				}
				list.serializedProperty.DeleteArrayElementAtIndex(index);
			};
		}

		ruleList.DoLayoutList();

		serializedObject.ApplyModifiedProperties();

	}

}
