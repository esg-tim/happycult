using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GoalPanel : MonoBehaviour
{
	private GameController gameController;

	private List<SingleGoalDisplay> currentGoalList = new List<SingleGoalDisplay>();

	private void Start()
	{
		gameController = FindObjectOfType<GameController>();

		gameController.goalCharactersChanged += OnGoalCharactersChanged;
	}

	private void OnGoalCharactersChanged(Action feedbackComplete)
	{
		foreach (var goal in currentGoalList)
		{
			Destroy(goal.gameObject);
		}
		currentGoalList.Clear();

		foreach (var character in gameController.currentGoalCharacters)
		{
			var goalDisplay = SingleGoalDisplay.Create(character);
			goalDisplay.transform.SetParent(this.transform, false);
			currentGoalList.Add(goalDisplay);
		}

		feedbackComplete();
	}
}
