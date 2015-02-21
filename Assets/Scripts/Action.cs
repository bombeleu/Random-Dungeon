
using System.Collections;
using RandomDungeon;
public abstract class Action {
	public Actor actor;
	public Stage stage;
	GameResult gameResult;
	public Action(Actor actor,Stage stage){
		this.actor = actor;
		this.stage = stage;
	}

	
	public void bind(Actor actor) {
		this.actor = actor;
		this.stage = actor.stage;
	}
	
	public ActionResult perform(GameResult gameResult) {

		this.gameResult = gameResult;
		return onPerform();
	}
	
	public abstract ActionResult onPerform();
	
	public void addEvent(RandomDungeon.Event e) {
		gameResult.events.Add(e);
	}
	
	public ActionResult alternate(Action action) {
		action.bind(actor);
		return new ActionResult(action);
	}
}

public class ActionResult {
	 public static ActionResult SUCCESS = new ActionResult(succeeded: true);
	 public static ActionResult FAILURE = new ActionResult(succeeded: false);
	
	/// An alternate [Action] that should be performed instead of the one that
	/// failed to perform and returned this. For example, when the [Hero] walks
	/// into a closed door, the [WalkAction] will fail (the door is closed) and
	/// return an alternate [OpenDoorAction] instead.
	public Action alternative;
	
	/// `true` if the [Action] was successful and energy should be consumed.
	public bool succeeded;
	
	public ActionResult(bool succeeded){
		this.succeeded = succeeded;
		alternative = null;
	}
		
	
	public ActionResult(Action alternative){
		this.succeeded = false;
		this.alternative = alternative;
	}
}

public class WalkAction : Action {
	public IntVector2 offset;
	
	public WalkAction(Actor actor, Stage stage,IntVector2 offset) : base(actor,stage){
		this.offset = offset;
	}
	
	public override ActionResult onPerform() {
		IntVector2 pos = actor.pos + offset;

		if ( stage.actors.ContainsKey(pos) ){
			Actor target = stage.actors[pos];
			if (target != null && target != actor) {
				return alternate(new AttackAction(actor,stage,target));
			}
		}
		// See if we can walk there.
		TileType tile = stage.GetTile(pos);
		switch (tile) {
		case TileType.Wall:
			addEvent(new Event("bonk", pos));
			return ActionResult.FAILURE;
			
		case TileType.ClosedDoor:
			return alternate(new OpenDoorAction(actor,stage,pos));
		}
		
		actor.pos = pos;
		return ActionResult.SUCCESS;
	}
}

/// [Action] for a melee attack from one [Actor] to another.
public class AttackAction : Action {
	public Actor defender;
	
	public AttackAction(Actor actor, Stage stage,Actor defender) : base(actor,stage){
		this.defender = defender;
	}

	public override ActionResult onPerform() {
		addEvent(new Event("hit", defender.pos));
		
		if (defender is Monster) {
			defender.pos = stage.findOpenTile();
		}
		
		return ActionResult.SUCCESS;
	}
}

public class TeleportAction : Action {
	public TeleportAction(Actor actor, Stage stage) : base(actor,stage){}
	
	public override ActionResult onPerform() {
		actor.pos = stage.findOpenTile();
		return ActionResult.SUCCESS;
	}
}

public class OpenDoorAction : Action {
	public IntVector2 doorPos;
	
	public OpenDoorAction(Actor actor, Stage stage,IntVector2 doorPos) : base(actor,stage){
		this.doorPos = doorPos;
	}
	public override ActionResult onPerform() {
		stage.SetTile(doorPos,TileType.OpenDoor);
		return ActionResult.SUCCESS;
	}
}

public class CloseDoorAction : Action {

	public CloseDoorAction(Actor actor, Stage stage) : base(actor,stage){}

	public override ActionResult onPerform() {
		foreach (IntVector2 dir in Direction.CARDINAL) {
			if (stage.GetTile(actor.pos + dir) == TileType.OpenDoor) {
				stage.SetTile(actor.pos + dir, TileType.ClosedDoor);
			}
		}
		return ActionResult.SUCCESS;
	}
}