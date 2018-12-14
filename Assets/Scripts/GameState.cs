using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains the logic state of the game and send relevant events 
/// </summary>
public class GameState
{   
    
    public delegate void SelectedLineChangedDelegate(List<BlockCoordinate> selectedBlocks);
    public static event SelectedLineChangedDelegate SelectedLineChangedEvent;
    /// <summary>
    /// Triggers an event when the selected line has been modified 
    /// </summary>
    /// <param name="selectedBlocks">List of selected blocks</param>
    static void SelectedLineChanged(List<BlockCoordinate> selectedBlocks){
        if(SelectedLineChangedEvent != null)
            SelectedLineChangedEvent(selectedBlocks);
    }

    
    public delegate void NewLineDelegate(bool success, List<BlockCoordinate> blocksInTheLine, int newScore);
    public static event NewLineDelegate NewLineEvent;

    /// <summary>
    /// Triggers an event when a new line is finished
    /// </summary>
    /// <param name="success">If the line have more than 3 blocks</param>
    /// <param name="blocksInTheLine">The blocks of the line</param>
    /// /// <param name="newScore">The new score after the line</param>
    public static void NewLine(bool success, List<BlockCoordinate> blocksInTheLine, int newScore){
        if(NewLineEvent != null)
            NewLineEvent(success,blocksInTheLine,newScore);
    }
    
    
    public delegate void RemainingMovesUpdatedDelegate(int remainingMoves);
    public static event RemainingMovesUpdatedDelegate RemainingMovesUpdatedEvent;
    
    /// <summary>
    /// Triggers an event when remainingMoves has changed
    /// </summary>
    /// <param name="remainingMoves"></param>
    public static void RemainingMovesUpdated(int remainingMoves){
        if(RemainingMovesUpdatedEvent != null)
            RemainingMovesUpdatedEvent(remainingMoves);
    }
    
    
    public delegate void GameOverDelegate();
    public static event GameOverDelegate GameOverEvent;
    /// <summary>
    /// Triggers game over
    /// </summary>
    static void GameOver(){
        if(GameOverEvent != null)
            GameOverEvent();
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
    
    //Stack containing the blocks selected
    Stack<BlockCoordinate> _selectedBlocks;
    
    public GameState()
    {
        _board = new BlockState[Game.boardWidth,Game.boardHeight*2];
        _score = 0;
        _selectedBlocks = new Stack<BlockCoordinate>();
        
    }
    public void Reset()
    {
        for (int x = 0; x < Game.boardWidth; x++)
        {
            for (int y = 0; y < _board.GetLength(1); y++)
            {
                _board[x, y] = new BlockState(new BlockCoordinate(x,y));
            }
        }
        _score = 0;
        _remainingMoves = Game.remainingMovesAtStart;
        RemainingMovesUpdated(_remainingMoves);
        
        _selectedBlocks.Clear();
    }

    

    public void SetBlockColor(BlockCoordinate coord, int colorID)
    {
        int fromColorID = _board[coord.x, coord.y].colorID;
        _board[coord.x, coord.y].colorID = colorID;
        
    }
    
    /// <summary>
    /// Adds a BlockCoordinate to the selection stack
    /// </summary>
    /// <param name="coord">Added coord</param>
    public void SelectBlock(BlockCoordinate coord)
    {
        if (!_selectedBlocks.Contains(coord))
        {
            
            _selectedBlocks.Push(coord);
            _board[coord.x, coord.y].state = BlockState.State.Selected;
            
            
            //If is the first selected set selecting color as the color of the selected block
            if (_selectedBlocks.Count == 1)
                _selectingColorID = _board[coord.x, coord.y].colorID;
            
            
            SelectedLineChanged(_selectedBlocks.ToList());
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
            _board[coord.x, coord.y].state = BlockState.State.Waiting;
            
            //If there is no remaining blocks selected, set _selectingColorID as 0  
            if (_selectedBlocks.Count == 0)
                _selectingColorID = 0;
            
            SelectedLineChanged(_selectedBlocks.ToList());
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
            _score += 2+ Mathf.FloorToInt(Mathf.Pow(1.5f,_selectedBlocks.Count));
            _remainingMoves--;
            RemainingMovesUpdated(_remainingMoves);
            
            //Make a list with the selected block to send an event with them
            List<BlockCoordinate> blocksInTheLine = new List<BlockCoordinate>();
            while (_selectedBlocks.Count > 0)
            {
                BlockCoordinate coord =_selectedBlocks.Pop();
                _board[coord.x, coord.y].colorID = 0;
                _board[coord.x, coord.y].state = BlockState.State.ExplodeAnimation;
                blocksInTheLine.Add(coord);
            }

            MarkUpwardsBlocksAsWaitingForNewColor(blocksInTheLine);
            NewLine(true,blocksInTheLine,_score);

            if (_remainingMoves == 0)
                GameOver();
        }
        else
        {
            List<BlockCoordinate> blocksInTheLine = new List<BlockCoordinate>();
            while (_selectedBlocks.Count > 0)
            {
                BlockCoordinate coord =_selectedBlocks.Pop();
                _board[coord.x, coord.y].state = BlockState.State.Waiting;
                blocksInTheLine.Add(coord);
            }    
            NewLine(false,blocksInTheLine,_score);
        }
        
    }

    /// <summary>
    /// Each time a block is exploded, all upwards block should be in state WaitingForNewColorAnimation 
    /// </summary>
    /// <param name="blocksInTheLine">A list with all the blocks in the line</param>
    void MarkUpwardsBlocksAsWaitingForNewColor(List<BlockCoordinate> blocksInTheLine)
    {
        //Sort from bottom to top so in case of vertical concave line the top blocks 
        //don't be overwritten as WaitingForNewColorAnimation inst1ead of ExplodeAnimation
        blocksInTheLine = blocksInTheLine.OrderBy(a => a.y).ToList();
        foreach (BlockCoordinate startingCoord in blocksInTheLine)
        {
            _board[startingCoord.x, startingCoord.y].state = BlockState.State.ExplodeAnimation;
            for (int y = 1; startingCoord.y + y < Game.boardHeight; y++)
            {
                _board[startingCoord.x, startingCoord.y + y].state = BlockState.State.WaitingForNewColorAnimation;
            }
        }
    }

    public BlockCoordinate LastBlockSelected()
    {
        return _selectedBlocks.Peek();

    }
}
