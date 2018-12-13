using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	static List<BlockCoordinate> _animatingBlocks;
	
	
	static List<BlockCoordinate> _linkedCoords;
	
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
		_blockInstances = new Block[Game.boardWidth, Game.boardHeight * 2];
		_linkedCoords = new List<BlockCoordinate>();

		//Instantiate board
		for (int x = 0; x < _blockInstances.GetLength(0); x++)
		{
			for (int y = 0; y < _blockInstances.GetLength(1); y++)
			{
				_blockInstances[x, y] = (Instantiate(_blockPrefab) as GameObject).GetComponent<Block>();
				_blockInstances[x, y].Init(new BlockCoordinate(x, y));
				_blockInstances[x, y].transform.SetParent(_myTransform);
				_blockInstances[x, y].gameObject.name = "Block (" + x + "," + y + ")";
			}
		}

		//Set initial board state as random blocks
		for (int x = 0; x < _game.gameState.board.GetLength(0); x++)
		{
			for (int y = 0; y < Game.boardHeight; y++)
			{
				_game.gameState.board[x, y].colorID = 1; //Random.Range(1, Game.totalColorIDs + 1);
			}

			//Invisible temporary slots to make fall animation
			for (int y = Game.boardHeight; y < Game.boardHeight * 2; y++)
			{
				_game.gameState.board[x, y].colorID = 0;
			}
		}
	}

	void OnEnable()
	{
		BlockPointerEvents.PointerEnterBlockEvent += OnPointerEnterBlockEvent;
		BlockPointerEvents.PointerExitBlockEvent += OnPointerExitBlockEvent;
		BlockPointerEvents.PointerDownBlockEvent += OnPointerDownBlockEvent;
		BlockPointerEvents.PointerUpBlockEvent += OnPointerUpBlockEvent;
		GameState.NewLineEvent += OnNewLineEvent;
		Block.ExplodeBlockAnimationEndedEvent += OnExplodeBlockAnimationEndedEvent;
	}

	

	void OnDisable()
	{
		BlockPointerEvents.PointerEnterBlockEvent -= OnPointerEnterBlockEvent;
		BlockPointerEvents.PointerExitBlockEvent -= OnPointerExitBlockEvent;
		BlockPointerEvents.PointerDownBlockEvent -= OnPointerDownBlockEvent;
		BlockPointerEvents.PointerUpBlockEvent -= OnPointerUpBlockEvent;
		GameState.NewLineEvent -= OnNewLineEvent;
		Block.ExplodeBlockAnimationEndedEvent -= OnExplodeBlockAnimationEndedEvent;
	}
	
	
	void OnPointerEnterBlockEvent(BlockCoordinate coord)
	{
		if (_game.gameState.board[coord.x, coord.y].state != BlockState.State.ExplodeAnimation
		    &&_game.gameState.board[coord.x, coord.y].state != BlockState.State.WaitingForNewColorAnimation)
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
						if (_game.gameState.LastBlockSelected().IsHexNeighbor(coord))
							_game.gameState.SelectBlock(coord);
					}
				}
			}
			else
			{
				
				_game.gameState.board[coord.x, coord.y].state = BlockState.State.Over;
			}
		}
	}
	void OnPointerExitBlockEvent(BlockCoordinate coord)
	{
		if (_game.gameState.board[coord.x, coord.y].state != BlockState.State.ExplodeAnimation
		    &&_game.gameState.board[coord.x, coord.y].state != BlockState.State.WaitingForNewColorAnimation)
		{
			if (!_pointerPressed)
			{
				_game.gameState.board[coord.x, coord.y].state = BlockState.State.Waiting;
			}
		}
	}
	void OnPointerDownBlockEvent(BlockCoordinate coord)
	{
		if (_game.gameState.board[coord.x, coord.y].state != BlockState.State.ExplodeAnimation
		    &&_game.gameState.board[coord.x, coord.y].state != BlockState.State.WaitingForNewColorAnimation)
		{
			_game.gameState.SelectBlock(coord);
		}
	}
	void OnPointerUpBlockEvent(BlockCoordinate coord)
	{
		if (_game.gameState.board[coord.x, coord.y].state != BlockState.State.ExplodeAnimation
		    &&_game.gameState.board[coord.x, coord.y].state != BlockState.State.WaitingForNewColorAnimation)
		{
			_game.gameState.EndSelection();
		}
	}

	
	void OnNewLineEvent(bool success, List<BlockCoordinate> blocksInTheLine)
	{
		if(success)
			_animatingBlocks = blocksInTheLine;
	}
	void OnExplodeBlockAnimationEndedEvent(BlockCoordinate coord)
	{
		//Set exploded coord as WaitingForNewColorAnimation
		_game.gameState.board[coord.x, coord.y].state = BlockState.State.WaitingForNewColorAnimation;
		
		
		
		
		//if not all animating blocks are WaitingForNewColorAnimation return and do nothing else
		for (int i = 0; i < _animatingBlocks.Count; i++)
		{
			if (_game.gameState.board[_animatingBlocks[i].x, _animatingBlocks[i].y].state !=
			    BlockState.State.WaitingForNewColorAnimation)
			{
				return;
			}
		}
		
		
		//If we didn't returned it means all animating blocks already exploded
		//So we need to trigger falling blocks animation
		TriggerFallAnimations();
	}

	
	

	/// <summary>
	/// Returns the coord of the block that will fall in originCoord
	/// </summary>
	/// <param name="originCoord">Coordinate that will receive a new color</param>
	/// <returns></returns>
	BlockCoordinate GetFallingCoord(BlockCoordinate originCoord)
	{
		
		for (int y = originCoord.y + 1; y < _game.gameState.board.GetLength(1); y++)
		{
			BlockCoordinate coord = new BlockCoordinate(originCoord.x, y); 
			if (_game.gameState.board[coord.x, coord.y].colorID != 0 && !_linkedCoords.Contains(coord))
			{
				_linkedCoords.Add(coord);
				return coord;
			}
		}

		Debug.LogError("There is no color to fall @ "+originCoord);
		return new BlockCoordinate(9999,9999);
	}

	int GetExtraColorsNeeded(int x)
	{
		int extraColorsNeeded = 0;
		for (int y = 0; y < Game.boardHeight; y++)
		{
			if (_game.gameState.board[x, y].colorID == 0)
			{
				extraColorsNeeded++;
			}
			
		}

		return extraColorsNeeded;
	}

	void AddExtraColorsOnTop(int x, int amountOfNewColors)
	{
		for (int y = 0; y < amountOfNewColors; y++)
		{
			_game.gameState.board[x, Game.boardHeight + y].colorID = Random.Range(1, Game.totalColorIDs + 1);
		}
	}
	
	void TriggerFallAnimations()
	{
		//Add a new color at the top of the board if needed
		for (int x = 0; x < Game.boardWidth; x++)
			AddExtraColorsOnTop(x, GetExtraColorsNeeded(x));

		//Relink bgColorGameObject between receiving ans falling coords
		//From bottom to top
		for (int x = 0; x < Game.boardWidth; x++)
		{
			for (int y = 0; y < Game.boardHeight; y++)
			{
				BlockCoordinate coord = new BlockCoordinate(x, y);
				if (_game.gameState.board[coord.x, coord.y].state == BlockState.State.WaitingForNewColorAnimation)
				{
					BlockCoordinate receivingCoord = coord;
					BlockCoordinate fallingCoord = GetFallingCoord(receivingCoord);
					Debug.Log(receivingCoord+" receives "+fallingCoord);
					Block receivingBlock = _blockInstances[receivingCoord.x, receivingCoord.y];
					Block fallingBlock = _blockInstances[fallingCoord.x, fallingCoord.y];
					
					//Link receivingBlock with falling blick
					receivingBlock.DestroyBgColorGameObject();
					receivingBlock.SetBgColorGameObject(fallingBlock.GetBgColorGameObject());
					receivingBlock.StartFallBgColorAnimation();
					
					receivingBlock.Init(receivingCoord);
					receivingBlock.gameObject.name = "Block (" + receivingCoord.x + "," + receivingCoord.y + ")";
					_game.gameState.board[receivingCoord.x, receivingCoord.y].colorID =  _game.gameState.board[fallingCoord.x, fallingCoord.y].colorID;
					_game.gameState.board[receivingCoord.x, receivingCoord.y].state = BlockState.State.Waiting;
					
					//Reset bg color of the falling block 
					fallingBlock.UnlinkBgColor();
					fallingBlock.Init(fallingCoord);
					fallingBlock.InstantiateBgColorGameObject();
					

//					fallingBlock.InstantiateBgColorGameObject();
//					_game.gameState.board[fallingCoord.x, fallingCoord.y].colorID = 0;					
				}
			}
		}
		_linkedCoords.Clear();
		
		//Clear top board
		for (int x = 0; x < Game.boardWidth; x++)
			for (int y = Game.boardHeight; y < _game.gameState.board.GetLength(1); y++)
				_game.gameState.board[x, y].colorID = 0;
		




//
//		//Relink the bgColorObject of each block from bottom to top and trigger fall animation
//		for (int i = 0; i < waitingNewColorBlocks.Count; i++)
//		{
//			
//			BlockCoordinate receivingCoord = waitingNewColorBlocks[i];
//			BlockCoordinate fallingCoord = GetFallingCoord(receivingCoord);
//			Debug.Log("receivingCoord "+receivingCoord+"\t"+fallingCoord);
//			Block receivingBlock = _blockInstances[receivingCoord.x, receivingCoord.y];
//			Block fallingBlock = _blockInstances[fallingCoord.x, fallingCoord.y];
//			GameObject fallingBgColorGameObject = fallingBlock.GetBgColorGameObject();
//			GameObject receivingBgColorGameObject = receivingBlock.GetBgColorGameObject();
//			Destroy(receivingBgColorGameObject);
//			receivingBlock.SetBgColorGameObject(fallingBgColorGameObject);
//			receivingBlock.StartFallBgColorAnimation();
//
//			_game.gameState.board[receivingCoord.x, receivingCoord.y].colorID =
//				_game.gameState.board[fallingCoord.x, fallingCoord.y].colorID; 
//			_game.gameState.board[receivingCoord.x, receivingCoord.y].state = BlockState.State.Waiting;
//			
////			_game.gameState.board[fallingCoord.x, fallingCoord.y].colorID = 0;
//		}
//		
//		_linkedCoords.Clear();
		
	}
	

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
			_pointerPressed = true;
		
		if (Input.GetMouseButtonUp(0))
			_pointerPressed = false;
	}
	
}
