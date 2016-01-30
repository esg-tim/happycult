using System;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
	public static Character Create(CharacterData character)
	{
		var go = GameObject.Instantiate(PrefabSettings.main.characterDisplayPrefab);
		go.name = character.name;

		var component = go.GetComponent<Character>();
		component.character = character;

		return component;
	}

	public CharacterData character;

	public Image displayImage;

	public void Start()
	{
		displayImage.sprite = character.image;
	}
}
