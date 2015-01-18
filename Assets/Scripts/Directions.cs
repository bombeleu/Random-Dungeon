using UnityEngine;
using System.Collections;

public class Directions {
	public static Vector2 north = new Vector2(0,+1);
	public static Vector2 east = new Vector2(+1,0);
	public static Vector2 south = new Vector2(0,-1);
	public static Vector2 west = new Vector2(-1,0);
	public static Vector2[] cardinal = {north,east,south,west};

	public Directions(){}

}
