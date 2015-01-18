using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

public class Room {

	public Vector2 lowerLeftCorner;
	public int width;
	public int height;
	public List<Vector2> tilePositions; 
	public List<Vector2> borderTilePosition;
	public List<Vector2> allTilePositions;

	public Room (Vector2 lowerLeftCorner,int width,int height){
		this.lowerLeftCorner = lowerLeftCorner;
		this.width = width;
		this.height = height;
		tilePositions = new List<Vector2>();
		borderTilePosition = new List<Vector2>();
		allTilePositions = new List<Vector2>();


		int x = (int)lowerLeftCorner.x;
		int y = (int)lowerLeftCorner.y;
		for (int i = y; i < y+ height + 2; i++) {
			for (int j = x; j < x+width + 2; j++) {
				if((x == j)||(x +width +1== j)||(y == i)||(y +height +1== i)){
					borderTilePosition.Add(new Vector2(j,i));
				}else
					tilePositions.Add(new Vector2(j,i));
			}

		}
		allTilePositions = new List<Vector2>(borderTilePosition);
		allTilePositions.AddRange(tilePositions);

	}
	public bool Intersects(Room other){

		List<Vector2> firstNotSecond = allTilePositions.Except(other.tilePositions).ToList();
		if(firstNotSecond.Count == allTilePositions.Count){
			return false;
		}
		return true;


	}
}
