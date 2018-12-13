using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the global definitions of the game,
/// the board and the game state and the UIManager
/// </summary>
[RequireComponent(typeof(Board))]
public class Game : MonoBehaviour
{
	public const int boardWidth = 7;
	public const int boardHeight = 6;
	public const int remainingMovesAtStart = 1;
	public const int totalColorIDs = 4;
	[SerializeField] UIManager _uiManager;
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
		_uiManager.Init(gameState);
		
	}

}
