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
				tileType[j,i]= TileType.Empty;
			}
		}
	}
	public void AddRoom(Room room){

		for (int i = 0; i < room.tilePositions.Count; i++) {
			Vector2 pos = (Vector2)room.tilePositions[i];
			tileType[(int)pos.x,(int)pos.y]= TileType.Test;
				}
		for (int j = 0; j < room.borderTilePosition.Count; j++) {
			Vector2 pos = (Vector2)room.borderTilePosition[j];
			tileType[(int)pos.x,(int)pos.y]= TileType.Test;
		}
		
		
	}
	
}
