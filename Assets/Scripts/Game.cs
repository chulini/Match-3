using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Board))]
public class Game : MonoBehaviour
{
	public const int boardWidth = 7;
	public const int boardHeight = 6;
	GameState _gameState;
	public GameState gameState
	{
		get { return _gameState; }
	}

	static Board _board;
	void Awake()
	{
		if(_gameState == null)
			_gameState = new GameState();
		
		_gameState.Reset();
		_board = GetComponent<Board>();
		_board.Init(this);
	}
	
	
	 
}
