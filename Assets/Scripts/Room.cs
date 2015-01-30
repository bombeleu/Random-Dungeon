using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

public class Room {

	public IntVector2 lowerLeftCorner;
	public int width;
	public int height;
	public List<IntVector2> tilePositions; 



	public Room (IntVector2 lowerLeftCorner,int width,int height){
		this.lowerLeftCorner = lowerLeftCorner;
		this.width = width;
		this.height = height;
		tilePositions = new List<IntVector2>();



		int x = lowerLeftCorner.x;
		int y = lowerLeftCorner.y;
		for (int i = y; i < y+ height ; i++) {
			for (int j = x; j < x+width ; j++) {
				tilePositions.Add(new IntVector2(j,i));
			}

		}


	}
	public bool Intersects(Room other){
		List<IntVector2> firstNotSecond = tilePositions.Except(other.tilePositions).ToList();
		if(firstNotSecond.Count == tilePositions.Count){
			return false;
		}
		return true;


	}
}
