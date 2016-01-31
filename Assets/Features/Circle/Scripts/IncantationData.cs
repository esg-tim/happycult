using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IncantationData : ScriptableObject
{
	public List<MoveType> moves;
	public CharacterData summon;
}
