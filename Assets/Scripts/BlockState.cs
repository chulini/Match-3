﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the logic state of a block and sends events when a its state changes
/// </summary>
public class BlockState
{
    /// <summary>
    /// To send an event when a block colorID is changed
    /// Event System made by my own tool here:
    /// https://github.com/chulini/unity-quick-events
    /// </summary>
    /// <param name="coord">Coordinate of the block</param>
    /// <param name="fromColorID">Last Color ID</param>
    /// <param name="toColorID">New Color ID</param>
    public delegate void BlockColorIDChangedDelegate(BlockCoordinate coord, int fromColorID, int toColorID);
    public static event BlockColorIDChangedDelegate BlockColorIDChangedEvent;
    static void BlockColorIDChanged(BlockCoordinate coord, int fromColorID, int toColorID){
        if(BlockColorIDChangedEvent != null)
            BlockColorIDChangedEvent(coord,fromColorID,toColorID);
    }
    
    /// <summary>
    /// To send an event when a block colorID is changed
    /// </summary>
    /// <param name="coord">Coordinate of the block</param>
    /// <param name="fromSelectionState">Last selection state</param>
    /// <param name="toSelectionState">New selection state</param>
    public delegate void BlockSelectionChangedDelegate(BlockCoordinate coord, SelectionState fromSelectionState, SelectionState toSelectionState);
    public static event BlockSelectionChangedDelegate BlockSelectionChangedEvent;
    static void BlockSelectionChanged(BlockCoordinate coord, SelectionState fromSelectionState, SelectionState toSelectionState){
        if(BlockSelectionChangedEvent != null)
            BlockSelectionChangedEvent(coord,fromSelectionState,toSelectionState);
    }
    
    /// <summary>
    /// A block can be:
    /// Waiting (waiting for interaction)
    /// Selected (Selected to make the line)
    /// Over (With the mouse over)
    /// </summary>
    public enum SelectionState{Waiting, Selected, Over}
    
    /// <summary>
    /// Current selection state
    /// </summary>
    SelectionState _selectionState = SelectionState.Waiting;
    public SelectionState selectionState
    {
        get { return _selectionState; }
        set
        {
            if (_selectionState != value)
            {
                SelectionState fromSelectionState = _selectionState;
                _selectionState = value;
                BlockSelectionChanged(_coordinate, fromSelectionState, _selectionState);
            }
        }
    }
    /// <summary>
    /// The current color of the block
    /// </summary>
    int _colorID = 0;
    public int colorID
    {
        get { return _colorID; }
        set
        {
            if (_colorID != value)
            {
                int fromColorID = _colorID;
                _colorID = value;
                BlockColorIDChanged(coordinate,fromColorID,colorID);
            }
        }
    }
    
    /// <summary>
    /// The coordinate of the block
    /// </summary>
    readonly BlockCoordinate _coordinate;
    BlockCoordinate coordinate
    {
        get { return _coordinate; }
    }
    public BlockState(BlockCoordinate coordinate)
    {
        _coordinate = coordinate;
    }
    

    
}
