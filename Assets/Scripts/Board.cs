using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiates the blocks on the scene,
/// listen to pointer events and implements the game logic
/// updating the GameState 
/// </summary>
public class Board : MonoBehaviour
{
	[SerializeField] GameObject _blockPrefab;
	static Game _game;
	static Block[,] _blockInstances;
	static Transform _myTransform;
	static bool _pointerPressed = false;
	
	void Awake()
	{
		_myTransform = transform;
	}
	
	/// <summary>
	/// Initializes a new board on the scene
	/// </summary>
	/// <param name="game">A reference to the Game instance</param>
	public void Init(Game game)
	{
		_game = game;
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
		
		//Set initial board state as random blocks
		for (int x = 0; x < Game.boardWidth; x++)
		{
			for (int y = 0; y < Game.boardHeight; y++)
			{
				_game.gameState.board[x, y].colorID = Random.Range(1, Game.totalColorIDs+1);
			}
		}
	}

	void OnEnable()
	{
		BlockPointerEvents.PointerEnterBlockEvent += OnPointerEnterBlockEvent;
		BlockPointerEvents.PointerExitBlockEvent += OnPointerExitBlockEvent;
		BlockPointerEvents.PointerDownBlockEvent += OnPointerDownBlockEvent;
		BlockPointerEvents.PointerUpBlockEvent += OnPointerUpBlockEvent;
	}


	void OnDisable()
	{
		BlockPointerEvents.PointerEnterBlockEvent -= OnPointerEnterBlockEvent;
		BlockPointerEvents.PointerExitBlockEvent -= OnPointerExitBlockEvent;
		BlockPointerEvents.PointerDownBlockEvent -= OnPointerDownBlockEvent;
		BlockPointerEvents.PointerUpBlockEvent -= OnPointerUpBlockEvent;
	}
	
	
	void OnPointerEnterBlockEvent(BlockCoordinate coord)
	{
		if (_pointerPressed)
		{
			//A new block is selectable only if is from the current selecting color
			if (_game.gameState.board[coord.x, coord.y].colorID == _game.gameState.selectingColorID)
			{
				//If new block is already in the selected queue
				//unselect until this block.
				if (_game.gameState.SelectedContains(coord))
				{
					_game.gameState.UnselectUntilBlockCoord(coord);
				}
				//Otherwise is a new block 
				else
				{
					//Select only if is neighbor from the last selected block 
					if(_game.gameState.LastBlockSelected().IsHexNeighbor(coord))
						_game.gameState.SelectBlock(coord);	
				}
			}
		}
		else
		{
			_game.gameState.board[coord.x, coord.y].selectionState = BlockState.SelectionState.Over;
		}
	}
	void OnPointerExitBlockEvent(BlockCoordinate coord)
	{
		if (!_pointerPressed)
		{
			_game.gameState.board[coord.x, coord.y].selectionState = BlockState.SelectionState.Waiting;
		}
	}
	void OnPointerDownBlockEvent(BlockCoordinate coord)
	{
		_game.gameState.SelectBlock(coord);
	}
	void OnPointerUpBlockEvent(BlockCoordinate coord)
	{
		_game.gameState.EndSelection();	
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
			_pointerPressed = true;
		
		if (Input.GetMouseButtonUp(0))
			_pointerPressed = false;
	}
	
}
