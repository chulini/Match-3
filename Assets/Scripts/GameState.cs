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

    int _remainingMoves = 10;
    public int remainingMoves
    {
        get { return _remainingMoves; }
    }

    public GameState()
    {
        _board = new BlockState[Game.boardWidth,Game.boardHeight];
        _score = 0;
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

    }

    public void SetBlock(BlockCoordinate coord, int colorID)
    {
        int fromColorID = _board[coord.x, coord.y].colorID;
        _board[coord.x, coord.y].colorID = colorID;
        
    }
}
