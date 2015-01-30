using UnityEngine;
using System.Collections;

public class Directions {
	public static IntVector2 north = new IntVector2(0,+1);
	public static IntVector2 east = new IntVector2(+1,0);
	public static IntVector2 south = new IntVector2(0,-1);
	public static IntVector2 west = new IntVector2(-1,0);
	public static IntVector2[] cardinal = {north,east,south,west};

	public Directions(){}

}
