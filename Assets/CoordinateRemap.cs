using UnityEngine;
using System.Collections.Generic;

public class CoordinateRemap : MonoBehaviour {

	public static Vector3g DesiredLocalOrigin;

	public static Vector3g LocalOrigin;

	public GameObject StaticObject;
	public GameObject PositionedObject;

	void Start () {
		Debug.Log ("Coord holder start");
		int size = 100;
		int count = 10;
		for (int x = -size; x < size; x += size/count) {
			for (int z = -size; z < size; z += size/count) {
				GameObject.Instantiate(StaticObject, new Vector3(x,0,z), Quaternion.identity);
			}
		}

		var obj1 = (GameObject)GameObject.Instantiate(PositionedObject, new Vector3(0,0,0), Quaternion.identity);
		var obj2 = (GameObject)GameObject.Instantiate(PositionedObject, new Vector3(0,0,0), Quaternion.identity);
		var obj3 = (GameObject)GameObject.Instantiate(PositionedObject, new Vector3(0,0,0), Quaternion.identity);

		var positioned = new [] { obj1, obj2, obj3 };
		decimal i = 0;
		foreach (var obj in positioned) {
			var positionHolder = obj.GetComponent<TestMapping>();
			positionHolder.StartPosition.z = i + 5;
			i += 10;
		}
	}

	void Update () {

		if (DesiredLocalOrigin != LocalOrigin) {
			var sw = new System.Diagnostics.Stopwatch();
			var offset = (DesiredLocalOrigin - LocalOrigin).ToVector3();

			var allTransforms = FindObjectsOfType<Transform>();
			int rootCount = 0;

			sw.Start();
			foreach (var t in allTransforms)
			{
				// Only translate root objects — children follow their parents automatically.
				// This avoids the double-translation bug where child transforms would be
				// moved both by their parent shifting and by the explicit offset.
				if (t.parent != null)
					continue;

				t.position += offset;
				rootCount++;
			}
			sw.Stop();
			Debug.Log("Translated " + rootCount + " root objects in " + sw.Elapsed.TotalSeconds + "s");
			LocalOrigin = DesiredLocalOrigin;
		}
	}
}
