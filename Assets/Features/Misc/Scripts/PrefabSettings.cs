using System;
using UnityEngine;

[CreateAssetMenu]
public class PrefabSettings : ScriptableObject
{
	private static PrefabSettings _main;
	public static PrefabSettings main
	{
		get
		{
			return _main ?? (_main = Resources.LoadAll<PrefabSettings>("")[0]);
		}
	}

	public GameObject characterDisplayPrefab;
}
