using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{   
    readonly BlockState[,] _board;
    public BlockState[,] board
    {
        get { return _board; }
    }
    
    int _score;
    public int score
    {
        get { return _score; }
    }

    int _remainingMoves = Game.remainingMovesAtStart;
    public int remainingMoves
    {
        get { return _remainingMoves; }
    }
    
    /// <summary>
    /// Contains the number of the selected color.
    /// If no selection is made it contains 0.  
    /// </summary>
    int _selectingColorID = 0;
    public int selectingColorID
    {
        get { return _selectingColorID; }
    }
    
    Queue<BlockCoordinate> _selectedBlocks;
    
    public GameState()
    {
        _board = new BlockState[Game.boardWidth,Game.boardHeight];
        _score = 0;
        _selectedBlocks = new Queue<BlockCoordinate>();
    }
    public void Reset()
    {
        for (int x = 0; x < Game.boardWidth; x++)
        {
            for (int y = 0; y < Game.boardHeight; y++)
            {
                _board[x, y] = new BlockState(new BlockCoordinate(x,y));
            }
        }
        _score = 0;
        _remainingMoves = Game.remainingMovesAtStart;
        _selectedBlocks.Clear();
    }

    

    public void SetBlockColor(BlockCoordinate coord, int colorID)
    {
        int fromColorID = _board[coord.x, coord.y].colorID;
        _board[coord.x, coord.y].colorID = colorID;
        
    }

    public void SelectBlock(BlockCoordinate coord)
    {
        if (!_selectedBlocks.Contains(coord))
        {
            
            _selectedBlocks.Enqueue(coord);
            _board[coord.x, coord.y].selectionState = BlockState.SelectionState.Selected;
            
            //If is the first selected set selecting color as the color of the selected block
            if (_selectedBlocks.Count == 1)
                _selectingColorID = _board[coord.x, coord.y].colorID;
        }
    }

    public void UnselectBlock(BlockCoordinate coord)
    {
        //We can unselect only the last selected block
        if (_selectedBlocks.Peek() == coord)
        {
            _selectedBlocks.Dequeue();
            _board[coord.x, coord.y].selectionState = BlockState.SelectionState.Waiting;
            
            //If there is no remaining blocks selected, set _selectingColorID as 0  
            if (_selectedBlocks.Count == 0)
                _selectingColorID = 0;
        }
        else
        {
          Debug.LogWarning("We can unselect only the last selected block.");  
        }
    }
    public void EndSelection()
    {
        while (_selectedBlocks.Count > 0)
        {
            BlockCoordinate coord =_selectedBlocks.Dequeue();
            _board[coord.x, coord.y].selectionState = BlockState.SelectionState.Waiting;
        }
    }

    public BlockCoordinate LastBlockSelected()
    {
        return _selectedBlocks.Peek();

    }
}
