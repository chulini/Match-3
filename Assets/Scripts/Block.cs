using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Refreshes the layout of the block listening to BlockState events
/// </summary>
public class Block : MonoBehaviour
{
	
	public delegate void ExplodeBlockAnimationEndedDelegate(BlockCoordinate coord);
	public static event ExplodeBlockAnimationEndedDelegate ExplodeBlockAnimationEndedEvent;
	/// <summary>
	/// Triggers an event indicating the end of a block explosion
	/// </summary>
	/// <param name="coord">Exploded coordinate</param>
	static void ExplodeBlockAnimationEnded(BlockCoordinate coord){
		if(ExplodeBlockAnimationEndedEvent != null)
			ExplodeBlockAnimationEndedEvent(coord);
	}
	/// <summary>
	/// Horizontal separation between blocks
	/// </summary>
	static readonly float hSeparation = .8f;
	
	/// <summary>
	/// Vertical separation between blocks
	/// </summary>
	static readonly float vSeparation = .72f;

	[Header("Graphics Elements")] 
	[SerializeField] GameObject _bgColorPrefab;
	[SerializeField] MeshRenderer _borderMeshRenderer;
	

	[Header("Border Materials")] 
	[SerializeField] Material borderSelectedMaterial;
	[SerializeField] Material borderOverMaterial;
	
	static Material[] _blockMaterials;

	
	BlockCoordinate _myCoordinate;
	GameObject _bgColorGameObject;
	MeshRenderer _bgColorMeshRenderer;
	Transform _bgColorMeshTransform;
	
	
	BoxCollider _myBoxCollider;
	public BlockCoordinate myCoordinate
	{
		get { return _myCoordinate; }
	}
	Transform _myTransform;
	float _targetScale = .6f;
	Vector3 _positionInHexGid;
	void Awake()
	{
		if (_blockMaterials == null)
			_blockMaterials = Resources.LoadAll<Material>("Materials/Block") as Material[];

		_myTransform = GetComponent<Transform>();
		_myBoxCollider = GetComponent<BoxCollider>();
	}

	public void Init(BlockCoordinate coord)
	{
		_myCoordinate = coord;
		_myTransform.position = Get3DPositionForCoordinate(coord); 
		_positionInHexGid = _myTransform.position; 
		InstantiateBgColorGameObject();
		_bgColorMeshTransform.position = _myTransform.position;

		if (myCoordinate.y >= Game.boardHeight)
		{
			_myBoxCollider.enabled = false;
			_bgColorMeshRenderer.enabled = false;
		}
	}

	Vector3 Get3DPositionForCoordinate(BlockCoordinate coord)
	{
		// Set the block in the position using hexagonal grid
		//	AXISES
		//	(0,0) is at bottom left
		//	Y
		//	^
		//	|
		//	|
		//	0-------> X
		float yOffset = (_myCoordinate.x % 2 == 0) ? 0 : vSeparation;
		return new Vector3(_myCoordinate.x * hSeparation * 1.5f, _myCoordinate.y * vSeparation * 2f+yOffset);
	}


	public void StartFallBgColorAnimation()
	{
		_bgColorGameObject.GetComponent<BlockBgFallAnimation>().PlayAnimation(_bgColorMeshTransform.position,_positionInHexGid);
	}

	public void DestroyBgColorGameObject()
	{
		if (_bgColorGameObject != null)
		{
			DestroyImmediate(_bgColorGameObject);
			UnlinkBgColor();
		}
		else
		{
			Debug.Log(myCoordinate+ " Can't destroy bg");
		}
	}

	public void UnlinkBgColor()
	{
		_bgColorGameObject = null;
		_bgColorMeshRenderer = null;
		_bgColorMeshTransform = null;
		
	}
	public void InstantiateBgColorGameObject()
	{
		if (_bgColorGameObject == null)
		{
			_bgColorGameObject = Instantiate(_bgColorPrefab) as GameObject;
			SetBgColorGameObject(_bgColorGameObject);
		}

	}
	public void SetBgColorGameObject(GameObject bgColorGameObject)
	{
		_bgColorGameObject = bgColorGameObject;
		_bgColorMeshRenderer = _bgColorGameObject.GetComponent<MeshRenderer>();
		_bgColorMeshTransform = _bgColorGameObject.transform;
		_bgColorMeshTransform.SetParent(_myTransform);
	}

	public GameObject GetBgColorGameObject()
	{
		_bgColorMeshRenderer.enabled = true;
		return _bgColorGameObject;
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
			if (toColorID != 0)
			{
				_bgColorMeshRenderer.sharedMaterial = _blockMaterials[toColorID];
			}
		}
	}
	void OnBlockSelectionChangedEvent(BlockCoordinate coord, BlockState.State fromState, BlockState.State toState)
	{
		if (coord == _myCoordinate)
		{
			if (toState == BlockState.State.Over)
			{
				_targetScale = .8f;
				_borderMeshRenderer.enabled = true;
				_borderMeshRenderer.sharedMaterial = borderOverMaterial;
			} else if (toState == BlockState.State.Selected)
			{
				_bgColorMeshTransform.localScale = Vector3.one*1.1f;
				_targetScale = .8f;
				_borderMeshRenderer.enabled = true;
				_borderMeshRenderer.sharedMaterial = borderSelectedMaterial;
			}
			else if (toState == BlockState.State.Waiting)
			{
				_targetScale = .6f;
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
				{
					StartCoroutine(ExplodeAnimation(.3f+i*.05f));
				}
					
		}
	}
	/// <summary>
	/// Makes the block inflate and shake. After duration, disappears
	/// </summary>
	/// <param name="duration">Duration in seconds</param>
	/// <returns></returns>
	IEnumerator ExplodeAnimation(float duration)
	{
		_borderMeshRenderer.enabled = false;
		_targetScale = 1f;
		float t0 = Time.time;
		float r = 0;
		do
		{
			r = (Time.time - t0) / duration;
			_bgColorMeshTransform.position = _positionInHexGid 
			                                 + new Vector3(Random.Range(-1f,1f)*r*.08f,Random.Range(-1f,1f)*r*.08f,0);
			yield return null;
		} while (r < 1f);
		
		_bgColorMeshTransform.localScale = Vector3.zero;
		_targetScale = 0;
		_bgColorMeshTransform.position = _positionInHexGid;
		ExplodeBlockAnimationEnded(myCoordinate);

	}
	void Update()
	{
		if (_bgColorMeshTransform != null)
		{
			_bgColorMeshTransform.localScale =
				Vector3.Lerp(_bgColorMeshTransform.localScale, Vector3.one * _targetScale, Time.deltaTime * 12f);
		}
	}
}
