using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEffects : MonoBehaviour
{
	AudioSource _myAudioSource;
	
	void Awake()
	{
		_myAudioSource = GetComponent<AudioSource>();
	}

	void OnEnable()
	{
		//throw new System.NotImplementedException();
	}

	void OnDisable()
	{
		//throw new System.NotImplementedException();
	}
}
