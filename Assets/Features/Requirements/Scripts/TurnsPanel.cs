using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TurnsPanel : MonoBehaviour 
{
	[SerializeField]
	private Text text;

	private GameController gameController;

	private void Start()
	{
		gameController = FindObjectOfType<GameController>();

		gameController.addedTurns += (before, after, doneFeedback) => StartCoroutine(AnimateTurns(before, after, doneFeedback));
	}

	private bool isAnimatingTurnGain = false;
	private IEnumerator AnimateTurns(int turnsBefore, int turnsAfter, Action done)
	{
		isAnimatingTurnGain = true;

		var t = 0f; 
		var currentValue = Mathf.CeilToInt(Mathf.Lerp(turnsBefore, turnsAfter, t));
		while (t < 1)
		{
			string.Format("{0}", currentValue);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime / 1.5f;
			var lastValue = currentValue;
			currentValue = Mathf.CeilToInt(Mathf.Lerp(turnsBefore, turnsAfter, t));
		}
			
		isAnimatingTurnGain = false;
		done();
	}

	private void Update()
	{
		if (!isAnimatingTurnGain)
		{
			text.text = string.Format("{0}", Mathf.CeilToInt(gameController.roundTurnsRemaining));
		}
	}

}
