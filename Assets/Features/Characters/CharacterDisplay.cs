using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDisplay : MonoBehaviour
{

	public static CharacterDisplay Create(CharacterData character)
	{
		var go = new GameObject(character.displayName, typeof(CharacterDisplay));
		var component = go.GetComponent<CharacterDisplay>();
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
