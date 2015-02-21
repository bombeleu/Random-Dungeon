using UnityEngine;
using System.Collections;

public class Direction {
	public static IntVector2 NORTH = new IntVector2(0,+1);
	public static IntVector2 EAST = new IntVector2(+1,0);
	public static IntVector2 SOUTH = new IntVector2(0,-1);
	public static IntVector2 WEST = new IntVector2(-1,0);
	public static IntVector2 NONE = new IntVector2(0,0);
	public static IntVector2[] CARDINAL = {NORTH,EAST,SOUTH,WEST};

	public Direction(){}

}
