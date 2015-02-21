using System;
using System.Collections.Generic;
using RandomDungeon;
/// Each call to [Game.update()] will return a [GameResult] object that tells
/// the UI what happened during that update and what it needs to do.
public class GameResult {
	/// The "interesting" events that occurred in this update.
	public List<Event> events;
	
	/// Whether or not any game state has changed. If this is `false`, then no
	/// game processing has occurred (i.e. the game is stuck waiting for user
	/// input for the [Hero]).
	public bool madeProgress = false;
	
	/// Returns `true` if the game state has progressed to the point that a change
	/// should be shown to the user.
	public bool needsRefresh {
		get {
			return madeProgress || events.Count > 0;
		}
	}
	public GameResult(){
		events = new List<Event> ();
	}
	
}