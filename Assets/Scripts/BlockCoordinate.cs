using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	public override string ToString(){
		return "BlockCoordinate("+x+","+y+")";
	}
	public int HorizontalSquareDistance(BlockCoordinate b){
		int deltaX = Mathf.Abs (b.x - this.x);
		int deltaY = Mathf.Abs (b.y - this.y);
		return Mathf.Max (deltaX, deltaY);
	}
}

