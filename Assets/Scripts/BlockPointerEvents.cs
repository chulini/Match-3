using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Is in charge of sending pointer events on any block so other classes can listen and update
/// </summary>
[RequireComponent(typeof(Block))]
public class BlockPointerEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	/// <summary>
	/// To send an event when pointer get in a block
	/// </summary>
	/// <param name="coord">Coordinate of the block being enter</param>
	public delegate void PointerEnterBlockDelegate(BlockCoordinate coord);
	public static event PointerEnterBlockDelegate PointerEnterBlockEvent;
	static void PointerEnterBlock(BlockCoordinate coord){
		if(PointerEnterBlockEvent != null)
			PointerEnterBlockEvent(coord);
	}
	/// <summary>
	/// To send an event when pointer get out off a block
	/// </summary>
	/// <param name="coord">Coordinate of the block being exit</param>
	public delegate void PointerExitBlockDelegate(BlockCoordinate coord);
	public static event PointerExitBlockDelegate PointerExitBlockEvent;
	static void PointerExitBlock(BlockCoordinate coord){
		if(PointerExitBlockEvent != null)
			PointerExitBlockEvent(coord);
	}
	/// <summary>
	/// To send an event when pointer is clicked down
	/// </summary>
	/// <param name="coord">Coordinate of the block being clicked down</param>
	public delegate void PointerDownBlockDelegate(BlockCoordinate coord);
	public static event PointerDownBlockDelegate PointerDownBlockEvent;
	static void PointerDownBlock(BlockCoordinate coord){
		if(PointerDownBlockEvent != null)
			PointerDownBlockEvent(coord);
	}
	
	/// <summary>
	/// To send an event when pointer is clicked up
	/// </summary>
	/// <param name="coord">Coordinate of the block being clicked up</param>
	public delegate void PointerUpBlockDelegate(BlockCoordinate coord);
	public static event PointerUpBlockDelegate PointerUpBlockEvent;
	static void PointerUpBlock(BlockCoordinate coord){
		if(PointerUpBlockEvent != null)
			PointerUpBlockEvent(coord);
	}
	Block _myBlock;

	void Awake()
	{
		_myBlock = GetComponent<Block>();
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		PointerEnterBlock(_myBlock.myCoordinate);
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		PointerExitBlock(_myBlock.myCoordinate);
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		PointerDownBlock(_myBlock.myCoordinate);
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		PointerUpBlock(_myBlock.myCoordinate);
	}
}
