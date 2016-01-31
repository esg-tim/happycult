using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour 
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	private Coroutine startGameCoroutine;
	public void OnPlayPressed()
	{
		if (startGameCoroutine == null)
		{
			startGameCoroutine = StartCoroutine(StartGame());
		}
	}

	private IEnumerator StartGame()
	{
		var t = 0f;
		while (t < 1)
		{
			canvasGroup.alpha = 1.0f - t;
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime;
		}

		SceneManager.LoadScene("Main");
	}
}
