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
	
	void Awake()
	{
		if (_blockMaterials == null)
			_blockMaterials = Resources.LoadAll<Material>("Materials/Block") as Material[];

		_myTransform = GetComponent<Transform>();
	}

	public void Init(BlockCoordinate coord)
	{
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
		_myTransform.position = new Vector3(_myCoordinate.x * hSeparation * 1.5f, _myCoordinate.y * vSeparation * 2f+yOffset);
	}

	void OnEnable()
	{
		BlockState.BlockColorIDChangedEvent += OnBlockColorIDChangedEvent;
		BlockState.BlockSelectionChangedEvent += OnBlockSelectionChangedEvent;
	}

	void OnDisable()
	{
		BlockState.BlockColorIDChangedEvent-= OnBlockColorIDChangedEvent;
		BlockState.BlockSelectionChangedEvent -= OnBlockSelectionChangedEvent;
	}

	void OnBlockColorIDChangedEvent(BlockCoordinate coord, int fromColorID, int toColorID)
	{
		if (coord == _myCoordinate)
		{
			_bgBlockMeshRenderer.sharedMaterial = _blockMaterials[toColorID];
			
			//TODO make fall animation when fromColorID == 0
			//TODO make pop animation when toColor == 0
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

	void Update()
	{
		_bgBlockMeshTransform.localScale =
			Vector3.Lerp(_bgBlockMeshTransform.localScale, Vector3.one*_targetScale, Time.deltaTime * 12f);
	}
}
