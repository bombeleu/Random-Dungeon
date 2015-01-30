using UnityEngine;
using System.Collections;

public class Stage  {
	
	public IntVector2 size;
	
	public TileType[,] tileType;
	
	public Stage(IntVector2 size){
		this.size = size;
		tileType = new TileType[size.x,size.y];
	}
	public void FillStage(TileType type){
		for (int j = 0; j < size.y; j++) {
			for (int i = 0; i < size.x; i++) {
				tileType[j,i]= type;
			}
		}
	}
	public void AddRoom(Room room){
		for (int i = 0; i < room.tilePositions.Count; i++) {
			SetTile(room.tilePositions[i],TileType.Ground);
		}
	}
	public void SetTile(IntVector2 pos,TileType type){
		tileType[pos.x,pos.y]=type;
	}

	public TileType GetTile(IntVector2 pos){
		return tileType[pos.x,pos.y];
	}
	
}
