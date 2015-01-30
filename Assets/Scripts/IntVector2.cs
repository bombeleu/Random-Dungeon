using UnityEngine;

public struct IntVector2 {
	public int x, y;
	public float magnitude;
	public float lengthSqrt;
	
	public IntVector2(int x, int y) {
		this.x = x;
		this.y = y;
		this.lengthSqrt = x*x+y*y;
		this.magnitude = Mathf.Sqrt(lengthSqrt);

	}
	
	public static IntVector2 operator +(IntVector2 a, IntVector2 b) {
		return new IntVector2(a.x + b.x, a.y + b.y);
	}
	
	public static IntVector2 operator +(IntVector2 a, Vector2 b) {
		return new IntVector2(a.x + (int)b.x, a.y + (int)b.y);
	}
	
	public static IntVector2 operator -(IntVector2 a, IntVector2 b) {
		return new IntVector2(a.x - b.x, a.y - b.y);
	}
	
	public static IntVector2 operator -(IntVector2 a, Vector2 b) {
		return new IntVector2(a.x - (int)b.x, a.y - (int)b.y);
	}
	
	public static IntVector2 operator *(IntVector2 a, int b) {
		return new IntVector2(a.x * b, a.y * b);
	}
	static public implicit operator Vector3(IntVector2 a) 
	{
		return new Vector3(a.x,a.y,0);
	}
	public static bool operator <(IntVector2 a, int b) {

			return a.lengthSqrt < b * b;
	}
	public static bool operator >(IntVector2 a, int b) {
		
		return a.lengthSqrt > b * b;
	}
}

