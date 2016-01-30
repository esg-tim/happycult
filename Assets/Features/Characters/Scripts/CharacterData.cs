using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
	public Sprite image;
	public string displayName;

	[SerializeField, HideInInspector]
	private List<CharacterRule> rules;

	public bool CheckRules(CircleController controller, Character character)
	{
		return rules.All(x => x.Check(controller, character));
	}

	#if UNITY_EDITOR

	public void AddRule(CharacterRule rule)
	{
		rules.Add(rule);

		AssetDatabase.AddObjectToAsset(rule, this);
	}

	public void RemoveAndDestroy(CharacterRule rule)
	{
		rules.Remove(rule);

		UnityEngine.Object.DestroyImmediate(rule, true);
	}

	#endif
}
