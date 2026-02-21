using UnityEngine;
using System;

public struct Vector3g : IEquatable<Vector3g>
{
	public decimal x;
	public decimal y;
	public decimal z;

	public Vector3g(decimal x, decimal y, decimal z) {
		this.x = x; this.y = y; this.z = z;
	}

	public static readonly Vector3g Zero = new Vector3g(0, 0, 0);

	// Arithmetic operators

	public static Vector3g operator +(Vector3g a, Vector3g b) {
		return new Vector3g(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vector3g operator -(Vector3g a, Vector3g b) {
		return new Vector3g(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Vector3g operator -(Vector3g a) {
		return new Vector3g(-a.x, -a.y, -a.z);
	}

	public static Vector3g operator *(Vector3g a, decimal scalar) {
		return new Vector3g(a.x * scalar, a.y * scalar, a.z * scalar);
	}

	public static Vector3g operator *(decimal scalar, Vector3g a) {
		return a * scalar;
	}

	// Equality

	public static bool operator ==(Vector3g a, Vector3g b) {
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator !=(Vector3g a, Vector3g b) {
		return !(a == b);
	}

	public bool Equals(Vector3g other) {
		return this == other;
	}

	public override bool Equals(object obj) {
		return obj is Vector3g && Equals((Vector3g)obj);
	}

	public override int GetHashCode() {
		unchecked {
			int hash = 17;
			hash = hash * 31 + x.GetHashCode();
			hash = hash * 31 + y.GetHashCode();
			hash = hash * 31 + z.GetHashCode();
			return hash;
		}
	}

	/// <summary>
	/// Converts to a Unity Vector3 without applying any origin offset.
	/// Use this when you need the raw decimal values as floats.
	/// </summary>
	public Vector3 ToVector3() {
		return new Vector3((float)x, (float)y, (float)z);
	}

	/// <summary>
	/// Converts a global position to a local Unity Vector3 by applying the current origin offset.
	/// This makes the dependency on CoordinateRemap.LocalOrigin explicit.
	/// </summary>
	public Vector3 ToLocalVector3() {
		return new Vector3(
			(float)(x + CoordinateRemap.LocalOrigin.x),
			(float)(y + CoordinateRemap.LocalOrigin.y),
			(float)(z + CoordinateRemap.LocalOrigin.z));
	}

	/// <summary>
	/// Converts a local Unity Vector3 to a global position by removing the current origin offset.
	/// This makes the dependency on CoordinateRemap.LocalOrigin explicit.
	/// </summary>
	public static Vector3g FromLocalVector3(Vector3 vec) {
		return new Vector3g(
			(decimal)vec.x - CoordinateRemap.LocalOrigin.x,
			(decimal)vec.y - CoordinateRemap.LocalOrigin.y,
			(decimal)vec.z - CoordinateRemap.LocalOrigin.z);
	}

	/// <summary>
	/// Implicit conversion to Unity Vector3. Applies CoordinateRemap.LocalOrigin offset.
	/// Note: Result depends on the current value of CoordinateRemap.LocalOrigin.
	/// For origin-independent conversion, use ToVector3() instead.
	/// </summary>
	public static implicit operator Vector3(Vector3g vec) {
		return vec.ToLocalVector3();
	}

	/// <summary>
	/// Implicit conversion from Unity Vector3. Removes CoordinateRemap.LocalOrigin offset.
	/// Note: Result depends on the current value of CoordinateRemap.LocalOrigin.
	/// For origin-independent conversion, use the Vector3g constructor directly.
	/// </summary>
	public static implicit operator Vector3g(Vector3 vec) {
		return FromLocalVector3(vec);
	}

	public override string ToString() {
		return string.Format("({0}, {1}, {2})", x, y, z);
	}
}
