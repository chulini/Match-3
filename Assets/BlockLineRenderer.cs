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
        _myLineRenderer.positionCount = selectedBlocks.Count;
        Vector3[] newPositions = new Vector3[selectedBlocks.Count];
        for (int i = 0; i < selectedBlocks.Count; i++)
        {
            newPositions[i] = Block.Get3DPositionForCoordinate(selectedBlocks[i]) + Vector3.back * 5f;
        }
        
        _myLineRenderer.SetPositions(newPositions);
    }
}
