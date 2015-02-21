using System;
using System.Collections.Generic;
using System.Linq;
/*
public class PathFindig
{
	public PathFindig ()
	{
	}
}
public class PathResult {
	/// The direction to move on the first step of the path.
	public Direction direction;
	
	/// The total number of steps in the path.
	public int length;
	
	public PathResult(Direction direction,int length){
		direction = this.direction;
		length = this.length;
	}
}

/// A* pathfinding algorithm.
public class AStar {
	/// When calculating pathfinding, how much it "costs" to move one step on
	/// an open floor tile.
	public static int FLOOR_COST = 10;
	
	/// When calculating pathfinding, how much it costs to move one step on a
	/// tile already occupied by an actor. For pathfinding, we consider occupied
	/// tiles as accessible but expensive. The idea is that by the time the
	/// pathfinding monster gets there, the occupier may have moved, so the tile
	/// is "sorta" empty, but still not as desirable as an actually empty tile.
	public static int OCCUPIED_COST = 40;
	
	/// When calculating pathfinding, how much it costs cross a currently-closed
	/// door. Instead of considering them completely impassable, we just have them
	/// be expensive, because it still may be beneficial for the monster to get
	/// closer to the door (for when the hero opens it later).
	public static int DOOR_COST = 80;
	
	/// When applying the pathfinding heuristic, straight steps (NSEW) are
	/// considered a little cheaper than diagonal ones so that straighter paths
	/// are preferred over equivalent but uglier zig-zagging ones.
	public static int STRAIGHT_COST = 9;
	
	/// Tries to find a path from [start] to [end], searching up to [maxLength]
	/// steps from [start]. Returns the [Direction] of the first step from [start]
	/// along that path (or [Direction.NONE] if it determines there is no path
	/// possible.
	public static Direction findDirection(Stage stage, IntVector2 start, IntVector2 end, int maxLength) {
		PathNode path = _findPath(stage, start, end, maxLength);
		if (path == null) return Direction.NONE;
		
		while (path.parent != null && path.parent.parent != null) {
			path = path.parent;
		}
		
		return path.direction;
	}

	public static PathResult findPath(Stage stage, IntVector2 start, IntVector2 end, int maxLength) {
		PathNode path = findPath(stage, start, end, maxLength);
		if (path == null) return new PathResult(Direction.NONE, 0);
		
		int length = 1;
		while (path.parent != null && path.parent.parent != null) {
			path = path.parent;
			length++;
		}
		
		return new PathResult(path.direction, length);
	}
	
	public static PathNode _findPath(Stage stage, IntVector2 start, IntVector2 end, int maxLength) {
		// TODO: More optimal data structure.
		PathNode startPath = new PathNode(null, Direction.NONE,
		                              start, 0, heuristic(start, end));
		HashSet<PathNode> open = new HashSet<PathNode>();
		open.Add(startPath);
		HashSet<IntVector2> closed = new HashSet<IntVector2>();
		
		while (open.Count > 0) {
			// Pull out the best potential candidate.
			PathNode current = open.Last;
			open.Remove(open.Last);
			if ((current.pos == end) ||
			    (current.cost > FLOOR_COST * maxLength)) {
				// Found the path.
				return current;
			}

			closed.Add(current.pos);
			
			foreach (Direction dir in Direction.CARDINAL) {
				IntVector2 neighbor = current.pos + dir;
				
				// Skip impassable tiles.
				if (stage.GetTile(neighbor) == TileType.Wall) continue;
				
				// Given how far the current tile is, how far is each neighbor?
				var stepCost = FLOOR_COST;
				if (stage.actors.ContainsKey(neighbor)) {
					stepCost = OCCUPIED_COST;
				}
				
				var cost = current.cost + stepCost;
				
				// See if we just found a better path to a tile we're already
				// considering. If so, remove the old one and replace it (below) with
				// this new better path.
				var inOpen = false;
				
				for (var i = 0; i < open.Count; i++) {
					PathNode alreadyOpen = open[i];
					if (alreadyOpen.pos == neighbor) {
						if (alreadyOpen.cost > cost) {
							open.Remove(i);
							i--;
						} else {
							inOpen = true;
						}
						break;
					}
				}

				bool inClosed = closed.Contains(neighbor);
				
				// TODO: May need to do the above check on the closed set too if
				// we use inadmissable heuristics.
				
				// If we have a new path, add it.
				if (!inOpen && !inClosed) {
					var guess = cost + heuristic(neighbor, end);
					PathNode path = new PathNode(current, dir, neighbor, cost, guess);
					
					// Insert it in sorted order (such that the best node is at the *end*
					// of the list for easy removal).
					bool inserted = false;
					for (var i = open.Count - 1; i >= 0; i--) {
						PathNode node = open[i];
						if (node.guess > guess) {
							//open.insert(i + 1, path);
							inserted = true;
							break;
						}
					}
					
					// If we didn't find a node to put it after, put it at the front.
					//if (!inserted) open.insert(0, path);
				}
			}
		}
		
		// No path.
		return null;
	}
	
	/// The estimated cost from [pos] to [end].
	public static int heuristic(IntVector2 pos, IntVector2 end) {
		// A simple heuristic would just be the kingLength. The problem is that
		// diagonal moves are as "fast" as straight ones, which means many
		// zig-zagging paths are as good as one that looks "straight" to the player.
		// But they look wrong. To avoid this, we will estimate straight steps to
		// be a little cheaper than diagonal ones. This avoids paths like:
		//
		// ...*...
		// s.*.*.g
		// .*...*.
		/*
		int offset = (end - pos).abs();
		int numDiagonal = Math.min(offset.x, offset.y);
		numStraight = math.max(offset.x, offset.y) - numDiagonal;
		return (numDiagonal * FLOOR_COST) +	(numStraight * STRAIGHT_COST);
		*//*
	}
}
/*
public class PathNode {
	public PathNode parent;
	public Direction direction;
	public IntVector2 pos;
	
	/// The cost to get to this node from the starting point. This is roughly the
	/// distance, but may be a little different if we start weighting tiles in
	/// interesting ways (i.e. make it more expensive for light-abhorring
	/// monsters to walk through lit tiles).
	public int cost;
	
	/// The guess as to the total cost from the start node to the end node going
	/// along this path. In other words, this is [cost] plus the heuristic.
	public int guess;
	
	PathNode(PathNode parent,Direction direction,IntVector2 pos,int cost,int guess){
		this.parent = parent;
		this.direction = direction;
		this.pos = pos;
		this.cost = cost;
		this.guess = guess;
	}
}
*/