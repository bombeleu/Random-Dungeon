using System;
using UnityEngine;
public class Monster : Actor{

	public Breed breed;
	public string appearance{get{return breed.appearance;}}
	public override int speed{get{return Energy.NORMAL_SPEED + breed.speed;}}
	public Monster (Stage stage, IntVector2 pos,Breed breed):base(stage,pos){
		this.breed = breed;
	}
	public override Action onGetAction(){
		IntVector2 walkDir = findMeleePath();
		if(walkDir == new IntVector2(9999,9999)) walkDir = Direction.NONE;
		return new WalkAction(this,stage,walkDir);
	}
	public IntVector2 findMeleePath(){
		Debug.Log("TO DO");
		return Direction.NONE;
	}
}


