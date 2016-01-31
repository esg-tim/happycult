using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SingleGoalDisplay : MonoBehaviour 
{
	public static SingleGoalDisplay Create(CharacterData goalCharacter)
	{
		var go = GameObject.Instantiate(PrefabSettings.main.singleGoalDisplayPrefab);
		go.name = goalCharacter.name;

		var component = go.GetComponent<SingleGoalDisplay>();
		component.goalCharacter = goalCharacter;

		return component;
	}

	private CharacterData goalCharacter;

	[SerializeField]
	private Image characterIcon;
	[SerializeField]
	private Image symbolIcon;

	public void Start()
	{
		characterIcon.sprite = goalCharacter.summonBubble;
		symbolIcon.sprite = goalCharacter.summonSymbol;
	}
}
