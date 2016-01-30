using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class CircleController : MonoBehaviour 
{
	public float moveTime = 1;

	public int startingCharacterCount = 3;

	public List<Transform> characterMarkTransforms;

	public Transform characterHolder;

	private List<CharacterDisplay> characters = new List<CharacterDisplay>();
	private List<CharacterMark> characterMarks = new List<CharacterMark>();
	private Dictionary<CharacterMark, CharacterDisplay> characterMarkSlots = new Dictionary<CharacterMark, CharacterDisplay>();

	private Coroutine moveCoroutine = null;

	public class CharacterMark
	{
		public Transform transform;
	}

	public void Start()
	{
		foreach (var markTransform in characterMarkTransforms)
		{
			characterMarks.Add(new CharacterMark() { transform = markTransform });
		}

		var allCharacterData = Resources.LoadAll<CharacterData>("");

		foreach (var characterData in allCharacterData.TakeRandom(startingCharacterCount))
		{
			var character = CharacterDisplay.Create(characterData);

			characters.Add(character);

			character.transform.SetParent(characterHolder);
		}

	}

	public void Update()
	{
		if (moveCoroutine == null)
		{
			if (Input.anyKeyDown)
			{
				var taking = new List<CharacterMark>(characterMarks);

				var a = Random.Range(0, taking.Count);
				var characterMarkA = taking[a];
				taking.RemoveAt(a);

				var b = Random.Range(0, taking.Count);
				var characterMarkB = taking[b];

				RunMove(DoSwap(characterMarkA, characterMarkB));
			}
		}

		foreach (var slot in characterMarkSlots)
		{
			slot.Value.transform.position = slot.Key.transform.position;
		}
	}

	public void RunMove(IEnumerator move)
	{
		StartCoroutine(RunThenClear(move));
	}

	public IEnumerator RunThenClear(IEnumerator coroutine)
	{
		moveCoroutine = StartCoroutine(coroutine);
		yield return moveCoroutine;
		moveCoroutine = null;
	}

	public IEnumerator DoSwap(CharacterMark a, CharacterMark b)
	{
		var characterDisplayA = characterMarkSlots[a];
		var characterDisplayB = characterMarkSlots[b];

		characterMarkSlots.Remove(a);
		characterMarkSlots.Remove(b);

		var t = 0f;
		while (t < 1)
		{
			characterDisplayA.transform.position = Vector3.Lerp(a.transform.position, b.transform.position, t);
			characterDisplayB.transform.position = Vector3.Lerp(b.transform.position, a.transform.position, t);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime / moveTime;
		}
	}

}
