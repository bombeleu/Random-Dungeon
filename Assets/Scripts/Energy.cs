using UnityEngine;
using System.Collections;
using System.Linq;

public class Energy {

	public static int MIN_SPEED    = 0;
	public static int NORMAL_SPEED = 3;
	public static int MAX_SPEED    = 5;
	
	public static int ACTION_COST = 12;

	public static int[] GAINS =  {
	                            2,     // 1/3 normal speed
	                            3,     // 1/2
	                            4,
	                            6,     // normal speed
	                            9,
	                            12,    // 2x normal speed
	};

	public static float ticksAtSpeed(int speed) {
		return ACTION_COST / GAINS[NORMAL_SPEED + speed];
	}

	public int energy = 0;

	public bool canTakeTurn {
		get
		{
			return energy >= ACTION_COST;
		}
	}


	public bool gain(int speed) {
		energy += GAINS[speed];
		return canTakeTurn;
	}
	
	/// Spends a turn's worth of energy.
	public void spend() {
		if(energy >= ACTION_COST)
			energy = energy % ACTION_COST;
	}
}
