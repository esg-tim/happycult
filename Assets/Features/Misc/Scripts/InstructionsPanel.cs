using UnityEngine;
using System.Collections;

public class InstructionsPanel : MonoBehaviour 
{
	public GameController gameController;
	public GameObject incantationInstructions;
	public GameObject summonInstructions;

	public void Update()
	{
		incantationInstructions.SetActive(gameController.gameMode == GameMode.Move);
		summonInstructions.SetActive(gameController.gameMode == GameMode.Summon);
	}
}
