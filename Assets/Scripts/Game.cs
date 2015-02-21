using RandomDungeon;
using System.Collections;
using UnityEngine;
using System.Linq;
public class Game : MonoBehaviour {
	public InputController input;
	public Stage stage;
	public int currentActorIndex = 0;
	public Actor[] actors;
	public Actor currentActor{get{return actors[currentActorIndex];}}
	public bool spendEnergyOnFailure = true;
	public bool stopAfterEveryProcess = false;
	public Transform heroTrans;
	public bool turn = true;
	public Dungeon dungeon;
	public GameBuilder gameBuilder;
	// Use this for initialization
	void Start () {
		if (input != null){
			input.OnKeydown += OnKeydown;
		}
		dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<Dungeon>();
		stage = dungeon.stage;
		//heroTrans.transform.position = new Vector2 (stage.hero.pos.x * dungeon.tileSize.x/100,stage.hero.pos.x * dungeon.tileSize.x/100);
	}
	void Update(){
		dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<Dungeon>();
		stage = dungeon.stage;
	}
	IEnumerator Reset(){
		yield return new WaitForSeconds(1);
		turn = true;
	}

	public void OnKeydown(IntVector2 dir)
	{
		if(!turn)return;

		turn =false;

		StartCoroutine(Reset());
		print (stage.actors.Count);
		Action next = null;
		if(dir.Equals(Direction.NORTH)){
			next = new WalkAction(stage.hero,stage,Direction.NORTH);
		}else if(dir.Equals(Direction.EAST)){
			next = new WalkAction(stage.hero,stage,Direction.EAST);
		}else if(dir.Equals(Direction.SOUTH)){
			next = new WalkAction(stage.hero,stage,Direction.SOUTH);
		}else if(dir.Equals(Direction.WEST)){
			next = new WalkAction(stage.hero,stage,Direction.WEST);
		}

		if (next != null) {
			stage.hero.setNextAction(next);
		}
		TUpdate();
		gameBuilder.Display();
		//heroTrans.transform.position  = new Vector2 (stage.hero.pos.x * dungeon.tileSize.x/100,stage.hero.pos.x * dungeon.tileSize.x/100);
	}
	public void TUpdate(){
		GameResult result = RUpdate();

		foreach (RandomDungeon.Event e in result.events) {
			switch (e.type) {
			case "hit":
				Debug.Log("Bounce");
				break;
			case "bonk":
				Debug.Log("Bounce");
				break;
			}
		}
	}

	public GameResult RUpdate(){
		GameResult gameResult = new GameResult();
		actors = stage.actors.Values.ToArray(); 
		while(true){
			Actor actor = currentActor;
			print (actor.name);
			if (actor.energy.canTakeTurn && actor.needsInput) return gameResult;

			gameResult.madeProgress = true;

			Action action = null;
			while(action == null){
				Actor actor2 = currentActor;

				if (actor2.energy.canTakeTurn || actor2.energy.gain(actor2.speed)) {
					// If the actor can move now, but needs input from the user, just
					// return so we can wait for it.
					if (actor2.needsInput) return gameResult;
					
					action = actor2.getAction();
				} else {
					// This actor doesn't have enough energy yet, so move on to the next.
					advanceActor();
					
					if (stopAfterEveryProcess) return gameResult;
				}
			}

			ActionResult result = action.perform(gameResult);
			while (result.alternative != null) {
				action = result.alternative;
				result = action.perform(gameResult);
			}
			
			if (spendEnergyOnFailure || result.succeeded) {
				action.actor.finishTurn(action);
				advanceActor();
			}
		}

	}
	void advanceActor() {
		currentActorIndex = (currentActorIndex + 1) % actors.Count();
	}
	void North()
	{
		Debug.Log ("NORTH");
	}

	void East()
	{
		Debug.Log ("EAST");
	}
	void South(){
		Debug.Log ("SOUTH");
	}
	void West()
	{
		Debug.Log ("WEST");
	}



		
		

}


