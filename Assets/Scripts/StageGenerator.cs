
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public static class Extensions
	{
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		}
	}
	public class StageGenerator
		{
				private Level level;
				private Stage stage;
				private IntVector2 stageSize;

				private int windingPercent;
				private int numRooms;
				private int roomExtraSize;
				
				private int currentRegion;
				private List<IntVector2> connectors;
				private Dictionary<IntVector2,int> _regions;
			
				private float extraConnectorChance;
				
				public StageGenerator (Level level)
				{
						this.level = level;
				}

				private void reset ()
				{
					currentRegion = -1;
					_regions = new Dictionary<IntVector2,int > ();
					windingPercent = level.windingPercent;
					numRooms = level.roomCount;
					roomExtraSize = level.roomExtraSize;
					extraConnectorChance = level.extraConnectorChance;
					stageSize = level.levelSize;
					stage = new Stage (stageSize);
					stage.FillStage (TileType.Wall);
					
				}
				
				public Stage GenerateStage ()
				{
					reset ();
					addRooms ();
					findOpenGrowMaze ();
					_connectRegions ();
					removeDeadEnds ();

					

					return stage;
				}

				private void growMaze (IntVector2 start)
				{
						List<IntVector2> cells = new List<IntVector2> ();
						IntVector2 lastDir = new IntVector2 (-10, -1);
			
						startRegion ();
						carve (start, TileType.Ground);
			
						cells.Add (start);
						while (cells.Count != 0) {
								IntVector2 cell = cells [cells.Count - 1];
								List<IntVector2> openDirs = new List<IntVector2> ();
								IntVector2[] directions = Direction.CARDINAL;
								for (int i = 0; i < directions.Length; i++) {
										if (canCarve (cell, directions [i]))
												openDirs.Add (directions [i]);
								}
								if (openDirs.Count != 0) {
										IntVector2 dir;
										if (openDirs.Contains (lastDir) && Random.Range (0, 101) > windingPercent) {
												dir = lastDir;
										} else {
												dir = openDirs [Random.Range (0, openDirs.Count)];
										}
										carve (cell + dir, TileType.Ground);
										carve (cell + dir * 2, TileType.Ground);
										cells.Add (cell + dir * 2);
										lastDir = dir;
								} else {
										cells.RemoveAt (cells.Count - 1);
					
										lastDir = new IntVector2 (-10, -10);
								}
				
						}
			
				}

				
		
				private void addRooms ()
				{
						ArrayList rooms = new ArrayList ();
						for (int i = 0; i < numRooms;) {
								var size = Random.Range (1, 3 + roomExtraSize) * 2 + 1;
								var rectangularity = Random.Range (0, 1 + size / 2) * 2;
								var width = size;
								var height = size;
								if (Random.Range (1, 3) == 2) {
										width += rectangularity;
								} else {
										height += rectangularity;
								}
				
								int x = Random.Range (0, ((int)stage.size.x - width) / 2) * 2 + 1;
								int y = Random.Range (0, ((int)stage.size.y - height) / 2) * 2 + 1;
				
								Room room = new Room (new IntVector2 (x, y), width, height);
				
								bool overlaps = false;
				
								foreach (Room other in rooms) {
										if (room.Intersects (other)) {
												overlaps = true;
												break;
										}
								}
				
				
								if (overlaps)
										continue;
								i++;
								rooms.Add (room);
								startRegion ();
								for (int j = y; j < y+ height; j++) {
										for (int ii = x; ii < x+ width; ii++) {
												carve (new IntVector2 (ii, j), TileType.Ground);
										}
								}
				
				
				
						}
				}
		
				private void findOpenGrowMaze ()
				{
						for (int i = 1; i < stage.size.y; i+=2) {
								for (int j = 1; j < stage.size.x; j+=2) {
										if (stage.tileType [j, i] != TileType.Wall)
												continue;
										growMaze (new IntVector2 (j, i));
								}
						}

				}
		
				private void connectRegions ()
				{
						Dictionary<IntVector2,List<int>> connectorRegions = new Dictionary<IntVector2, List<int>> ();
						for (int i = 1; i < stage.size.y-1; i++) {
								for (int j = 1; j < stage.size.x-1; j++) {
										IntVector2 pos = new IntVector2 (j, i);
										if (stage.GetTile (pos) != TileType.Wall)
												continue;
										List<int> regions = new List<int> ();
					
										foreach (IntVector2 dir in Direction.CARDINAL) {
												IntVector2 region = new IntVector2 (pos.x + dir.x, pos.y + dir.y);
												if (_regions.ContainsKey (region)) {
														if (!regions.Contains (_regions [region])) {
																regions.Add (_regions [region]);
														}
							
												}
						
										}
										if (regions.Count < 2)
												continue;
										regions.Sort ();
										connectorRegions [pos] = regions;
					
								}
						}
						connectors = connectorRegions.Select (x => x.Key).ToList ();
			
						int openRegions = currentRegion;
			
						while (openRegions >= 1) {
								IntVector2 connector = connectors [Random.Range (0, connectors.Count)];
								addJunction (connector);
								openRegions--;
				
								List<IntVector2> source = new List<IntVector2> ();
								for (int i = 0; i < connectors.Count; i++) {
										if (connectorRegions [connector] [0] == connectorRegions [connectors [i]] [0]) {
												source.Add (connectors [i]);
										}
								}
				
								foreach (IntVector2 item in source) {
										if (connectors.Count >= 2)
												connectors.Remove (item);
								}
						}
			
				}

		private void _connectRegions ()
		{
			Dictionary<IntVector2,HashSet<int>> connectorRegions = new Dictionary<IntVector2, HashSet<int>> ();
			for (int i = 1; i < stage.size.y-1; i++) {
				for (int j = 1; j < stage.size.x-1; j++) {
					IntVector2 pos = new IntVector2 (j, i);
					if (stage.GetTile (pos) != TileType.Wall)
						continue;
					HashSet<int> regions = new HashSet<int> ();
					
					foreach (IntVector2 dir in Direction.CARDINAL) {
						IntVector2 region = new IntVector2 (pos.x + dir.x, pos.y + dir.y);
						if (_regions.ContainsKey (region)) {
							if (!regions.Contains (_regions [region])) {
								regions.Add (_regions [region]);
							}
							
						}
						
					}
					if (regions.Count < 2)
						continue;

					connectorRegions [pos] = regions;
					
				}
			}
			connectors = connectorRegions.Select (x => x.Key).ToList ();
			int[] merged = new int[currentRegion+1];
			HashSet<int> openRegions = new HashSet<int>();

			for (int i = 0; i <= currentRegion; i++) {
				merged[i]=i;
				openRegions.Add(i);
						}



			while (openRegions.Count > 1) {
				IntVector2 connector = connectors [Random.Range (0, connectors.Count)];
				addJunction (connector);

				HashSet<int> regions = connectorRegions[connector].Select((region)=>merged[region]).ToHashSet();
				int dest = regions.First();

				List<int> sources = regions.Skip(1).ToList();

				for (int i = 0; i <= currentRegion; i++) {
					if(sources.Contains(merged[i])){
						merged[i] = dest;
					}
				}

				for (int i = 0; i < sources.Count;i++){
					openRegions.Remove(sources[i]);

				}


				for(int ii = 0; ii < connectors.Count; ii++) {

					IntVector2 pos = connectors[ii];

						
						if(connector - pos < 2){
							connectors.Remove(pos);
							continue;
						}

						HashSet<int> regionss = connectorRegions[pos].Select((region) => merged[region]).ToHashSet();

						if(regionss.Count > 1)continue;

						if(Random.Range(0,extraConnectorChance)==extraConnectorChance){ 
							//addJunction(pos);

						}
						connectors.Remove(pos);

				}





			}
			
		}

				
				private void carve (IntVector2 pos, TileType type)
				{
						stage.SetTile (pos, type);
						_regions [pos] = currentRegion;
				}
		
				private bool canCarve (IntVector2 pos, IntVector2 dir)
				{
						if (pos.x + dir.x * 3 > stage.size.x || pos.x + dir.x * 3 < 0 || pos.y + dir.y * 3 > stage.size.y || pos.y + dir.y * 3 < 0)
								return false;
			
						return stage.GetTile (pos + dir * 2) == TileType.Wall;
				}

				private void startRegion ()
				{
						currentRegion++;
				}
		
				private void addJunction (IntVector2 pos)
				{
						if (Random.Range (1, 5) == 1) {
								stage.SetTile (pos, Random.Range (1, 4) == 1 ? TileType.OpenDoor : TileType.Ground);
						} else {
								stage.SetTile (pos, TileType.ClosedDoor);
						}
				}
		
				private void removeDeadEnds ()
				{
						bool done = false;
			
						while (!done) {
								done = true;
				
								for (int i = 1; i < stage.size.y-1; i++) {
										for (int j = 1; j < stage.size.x-1; j++) {
												IntVector2 pos = new IntVector2 (j, i);
												if (stage.GetTile (pos) == TileType.Wall)
														continue;
						
												int exits = 0;
												foreach (IntVector2 dir in Direction.CARDINAL) {
														if (stage.GetTile (pos + dir) != TileType.Wall)
																exits++;
												}
						
												if (exits != 1)
														continue;
												done = false;
												stage.SetTile (pos, TileType.Wall);
										}
								}
						}
				}
		}
}
