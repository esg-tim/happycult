using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TimePanel : MonoBehaviour 
{
	[SerializeField]
	private Text text;

	private GameController gameController;

	private void Start()
	{
		gameController = FindObjectOfType<GameController>();

		gameController.addedTime += (before, after, doneFeedback) => StartCoroutine(AnimateTime(before, after, doneFeedback));
	}

	private bool isAnimatingTimeGain = false;
	private IEnumerator AnimateTime(float timeBefore, float TimeAfter, Action done)
	{
		isAnimatingTimeGain = true;

		var t = 0f; 
		while (t < 1)
		{
			string.Format("{0}s", Mathf.CeilToInt(Mathf.Lerp(timeBefore, TimeAfter, t)));
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime;
		}
			
		isAnimatingTimeGain = false;
		done();
	}

	private void Update()
	{
		if (!isAnimatingTimeGain)
		{
			text.text = string.Format("{0}s", Mathf.CeilToInt(gameController.roundTimeRemaining));
		}
	}

}
