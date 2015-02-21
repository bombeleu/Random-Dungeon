public class Breed {
	/// Untyped so the engine isn't coupled to how monsters appear.
	public string appearance;
	
	/// The breed's speed, relative to normal. Ranges from `-6` (slowest) to `6`
	/// (finalfastest) where `0` is normal speed.
	public int speed;
	
	public bool canOpenDoors;
	
	public Breed(string appearance,int speed,bool canOpenDoors){
		this.appearance = appearance;
		this.speed = speed;
		this.canOpenDoors = canOpenDoors;
	}

		
}
