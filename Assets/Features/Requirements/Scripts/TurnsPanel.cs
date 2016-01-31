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
		int currentValue = (int)Mathf.Lerp(turnsBefore, turnsAfter, t);
		while (t < 1)
		{
			string.Format("{0}", currentValue);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime / 1.5f;
			currentValue = (int)Mathf.Lerp(turnsBefore, turnsAfter, t);
		}
			
		isAnimatingTurnGain = false;
		done();
	}

	private void Update()
	{
		if (!isAnimatingTurnGain)
		{
			var turns = gameController.roundTurnsRemaining;
			text.text = string.Format("{0}", turns);
			if (turns <= 3)
			{
				var time = (Time.time % 1f);
				if (time > 0.5f)
				{
					text.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.1f, (time - 0.5f) / 0.5f );
				}
				else
				{
					text.transform.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.one, time / 0.5f );
				}
			}
		}
	}

}
