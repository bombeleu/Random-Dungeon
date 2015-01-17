using UnityEngine;
using System.Collections;

public class Stage  {
	
	public Vector2 size;
	
	public TileType[,] tileType;
	
	public Stage(Vector2 size){
		this.size = size;
		tileType = new TileType[(int)size.x,(int)size.y];
	}
	public void FillStage(){
		for (int j = 0; j < size.y; j++) {
			for (int i = 0; i < size.x; i++) {
				tileType[j,i]= TileType.Wall;
			}
		}
	}
	public void AddRoom(Room room){
		Debug.Log(room.position);
		for (int i = (int)room.position.y; i < (int)room.position.y+room.height; i++) {
			for (int j = (int)room.position.x; j < (int)room.position.x+room.width; j++) {
				tileType[i,j] = TileType.Ground;
			}
		}
		
		
	}
	
}
