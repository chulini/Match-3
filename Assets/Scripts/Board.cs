using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	[SerializeField] GameObject _blockPrefab;
	GameState _gameState;
	Block[,] _blockInstances;
	Transform _myTransform;

	void Awake()
	{
		_myTransform = transform;
	}


	/// <summary>
	/// Initializes a new board on the 
	/// </summary>
	/// <param name="gameState"></param>
	public void Init(GameState gameState)
	{
		_gameState = gameState;
		_blockInstances = new Block[Game.boardWidth,Game.boardHeight];
		
		//Instantiate board
		for (int x = 0; x < Game.boardWidth; x++)
		{
			for (int y = 0; y < Game.boardHeight; y++)
			{
				_blockInstances[x, y] = (Instantiate(_blockPrefab) as GameObject).GetComponent<Block>();
				_blockInstances[x,y].Init(new BlockCoordinate(x,y));
				_blockInstances[x,y].transform.SetParent(_myTransform);
			}	
		}
	}
	
}
