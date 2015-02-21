using System;
namespace RandomDungeon
{
public class Event {
		public string type;
		public IntVector2 pos;
		
		public Event(string type,IntVector2 pos){
			this.type = type;
			this.pos = pos;
		}
	}

}
