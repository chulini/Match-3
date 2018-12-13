using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the logic state of a block and sends events when a its state changes
/// </summary>
public class BlockState
{
    
    public delegate void BlockColorIDChangedDelegate(BlockCoordinate coord, int fromColorID, int toColorID);
    public static event BlockColorIDChangedDelegate BlockColorIDChangedEvent;
    /// <summary>
    /// Triggers an event when a block colorID is changed
    /// </summary>
    /// <param name="coord">Coordinate of the block</param>
    /// <param name="fromColorID">Last Color ID</param>
    /// <param name="toColorID">New Color ID</param>
    static void BlockColorIDChanged(BlockCoordinate coord, int fromColorID, int toColorID){
        if(BlockColorIDChangedEvent != null)
            BlockColorIDChangedEvent(coord,fromColorID,toColorID);
    }
    
    
    public delegate void BlockSelectionChangedDelegate(BlockCoordinate coord, State fromState, State toState);
    public static event BlockSelectionChangedDelegate BlockSelectionChangedEvent;
    /// <summary>
    /// Triggers an event when a block colorID is changed
    /// </summary>
    /// <param name="coord">Coordinate of the block</param>
    /// <param name="fromState">Last selection state</param>
    /// <param name="toState">New selection state</param>
    static void BlockSelectionChanged(BlockCoordinate coord, State fromState, State toState){
        if(BlockSelectionChangedEvent != null)
            BlockSelectionChangedEvent(coord,fromState,toState);
    }
    
    /// <summary>
    /// A block can be:
    /// Waiting (waiting for interaction)
    /// Selected (Selected to be part of the line)
    /// Over (With the mouse over)
    /// ExplodeAnimation (The block is exploding)
    /// WaitingForNewColorAnimation (The block will change color)
    /// </summary>
    public enum State{Waiting, Selected, Over, ExplodeAnimation, WaitingForNewColorAnimation}
    
    /// <summary>
    /// Current selection state
    /// </summary>
    State _state = State.Waiting;
    public State state
    {
        get { return _state; }
        set
        {
            if (_state == value) return;
            State fromState = _state;
            _state = value;
            BlockSelectionChanged(_coordinate, fromState, _state);
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
            if (_colorID == value) return;
            int fromColorID = _colorID;
            _colorID = value;
            BlockColorIDChanged(coordinate,fromColorID,colorID);
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
