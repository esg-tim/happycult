using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IEnumerableExtensions
{
	public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> self, int count)
	{
		var list = self.ToList();
		for (var i = 0; i < count; i++)
		{
			var index = UnityEngine.Random.Range(0, list.Count());
			yield return list[index];
			list.RemoveAt(index);
		}
	}
}
