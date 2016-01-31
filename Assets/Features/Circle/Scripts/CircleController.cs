using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

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

	public Image joystickIndicator;

	private List<CharacterData> requiredCharacters;

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
		var joyVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		var joyMag = joyVector.magnitude;
		joystickIndicator.color = Color.Lerp(Color.clear, Color.white, joyMag);

		var indicatorRectTransform = joystickIndicator.GetComponent<RectTransform>();

		indicatorRectTransform.anchoredPosition = joyVector * indicatorRectTransform.parent.GetComponent<RectTransform>().rect.width / 2;

		if (moveCoroutine == null)
		{
			if (Input.GetButton("Fire4") && joyMag > 0.8f)
			{
				var index = GetIndexInDirection(joyVector);

				if (index != -1)
				{
					var left = ((index - 1) + characterMarks.Count) % characterMarks.Count;
					var right = (index + 1) % characterMarks.Count;

					var a = characterMarks[left];
					var b = characterMarks[right];

					if (a != null || b != null)
					{
						RunMove(DoSwap(a, b, true));
					}
				}
			}
			else if (Input.GetButton("Fire1") && joyMag > 0.8f)
			{
				var index = GetIndexInDirection(joyVector);

				if (index != -1)
				{
					var across = (index + characterMarks.Count / 2) % characterMarks.Count;

					var a = characterMarks[index];
					var b = characterMarks[across];

					if (a != null || b != null)
					{
						RunMove(DoSwap(a, b, false));
					}
				}
			}
			else if (Input.GetButton("Fire3"))
			{
				if (joyMag > 0.8f)
				{
					var index = GetIndexInDirection(joyVector);

					if (index != -1)
					{
						RunMove(DoRotate(Direction.Clockwise, characterMarks[index]));
					}
				}
				else
				{
					RunMove(DoRotate(Direction.Clockwise));
				}

			}
			else if (Input.GetButton("Fire2"))
			{
				if (joyMag > 0.8f)
				{
					var index = GetIndexInDirection(joyVector);
					if (index != -1)
					{
						RunMove(DoRotate(Direction.CounterClockwise, characterMarks[index]));
					}
				}
				else
				{
					RunMove(DoRotate(Direction.CounterClockwise));
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
		var joyDirection = Vector2Extensions.FullAngle(Vector2.right, vec);
		var degreesBetweenMarks = 360.0f / characterMarks.Count;
		var index = joyDirection / degreesBetweenMarks;
		var portion = index - (int)index;
		if (portion < 0.3f || portion > 0.7f)
		{
			return Mathf.RoundToInt(index) % characterMarks.Count;
		}
		else
		{
			return -1;
		}
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

	private class Shift
	{
		public Shift(MoveType moveType, Character character, CharacterMark fromMark, CharacterMark toMark, Transform circleCenter )
		{
			this.moveType = moveType;
			this.character = character;
			this.fromMark = fromMark;
			this.toMark = toMark;
			this.circleCenter = circleCenter;
		}

		public MoveType moveType;
		public Character character;
		public CharacterMark fromMark;
		public CharacterMark toMark;

		private Transform circleCenter;

		public void Interpolate(float t)
		{
			if (character == null) return;

			switch (moveType)
			{
			case MoveType.Cross:
				character.transform.position = MoveTypeMath.EvaluateLine(fromMark.transform.position, toMark.transform.position, t);
				break;
			case MoveType.DoSiDo:
				character.transform.position = MoveTypeMath.EvaluateCircle(fromMark.transform.position, toMark.transform.position, circleCenter.position, t, true);
				break;
			case MoveType.Shift:
				character.transform.position = MoveTypeMath.EvaluateCircle(fromMark.transform.position, toMark.transform.position, circleCenter.position, t, false);
				break;
			}
		}
	}

	public IEnumerator DoSwap(CharacterMark a, CharacterMark b, bool doSiDo)
	{
		var characterDisplayA = characterMarkSlots[a];
		var characterDisplayB = characterMarkSlots[b];

		characterMarkSlots[a] = null;
		characterMarkSlots[b] = null;

		var shiftA = new Shift(doSiDo ? MoveType.DoSiDo : MoveType.Cross, characterDisplayA, a, b, circleCenter);
		var shiftB = new Shift(doSiDo ? MoveType.DoSiDo : MoveType.Cross, characterDisplayB, b, a, circleCenter);

		var t = 0f;
		while (t < 1)
		{
			shiftA.Interpolate(t);
			shiftB.Interpolate(t);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime / moveTime;
		}

		characterMarkSlots[a] = characterDisplayB;
		characterMarkSlots[b] = characterDisplayA;
	}

	public IEnumerator DoRotate(Direction direction, CharacterMark skip = null)
	{
		var characterMarks = skip == null ? this.characterMarks : this.characterMarks.Except(Enumerable.Repeat(skip, 1)).ToList();
		var shifts = new List<Shift>();

		for (var i = 0; i < characterMarks.Count; i++)
		{
			var current = characterMarks[i];

			var nextIndex = direction == Direction.Clockwise ? i + 1 : i - 1;
			nextIndex += characterMarks.Count;
			nextIndex %= characterMarks.Count;

			var next = characterMarks[nextIndex];

			var isDoSiDo = Mathf.Abs(this.characterMarks.IndexOf(current) - this.characterMarks.IndexOf(next)) > 1;

			shifts.Add(new Shift(isDoSiDo ? MoveType.DoSiDo : MoveType.Shift, characterMarkSlots[current], current, next, circleCenter));
		}

		foreach (var mark in characterMarks)
		{
			characterMarkSlots[mark] = null;
		}


		var t = 0f;
		while (t < 1)
		{
			foreach (var shift in shifts)
			{
				shift.Interpolate(t);
			}
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime / moveTime;
		}

		foreach (var shift in shifts)
		{
			characterMarkSlots[shift.toMark] = shift.character;
		}
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
