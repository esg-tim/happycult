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
			if (_allCharacterData == null)
			{
				_allCharacterData = (from characterData in Resources.LoadAll<CharacterData>("") where characterData.active select characterData).ToArray();
			}
			return _allCharacterData.AsEnumerable();
		}
	}

	public bool active = true;
	public Sprite image;
	public string displayName;
	public CharacterData beefCharacter;
	public Sprite summonSymbol;
	public Sprite summonBubble;
}
