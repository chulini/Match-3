using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{    
    
    /// <summary>
    /// To send an event when a block is changed
    /// Event System made by my own tool here:
    /// https://github.com/chulini/unity-quick-events
    /// </summary>
    /// <param name="coord">Coordinate of the block</param>
    /// <param name="fromColorID">Last Color ID</param>
    /// <param name="toColorID">New Color ID</param>
    public delegate void BlockChangedDelegate(BlockCoordinate coord, int fromColorID, int toColorID);
    public static event BlockChangedDelegate BlockChangedEvent;
    static void BlockChanged(BlockCoordinate coord, int fromColorID, int toColorID){
        if(BlockChangedEvent != null)
            BlockChangedEvent(coord,fromColorID,toColorID);
    }

    readonly int[,] _board;
    public int[,] board
    {
        get { return _board; }
    }
    
    int _score;
    public int score
    {
        get { return _score; }
    }
    public GameState()
    {
        _board = new int[Game.boardWidth,Game.boardHeight];
        _score = 0;
    }

    public void Reset()
    {
        for (int x = 0; x < Game.boardWidth; x++)
        for (int y = 0; y < Game.boardHeight; y++)
            _board[x, y] = 0;
        
        _score = 0;

    }

    public void SetBlock(BlockCoordinate coord, int colorID)
    {
        int fromColorID = _board[coord.x, coord.y];
        _board[coord.x, coord.y] = colorID;
        BlockChanged(coord,fromColorID,colorID);
        
    }
}
