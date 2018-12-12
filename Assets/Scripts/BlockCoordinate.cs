using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Struct that works as a Vector2 but with integers instead of float
/// </summary>
public struct BlockCoordinate{
	public int x;
	public int y;
	public BlockCoordinate(int _x, int _y){
		x = _x;
		y = _y;
	}
	public bool inBounds(){
		return (x >= 0 && y >= 0 && x < Game.boardWidth && y < Game.boardHeight);
	}

	public static bool operator !=(BlockCoordinate a, BlockCoordinate b) {
		return !(a.x == b.x && a.y == b.y);
	}
	public static bool operator ==(BlockCoordinate a, BlockCoordinate b) {
		return (a.x == b.x && a.y == b.y);
	}
	public override bool Equals(object o){
		if (!(o is BlockCoordinate))
			return false;
		BlockCoordinate b = (BlockCoordinate)o;
		return x == b.x && y == b.y;
	}
	public override int GetHashCode(){
		return x+y;
	}

	public static BlockCoordinate operator +(BlockCoordinate a, BlockCoordinate b) {
		return new BlockCoordinate(a.x + b.x,a.y + b.y);
	}

	public static BlockCoordinate operator -(BlockCoordinate a, BlockCoordinate b) {
		return new BlockCoordinate(a.x - b.x,a.y - b.y);
	}
	public static BlockCoordinate operator *(BlockCoordinate a, int b) {
		return new BlockCoordinate(a.x * b,a.y * b);;
	}

	/// <summary>
	/// Returns if coordToTest is a neighbor on an hexagonal grid
	/// </summary>
	/// <param name="coordToTest">Coord to test</param>
	/// <returns></returns>
	public bool IsHexNeighbor(BlockCoordinate coordToTest)
	{
		if (x % 2 == 0)
		{
			if (coordToTest.inBounds() &&
			    (coordToTest == (this + new BlockCoordinate(0, 1))
			     || coordToTest == (this + new BlockCoordinate(1, 0))
			     || coordToTest == (this + new BlockCoordinate(1, -1))
			     || coordToTest == (this + new BlockCoordinate(0, -1))
			     || coordToTest == (this + new BlockCoordinate(-1, -1))
			     || coordToTest == (this + new BlockCoordinate(-1, 0))))
			{
				return true;
			}
		}
		else
		{
			if (coordToTest.inBounds() &&
			    (coordToTest == (this + new BlockCoordinate(0, 1))
			     || coordToTest == (this + new BlockCoordinate(1, 1))
			     || coordToTest == (this + new BlockCoordinate(1, 0))
			     || coordToTest == (this + new BlockCoordinate(0, -1))
			     || coordToTest == (this + new BlockCoordinate(-1, 0))
			     || coordToTest == (this + new BlockCoordinate(-1, +1))))
			{
				return true;
			}
		}

		return false;
	}

	public override string ToString(){
		return "BlockCoordinate("+x+","+y+")";
	}
	
}

