using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the logic state of the game
/// </summary>
public class GameState
{   

    public delegate void NewLineDelegate(bool success, List<BlockCoordinate> blocksInTheLine);
    public static event NewLineDelegate NewLineEvent;
    /// <summary>
    /// Triggers an event when a new line is finished
    /// </summary>
    /// <param name="success">If the line have more than 3 blocks</param>
    /// <param name="blocksInTheLine">The blocks of the line</param>
    static void NewLine(bool success, List<BlockCoordinate> blocksInTheLine){
        if(NewLineEvent != null)
            NewLineEvent(success,blocksInTheLine);
    }
    
    
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
    
    Stack<BlockCoordinate> _selectedBlocks;
    
    public GameState()
    {
        _board = new BlockState[Game.boardWidth,Game.boardHeight];
        _score = 0;
        _selectedBlocks = new Stack<BlockCoordinate>();
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
            
            _selectedBlocks.Push(coord);
            _board[coord.x, coord.y].selectionState = BlockState.SelectionState.Selected;
            
            //If is the first selected set selecting color as the color of the selected block
            if (_selectedBlocks.Count == 1)
                _selectingColorID = _board[coord.x, coord.y].colorID;
        }
    }

    /// <summary>
    /// Undo the selection until the BlockCoord indicated
    /// </summary>
    /// <param name="coord">Selection will be undone until this coord</param>
    public void UnselectUntilBlockCoord(BlockCoordinate coord)
    {
        //Unselect the last block while last block is not the coord
        while (LastBlockSelected() != coord)
        {
            UnselectBlock(LastBlockSelected());
        }
    } 
    
    void UnselectBlock(BlockCoordinate coord)
    {
        //We can unselect only the last selected block
        if (_selectedBlocks.Peek() == coord)
        {
            _selectedBlocks.Pop();
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

    /// <summary>
    /// Returns if coord is in the selection stack
    /// </summary>
    /// <param name="coord">Coordinate to be tested</param>
    /// <returns></returns>
    public bool SelectedContains(BlockCoordinate coord)
    {
        return _selectedBlocks.Contains(coord);
    }
    
    public void EndSelection()
    {
        //If 3 or more blocks are selected, change the colorID, add score and trigger a success line event
        //Otherwise deselect all and trigger a failed line event
        if (_selectedBlocks.Count >= 3)
        {
            _score += _selectedBlocks.Count;
            //Make a list with the selected block to send an event with them
            List<BlockCoordinate> blocksInTheLine = new List<BlockCoordinate>();
            while (_selectedBlocks.Count > 0)
            {
                BlockCoordinate coord =_selectedBlocks.Pop();
                _board[coord.x, coord.y].colorID = 0;
                _board[coord.x, coord.y].selectionState = BlockState.SelectionState.InAnimation;
                blocksInTheLine.Add(coord);
            }
            NewLine(true,blocksInTheLine);
        }
        else
        {
            List<BlockCoordinate> blocksInTheLine = new List<BlockCoordinate>();
            while (_selectedBlocks.Count > 0)
            {
                BlockCoordinate coord =_selectedBlocks.Pop();
                _board[coord.x, coord.y].selectionState = BlockState.SelectionState.Waiting;
                blocksInTheLine.Add(coord);
            }    
            NewLine(false,blocksInTheLine);
        }
        
    }

    public BlockCoordinate LastBlockSelected()
    {
        return _selectedBlocks.Peek();

    }
}
