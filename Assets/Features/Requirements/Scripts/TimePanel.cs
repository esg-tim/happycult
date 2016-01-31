using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimePanel : MonoBehaviour 
{
	[SerializeField]
	private Text text;

	private GameController gameController;

	private void Start()
	{
		gameController = FindObjectOfType<GameController>();
	}

	private void Update()
	{
		text.text = string.Format("{0}s", Mathf.CeilToInt(gameController.roundTimeRemaining));
	}

}
