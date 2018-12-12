using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class Game : MonoBehaviour
{
	public static readonly int boardWidth = 7;
	public static readonly int boardHeight = 6;
	GameState _gameState;
	Board _board;
	void Awake()
	{
		if(_gameState == null)
			_gameState = new GameState();
		
		_gameState.Reset();
		_board = GetComponent<Board>();
		_board.Init(_gameState);
	}
	
	
	 
}
