using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public enum Direction
{
	Clockwise,
	CounterClockwise
}

public class CircleController : MonoBehaviour 
{
	public float moveTime = 1;

	public int startingCharacterCount = 3;

	public List<Transform> characterMarkTransforms;

	public Transform characterHolder;
	public Transform circleCenter;

	private List<Character> characters = new List<Character>();
	private List<CharacterMark> characterMarks = new List<CharacterMark>();
	private Dictionary<CharacterMark, Character> characterMarkSlots = new Dictionary<CharacterMark, Character>();

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

		foreach (var characterData in allCharacterData.PickRandom(startingCharacterCount))
		{
			var character = Character.Create(characterData);

			characters.Add(character);

			character.transform.SetParent(characterHolder);
		}

		var characterMarksToAddTo = characterMarks.TakeRandom(characters.Count).GetEnumerator();
		foreach (var character in characters)
		{
			characterMarksToAddTo.MoveNext();
			characterMarkSlots.Add(characterMarksToAddTo.Current, character);
		}

		var added = characterMarkSlots.Keys.ToArray();
		foreach (var mark in characterMarks.Except(added))
		{
			characterMarkSlots.Add(mark, null);
		}

	}

	public void Update()
	{
		if (moveCoroutine == null)
		{
			var joyVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

			if (joyVector.magnitude > 0.8f)
			{
				var index = GetIndexInDirection(joyVector);
				var left = ((index - 1) + characterMarks.Count) % characterMarks.Count;
				var right = (index + 1) % characterMarks.Count;

				var a = characterMarks[left];
				var b = characterMarks[right];

				if (a != null || b != null)
				{
					RunMove(DoSwap(a, b));
				}
			}
		}

		foreach (var slot in characterMarkSlots)
		{
			if (slot.Value != null) 
				slot.Value.transform.position = slot.Key.transform.position;
		}
	}

	public int GetIndexInDirection(Vector2 vec)
	{
		var joyDirection = SignedAngleBetween(Vector2.right, vec);
		var degreesBetweenMarks = 360.0f / characterMarks.Count;
		return Mathf.RoundToInt(joyDirection / degreesBetweenMarks) % characterMarks.Count;
	}

	public int GetIndexOfCharacter(Character character)
	{
		foreach (var pair in characterMarkSlots)
		{
			if (pair.Value == character)
				return characterMarks.IndexOf(pair.Key);
		}
		return -1;
	}

	public Character GetCharacterAtIndex(int markIndex)
	{
		while (markIndex < 0) markIndex += characterMarks.Count;
		markIndex %= characterMarks.Count;

		return characterMarkSlots[characterMarks[markIndex]];
	}

	private float SignedAngleBetween(Vector2 a, Vector2 b)
	{
		// angle in [0,180]
		float angle = Vector3.Angle(a,b);
		float sign = Mathf.Sign(Vector3.Dot(Vector3.back,Vector3.Cross(a,b)));

		// angle in [-179,180]
		float signed_angle = angle * sign;

		// angle in [0,360] (not used but included here for completeness)
		float angle360 =  signed_angle < 0 ? signed_angle + 360 : signed_angle;

		return angle360;
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

		characterMarkSlots[a] = null;
		characterMarkSlots[b] = null;

		var t = 0f;
		while (t < 1)
		{
			if (characterDisplayA != null)
			{
				characterDisplayA.transform.position = Vector3.Lerp(a.transform.position, b.transform.position, t);
			}
			if (characterDisplayB != null)
			{
				characterDisplayB.transform.position = Vector3.Lerp(b.transform.position, a.transform.position, t);
			}
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime / moveTime;
		}

		characterMarkSlots[a] = characterDisplayB;
		characterMarkSlots[b] = characterDisplayA;
	}

	private Vector2[] FindCircleCenter(Vector2 a, Vector2 b, float r)
	{
		var diff = (b - a);
		var mag = diff.magnitude;

		var midpoint = a + diff.normalized * mag / 2;

		var q = Mathf.Sqrt(Mathf.Pow(r, 2) - Mathf.Pow((mag / 2), 2));

		return new Vector2[] {
			new Vector2(
				midpoint.x + q * (a.y - b.y) / mag,
				midpoint.y + q * (b.x - a.x) / mag
			),
			new Vector2(
				midpoint.x - q * (a.y - b.y) / mag,
				midpoint.y - q * (b.x - a.x) / mag
			)
		};
	}

}
