using UnityEngine;
using System.Collections;

public class Room {

	public Vector2 position;
	public int width;
	public int height;

	public Room (Vector2 lowerLeftCorner,int width,int height){
		position = lowerLeftCorner;
		this.width = width;
		this.height = height;


	}
}
