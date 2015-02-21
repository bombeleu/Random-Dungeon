using System;
public class Hero : Actor
{
	public Action nextAction;
	
	public string appearance = "hero";
	

	public override bool needsInput{get{
			if(nextAction == null)
				return true;
			else
				return false;
		}}
	public override Action onGetAction() {
		Action action = nextAction;
		nextAction = null;
		return action;
	}
	
	public void setNextAction(Action action) {
		nextAction = action;
	}

	public Hero (Stage stage, IntVector2 pos): base(stage,pos){}

}


