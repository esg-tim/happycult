using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Character : MonoBehaviour
{
	public static Character Create(CharacterData character)
	{
		var go = GameObject.Instantiate(PrefabSettings.main.characterDisplayPrefab);
		go.name = character.name;

		var component = go.GetComponent<Character>();
		component.characterData = character;

		return component;
	}

	public CharacterData characterData;

	public Image displayImage;

	public Image beefBubble;

	public void ShowBeef()
	{
		StartCoroutine(ShowBeefBubble());
	}

	private IEnumerator ShowBeefBubble()
	{
		SoundController.main.PlaySound("dissatisfied");

		var t = 0f;
		while (t < 1)
		{
			beefBubble.color = Color.Lerp(Color.clear, Color.white, t);
			displayImage.transform.localScale = Vector3.one * (Mathf.Sin((t / 0.5f) * Mathf.PI) * 0.1f + 1.0f);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime / 0.2f;
		}

		beefBubble.color = Color.white;

		yield return new WaitForSeconds(0.5f);

		t = 0f;
		while (t < 1)
		{
			beefBubble.color = Color.Lerp(Color.white, Color.clear, t);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime / 0.2f;
		}
	}

	public void Start()
	{
		displayImage.sprite = characterData.image;
	}
}
