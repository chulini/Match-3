using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses LineRenderer to draw a line between selected blocks
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class BlockLineRenderer : MonoBehaviour
{
    LineRenderer _myLineRenderer;
    BlockCoordinate _firstCoord;
    
    void Awake()
    {
        _myLineRenderer = GetComponent<LineRenderer>();
        _myLineRenderer.positionCount = 0;
    }

    void OnEnable()
    {
        GameState.SelectedLineChangedEvent += OnSelectedLineChangedEvent;
        GameState.NewLineEvent += OnNewLineEvent;

    }

    

    void OnDisable()
    {
        GameState.SelectedLineChangedEvent -= OnSelectedLineChangedEvent;
        GameState.NewLineEvent -= OnNewLineEvent;
    }

    void OnNewLineEvent(bool success, List<BlockCoordinate> blocksInTheLine, int newScore)
    {
        _myLineRenderer.positionCount = 0;
    }

    void OnSelectedLineChangedEvent(List<BlockCoordinate> selectedBlocks, int lineColorID)
    {
        if (selectedBlocks.Count == 1)
        {
            _firstCoord = selectedBlocks[0];
            _myLineRenderer.sharedMaterial = Block.blockMaterials[lineColorID];
        }
        
        //Vertex of the line renderer should be triplicated so the width of the line doesn't change
        _myLineRenderer.positionCount = selectedBlocks.Count*3;
        Vector3[] newPositions = new Vector3[selectedBlocks.Count*3];
        for (int i = 0; i < selectedBlocks.Count; i++)
        {
            newPositions[3*i] = Block.Get3DPositionForCoordinate(selectedBlocks[i]) + Vector3.back * 5f;
            newPositions[3*i+1] = Block.Get3DPositionForCoordinate(selectedBlocks[i]) + Vector3.back * 5f;
            newPositions[3*i+2] = Block.Get3DPositionForCoordinate(selectedBlocks[i]) + Vector3.back * 5f;
        }
        
        _myLineRenderer.SetPositions(newPositions);
    }
}
