using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundController : MonoBehaviour 
{
	private Dictionary<string, AudioSource> allAudioSources; 

	private static SoundController _main;
	public static SoundController main
	{
		get
		{
			if (_main == null || System.Object.Equals(_main, null))
			{
				_main = FindObjectOfType<SoundController>();
			}
			return _main;
		}
	}

	public void Start()
	{
		allAudioSources = new Dictionary<string, AudioSource>();
		var audioSources = GetComponents<AudioSource>();
		foreach (var source in audioSources)
		{
			allAudioSources.Add(source.clip.name, source);
		}
	}

	public void PlaySound(string name)
	{
		allAudioSources[name].Play();
	}
}
