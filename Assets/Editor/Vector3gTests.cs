using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class Vector3gTests
{
	[SetUp]
	public void SetUp()
	{
		CoordinateRemap.LocalOrigin = Vector3g.Zero;
		CoordinateRemap.DesiredLocalOrigin = Vector3g.Zero;
	}

	// Construction

	[Test]
	public void Constructor_SetsComponents()
	{
		var v = new Vector3g(1.5m, 2.5m, 3.5m);
		Assert.AreEqual(1.5m, v.x);
		Assert.AreEqual(2.5m, v.y);
		Assert.AreEqual(3.5m, v.z);
	}

	[Test]
	public void Zero_IsAllZeros()
	{
		Assert.AreEqual(0m, Vector3g.Zero.x);
		Assert.AreEqual(0m, Vector3g.Zero.y);
		Assert.AreEqual(0m, Vector3g.Zero.z);
	}

	// Arithmetic operators

	[Test]
	public void Addition()
	{
		var a = new Vector3g(1, 2, 3);
		var b = new Vector3g(4, 5, 6);
		var result = a + b;
		Assert.AreEqual(new Vector3g(5, 7, 9), result);
	}

	[Test]
	public void Subtraction()
	{
		var a = new Vector3g(4, 5, 6);
		var b = new Vector3g(1, 2, 3);
		var result = a - b;
		Assert.AreEqual(new Vector3g(3, 3, 3), result);
	}

	[Test]
	public void UnaryNegation()
	{
		var v = new Vector3g(1, -2, 3);
		var result = -v;
		Assert.AreEqual(new Vector3g(-1, 2, -3), result);
	}

	[Test]
	public void ScalarMultiplication()
	{
		var v = new Vector3g(1, 2, 3);
		var result = v * 2.5m;
		Assert.AreEqual(new Vector3g(2.5m, 5m, 7.5m), result);
	}

	[Test]
	public void ScalarMultiplication_IsCommutative()
	{
		var v = new Vector3g(1, 2, 3);
		Assert.AreEqual(v * 2m, 2m * v);
	}

	// Equality

	[Test]
	public void Equality_SameValues()
	{
		var a = new Vector3g(1, 2, 3);
		var b = new Vector3g(1, 2, 3);
		Assert.IsTrue(a == b);
		Assert.IsFalse(a != b);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(a.Equals((object)b));
	}

	[Test]
	public void Equality_DifferentValues()
	{
		var a = new Vector3g(1, 2, 3);
		var b = new Vector3g(1, 2, 4);
		Assert.IsFalse(a == b);
		Assert.IsTrue(a != b);
		Assert.IsFalse(a.Equals(b));
	}

	[Test]
	public void Equality_NotEqualToNull()
	{
		var v = new Vector3g(1, 2, 3);
		Assert.IsFalse(v.Equals(null));
	}

	[Test]
	public void Equality_NotEqualToDifferentType()
	{
		var v = new Vector3g(1, 2, 3);
		Assert.IsFalse(v.Equals("not a vector"));
	}

	[Test]
	public void GetHashCode_SameForEqualValues()
	{
		var a = new Vector3g(1, 2, 3);
		var b = new Vector3g(1, 2, 3);
		Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
	}

	[Test]
	public void GetHashCode_DiffersForDifferentValues()
	{
		var a = new Vector3g(1, 2, 3);
		var b = new Vector3g(3, 2, 1);
		Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
	}

	// ToString

	[Test]
	public void ToString_FormatsCorrectly()
	{
		var v = new Vector3g(1.5m, 2.5m, 3.5m);
		Assert.AreEqual("(1.5, 2.5, 3.5)", v.ToString());
	}

	// Conversion — origin-independent

	[Test]
	public void ToVector3_IgnoresOriginOffset()
	{
		CoordinateRemap.LocalOrigin = new Vector3g(100, 200, 300);
		var v = new Vector3g(1, 2, 3);
		var result = v.ToVector3();
		Assert.AreEqual(1f, result.x, 0.001f);
		Assert.AreEqual(2f, result.y, 0.001f);
		Assert.AreEqual(3f, result.z, 0.001f);
	}

	// Conversion — origin-dependent

	[Test]
	public void ToLocalVector3_AppliesOriginOffset()
	{
		CoordinateRemap.LocalOrigin = new Vector3g(-10, -20, -30);
		var v = new Vector3g(15, 25, 35);
		var result = v.ToLocalVector3();
		Assert.AreEqual(5f, result.x, 0.001f);
		Assert.AreEqual(5f, result.y, 0.001f);
		Assert.AreEqual(5f, result.z, 0.001f);
	}

	[Test]
	public void FromLocalVector3_RemovesOriginOffset()
	{
		CoordinateRemap.LocalOrigin = new Vector3g(-10, -20, -30);
		var local = new Vector3(5, 5, 5);
		var result = Vector3g.FromLocalVector3(local);
		Assert.AreEqual(15m, result.x);
		Assert.AreEqual(25m, result.y);
		Assert.AreEqual(35m, result.z);
	}

	[Test]
	public void ImplicitConversion_RoundTrip_PreservesValue()
	{
		CoordinateRemap.LocalOrigin = new Vector3g(-100, 0, -100);
		var original = new Vector3g(105.5m, 0, 105.5m);

		Vector3 local = original;       // implicit Vector3g → Vector3
		Vector3g recovered = local;     // implicit Vector3 → Vector3g

		// Float precision limits exact equality, but values should be close
		Assert.AreEqual((double)original.x, (double)recovered.x, 0.01);
		Assert.AreEqual((double)original.y, (double)recovered.y, 0.01);
		Assert.AreEqual((double)original.z, (double)recovered.z, 0.01);
	}

	// Precision

	[Test]
	public void DecimalPrecision_MaintainedAtLargeCoordinates()
	{
		// Demonstrates that Vector3g maintains precision that would be lost in float.
		// float can only represent ~7 significant digits; these values require more.
		var farPosition = new Vector3g(1000000.001m, 0, 1000000.002m);
		var nearbyOffset = new Vector3g(0.001m, 0, 0.001m);
		var result = farPosition + nearbyOffset;
		Assert.AreEqual(1000000.002m, result.x);
		Assert.AreEqual(1000000.003m, result.z);
	}

	[Test]
	public void SubtractionNearOrigin_ProducesCorrectOffset()
	{
		var a = new Vector3g(1000000, 0, 1000000);
		var b = new Vector3g(999999, 0, 999999);
		var offset = a - b;
		Assert.AreEqual(1m, offset.x);
		Assert.AreEqual(1m, offset.z);
	}
}
