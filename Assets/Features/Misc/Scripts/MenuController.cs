using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour 
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	private Coroutine coroutine;
	public void OnPlayPressed()
	{
		if (coroutine == null)
		{
			coroutine = StartCoroutine(StartGame());
		}
	}

	public void OnQuitPressed()
	{
		if (coroutine == null)
		{
			coroutine = StartCoroutine(Quit());
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

	private IEnumerator Quit()
	{
		var t = 0f;
		while (t < 1)
		{
			canvasGroup.alpha = 1.0f - t;
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime;
		}

		Application.Quit();
	}
}
