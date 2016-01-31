using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class IncantationData : ScriptableObject
{
	private static IncantationData[] _allIncantationData;
	public static IEnumerable<IncantationData> allIncantationData
	{
		get
		{
			if (_allIncantationData == null)
			{
				_allIncantationData = Resources.LoadAll<IncantationData>("");
			}
			return _allIncantationData.AsEnumerable();
		}
	}

	public static IncantationData GetIncantationData(List<CircleController.GameMove> incantation)
	{
		return allIncantationData.FirstOrDefault(x => x.Matches(incantation));
	}

	private bool Matches(List<CircleController.GameMove> incantation)
	{
		if (isCycle)
		{
			foreach (var cycleOption in GetCycleOptions())
			{
				if (Match(incantation, cycleOption))
					return true;
			}
		}
		else
		{
			if (Match(incantation, moves))
				return true;
		}
		return false;
	}

	private bool Match(IEnumerable<CircleController.GameMove> gameMoves, IEnumerable<Move> dataMoves)
	{
		if (gameMoves.Count() != dataMoves.Count())
			return false;

		var gameMovesEnumerator = gameMoves.GetEnumerator();
		var dataMovesEnumerator = dataMoves.GetEnumerator();

		while (gameMovesEnumerator.MoveNext() && dataMovesEnumerator.MoveNext())
		{
			if (!(gameMovesEnumerator.Current.MatchDataMove(dataMovesEnumerator.Current)))
				return false;
		}

		return true;
	}

	private IEnumerable<IEnumerable<Move>> GetCycleOptions()
	{
		for (var i = 0; i < moves.Count; i++)
		{
			yield return StartWith(i);
		}
	}

	private IEnumerable<Move> StartWith(int number)
	{
		for (var i = 0; i < moves.Count; i++)
		{
			yield return moves[number + i % moves.Count];
		}
	}



	[Serializable]
	public class Move
	{
		public MoveType type;
		public Direction direction;
	}

	public bool isCycle;
	public List<Move> moves;
	public CharacterData summon;
}

