using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System;

public enum Direction
{
	Clockwise,
	CounterClockwise
}

public enum GameState
{
	Incant,
	Summon
}

public class CircleController : MonoBehaviour 
{
	public float moveTime = 1;

	public int startingCharacterCount = 3;

	public List<Transform> characterMarkTransforms;

	public Transform characterHolder;
	public Transform circleCenter;

	public Image joystickIndicator;

	public UILineRenderer incantationLine;

	public CharacterData mainCharacterData;

	private GameState gameState;

	private List<CharacterData> requiredCharacters;

	private Character mainCharacter;
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
		gameState = GameState.Incant;

		foreach (var markTransform in characterMarkTransforms)
		{
			characterMarks.Add(new CharacterMark() { transform = markTransform });
		}
			
		mainCharacter = Character.Create(mainCharacterData);

		characters.Add(mainCharacter);

		mainCharacter.transform.SetParent(characterHolder);

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

		foreach (var slot in characterMarkSlots)
		{
			if (slot.Value != null) 
				slot.Value.transform.position = slot.Key.transform.position;
		}

		if (moveCoroutine == null)
		{
			if (gameState == GameState.Incant)
			{
				HandleIncantInput(joyMag, joyVector);
			}
			else if (gameState == GameState.Summon)
			{
				
			}
		}
	}

	private void AddIncantationPoint(Vector3 point)
	{
		Vector2 transformed;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(incantationLine.rectTransform, (RectTransformUtility.WorldToScreenPoint(null, point)), null, out transformed);
		var rect = incantationLine.rectTransform.rect;
		var offset = (transformed - rect.min);
		var scaled = new Vector2(offset.x / rect.width, offset.y / rect.height);
		incantationLine.AddPoint(scaled);
	}

	private void HandleIncantInput(float joyMag, Vector2 joyVector)
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

	/*[SerializeField]
	private float incantationLineFidelity = 0.1f;
	public void AddFullLine(Func<float, Vector2> func, float length)
	{
		var segmentCount = length / incantationLineFidelity;

		for (var i = 0; i < segmentCount - 1; i++)
		{
			incantationLine.AddPoint(func(i / (float)segmentCount));
		}

		incantationLine.AddPoint(func(1.0f));
	}*/

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

		public Vector2 Interpolate(float t)
		{
			if (character == null) return Vector2.zero;

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

			return character.transform.position;
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
			var ptA = shiftA.Interpolate(t);
			var ptB = shiftB.Interpolate(t);

			if (characterDisplayA == mainCharacter)
			{
				AddIncantationPoint(ptA);
			}

			if (characterDisplayB == mainCharacter)
			{
				AddIncantationPoint(ptB);
			}

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

			var currentRealIndex = this.characterMarks.IndexOf(current);
			var currentNextIndex = this.characterMarks.IndexOf(next);
			var isDoSiDo = !((currentRealIndex - 1) % this.characterMarks.Count == currentNextIndex ||
				(currentRealIndex + 1) % this.characterMarks.Count == currentNextIndex);

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
				var pt = shift.Interpolate(t);
				if (shift.character == mainCharacter)
				{
					AddIncantationPoint(pt);
				}
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
