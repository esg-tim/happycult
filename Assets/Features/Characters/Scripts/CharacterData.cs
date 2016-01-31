using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
	private static CharacterData[] _allCharacterData;
	public static IEnumerable<CharacterData> allCharacterData
	{
		get
		{
			if (_allCharacterData != null)
			{
				_allCharacterData = Resources.LoadAll<CharacterData>("");
			}
			return _allCharacterData;
		}
	}

	public Sprite image;
	public string displayName;
	public CharacterData beefCharacter;
}
