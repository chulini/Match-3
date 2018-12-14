using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains the global definitions of the game,
/// the board and the game state
/// </summary>
[RequireComponent(typeof(Board))]
public class Game : MonoBehaviour
{
	public const int boardWidth = 7;//Configurable
	public const int boardHeight = 6; //Configurable
	public const int remainingMovesAtStart = 20; //Configurable
	public const int totalColorIDs = 4; //Configurable from 1 to 8 (To draw more colors add materials in Resources/Materials/Blocks)
	
	GameState _gameState;
	public GameState gameState
	{
		get { return _gameState; }
	}

	static Board _board;
	void Awake()
	{
		SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
	}

	void Start()
	{
		_gameState = new GameState();
		_gameState.Reset();
		_board = GetComponent<Board>();
		_board.Init(this);
	}

	
}
