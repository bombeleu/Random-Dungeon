using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class Level
    {
        public int roomCount = 20;
        public int roomExtraSize = 2;
		public int windingPercent = 20;
		public IntVector2 levelSize = new IntVector2(65,65);
    }
}
