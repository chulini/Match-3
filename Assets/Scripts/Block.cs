using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Refreshes the layout of the block listening to BlockState events
/// </summary>
public class Block : MonoBehaviour
{
	/// <summary>
	/// Horizontal separation between blocks
	/// </summary>
	static readonly float hSeparation = .8f;
	
	/// <summary>
	/// Vertical separation between blocks
	/// </summary>
	static readonly float vSeparation = .72f;
	
	[Header("Graphics Elements")] 
	[SerializeField] MeshRenderer _bgBlockMeshRenderer;
	[SerializeField] Transform _bgBlockMeshTransform;
	[SerializeField] MeshRenderer _borderMeshRenderer;
	[SerializeField] Transform _borderTransform;

	[Header("Border Materials")] 
	[SerializeField] Material borderSelectedMaterial;
	[SerializeField] Material borderOverMaterial;
	
	static Material[] _blockMaterials;

	BlockCoordinate _myCoordinate;
	public BlockCoordinate myCoordinate
	{
		get { return _myCoordinate; }
	}
	Transform _myTransform;
	float _targetScale = .9f;
	Vector3 _positionInHexGid;
	Board _board;
	void Awake()
	{
		if (_blockMaterials == null)
			_blockMaterials = Resources.LoadAll<Material>("Materials/Block") as Material[];

		_myTransform = GetComponent<Transform>();
	}

	public void Init(BlockCoordinate coord, Board board)
	{
		_board = board;
		_myCoordinate = coord;
		// Set the block in the position using _myCoordinate using hexagonal grid
		//	AXISES
		//	(0,0) is at bottom left
		//	Y
		//	^
		//	|
		//	|
		//	0-------> X
		float yOffset = (_myCoordinate.x % 2 == 0) ? 0 : vSeparation;
		_positionInHexGid = new Vector3(_myCoordinate.x * hSeparation * 1.5f, _myCoordinate.y * vSeparation * 2f+yOffset);
		_myTransform.position = _positionInHexGid;
		_bgBlockMeshTransform.localScale = Vector3.zero;
	}

	void OnEnable()
	{
		BlockState.BlockColorIDChangedEvent += OnBlockColorIDChangedEvent;
		BlockState.BlockSelectionChangedEvent += OnBlockSelectionChangedEvent;
		GameState.NewLineEvent += OnNewLineEvent;
	}
	
	void OnDisable()
	{
		BlockState.BlockColorIDChangedEvent-= OnBlockColorIDChangedEvent;
		BlockState.BlockSelectionChangedEvent -= OnBlockSelectionChangedEvent;
		GameState.NewLineEvent -= OnNewLineEvent;
	}

	void OnBlockColorIDChangedEvent(BlockCoordinate coord, int fromColorID, int toColorID)
	{
		if (coord == _myCoordinate)
		{
			if(toColorID != 0)
				_bgBlockMeshRenderer.sharedMaterial = _blockMaterials[toColorID];
			
		}
	}
	void OnBlockSelectionChangedEvent(BlockCoordinate coord, BlockState.SelectionState fromSelectionState, BlockState.SelectionState toSelectionState)
	{
		if (coord == _myCoordinate)
		{
			if (toSelectionState == BlockState.SelectionState.Over)
			{
				_targetScale = 1f;
				_borderMeshRenderer.enabled = true;
				_borderMeshRenderer.sharedMaterial = borderOverMaterial;
			} else if (toSelectionState == BlockState.SelectionState.Selected)
			{
				_bgBlockMeshTransform.localScale = Vector3.one*1.1f;
				_targetScale = 1f;
				_borderMeshRenderer.enabled = true;
				_borderMeshRenderer.sharedMaterial = borderSelectedMaterial;
			}
			else if (toSelectionState == BlockState.SelectionState.Waiting)
			{
				_targetScale = .9f;
				_borderMeshRenderer.enabled = false;
			}
		}
	}

	void OnNewLineEvent(bool success, List<BlockCoordinate> blocksInTheLine)
	{
		if (success)
		{
			for (int i = 0; i < blocksInTheLine.Count; i++)
				if (blocksInTheLine[i] == myCoordinate)
					StartCoroutine(ExplodeAnimation(.3f));
		}
	}
	IEnumerator ExplodeAnimation(float duration)
	{
		_borderMeshRenderer.enabled = false;
		_targetScale = .7f;
		float t0 = Time.time;
		float r = 0;
		do
		{
			r = (Time.time - t0) / duration;
			_bgBlockMeshTransform.position = _positionInHexGid 
			                                 + new Vector3(Random.Range(-1f,1f)*r*.2f,Random.Range(-1f,1f)*r*.2f,0);
			yield return null;
		} while (r < 1f);
		
		_bgBlockMeshTransform.localScale = Vector3.zero;
		_targetScale = 0;
		
		//TODO trigger pop audio fx
		//TODO get upwards block
		
	}
	void Update()
	{
		_bgBlockMeshTransform.localScale =
			Vector3.Lerp(_bgBlockMeshTransform.localScale, Vector3.one*_targetScale, Time.deltaTime * 12f);
	}
}
