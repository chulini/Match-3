using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	[SerializeField] MeshRenderer _meshRenderer;
	static Material[] _blockMaterials;

	BlockCoordinate _myCoordinate;
	Transform _myTransform;
	
	/// <summary>
	/// Horizontal separation between blocks
	/// </summary>
	static readonly float hSeparation = .8f;
	
	/// <summary>
	/// Vertical separation between blocks
	/// </summary>
	static readonly float vSeparation = .72f;
	void Awake()
	{
		if (_blockMaterials == null)
			_blockMaterials = Resources.LoadAll<Material>("Materials/Block") as Material[];

		_myTransform = GetComponent<Transform>();
		
		//r = 
		//h = Mathf.Sqrt(3f / 4f) * r;
	}

	public void Init(BlockCoordinate coord)
	{
		_myCoordinate = coord;
		RefreshPosition();
	}

	void OnEnable()
	{
		GameState.BlockChangedEvent += OnBlockChangedEvent;
	}

	void OnDisable()
	{
		GameState.BlockChangedEvent -= OnBlockChangedEvent;
	}

	void OnBlockChangedEvent(BlockCoordinate coord, int fromColorID, int toColorID)
	{
		if (coord == _myCoordinate)
		{
			
		}
	}

	/// <summary>
	/// Sets the block in the position using _myCoordinate
	/// </summary>
	void RefreshPosition()
	{
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

	void Update()
	{
		RefreshPosition();
	}
}
