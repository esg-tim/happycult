using System;
using UnityEngine;

public abstract class CharacterRule : ScriptableObject
{
	public static CharacterRule Create(Type t)
	{
		return (CharacterRule)ScriptableObject.CreateInstance(t);
	}

	public abstract bool Check(GameController controller, Character character);
}
