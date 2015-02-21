using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {

	public IntVector2 pos;
	public Stage stage;
	public Energy energy = new Energy();

	public virtual bool needsInput {get{return false;}}

	public virtual int speed{get{return Energy.NORMAL_SPEED;}}

	public Actor(Stage stage, IntVector2 pos){
		this.stage = stage;
		this.pos = pos;
	}

	public Action getAction(){
		Action action = onGetAction();
		if(action != null) action.bind(this);
		return action;
	}

	public virtual Action onGetAction(){
		return  null;
	}

	public void finishTurn(Action action){
		energy.spend();
	}

}
