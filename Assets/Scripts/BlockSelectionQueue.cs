using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Queue of the current selected blocks
/// </summary>
public class BlockSelectionQueue
{
    List<BlockCoordinate> _q;

    public BlockSelectionQueue()
    {
        _q = new List<BlockCoordinate>();
    }

    public void Clear()
    {
        _q.Clear();
    }
    
}
