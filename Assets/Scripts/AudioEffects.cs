using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listen to game events and triggers sound fx
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioEffects : MonoBehaviour
{
	AudioSource _myAudioSource;
	[SerializeField] AudioClip _pianoAudioClip;
	[SerializeField] AudioClip _lineAudioClip;
	
	void Awake()
	{
		_myAudioSource = GetComponent<AudioSource>();
	}

	void OnEnable()
	{
		GameState.SelectedLineChangedEvent += OnSelectedLineChangedEvent;
		GameState.NewLineEvent += OnNewLineEvent;
	}

	


	void OnDisable()
	{	
		GameState.SelectedLineChangedEvent -= OnSelectedLineChangedEvent;
		GameState.NewLineEvent -= OnNewLineEvent;
	}
	
	
	
	void OnSelectedLineChangedEvent(List<BlockCoordinate> selectedBlocks)
	{
		
		_myAudioSource.pitch = GetPentatonicPitch(selectedBlocks.Count-1);
		_myAudioSource.volume = .2f;
		_myAudioSource.PlayOneShot(_pianoAudioClip);
	}

	void OnNewLineEvent(bool success, List<BlockCoordinate> blocksInTheLine, int newScore)
	{
		_myAudioSource.pitch = 1f;
		_myAudioSource.volume = .2f;
		_myAudioSource.PlayOneShot(_lineAudioClip);
	}
	
	float GetPentatonicPitch(int index)
	{
		
		//1.05946^n is the pitch to get n semitones
		switch (index % 5)
		{
			case 0:
				return Mathf.Pow(1.05946f, 12f * (index / 5) + 0);
			case 1:
				return Mathf.Pow(1.05946f, 12f * (index / 5) + 3f);
			case 2:
				return Mathf.Pow(1.05946f, 12f * (index / 5) + 5f);
			case 3:
				return Mathf.Pow(1.05946f, 12f * (index / 5) + 7f);
			case 4:
				return Mathf.Pow(1.05946f, 12f * (index / 5) + 10f);;
			default:
				return 0;
		}
	}

	
}
