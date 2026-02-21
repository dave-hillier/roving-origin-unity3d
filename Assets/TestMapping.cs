using UnityEngine;

public class TestMapping : MonoBehaviour {

	public Vector3g StartPosition;
	public Vector3g MyGlobalPosition;
	public Vector3g FetchedGlobalPosition;
	public string CurrentGlobalPosition;
	public string GlobalFromTransform;
	public bool Move = true;

	void Start () {
		Debug.Log ("Test mapping cube start");
	}

	void Update () {
		if (Move) {
			MyGlobalPosition.x = StartPosition.x + (decimal)Mathf.Sin ((float)StartPosition.z * Time.fixedTime / 5.0f) * 2.5M;
			MyGlobalPosition.z = StartPosition.z + (decimal)Mathf.Cos ((float)StartPosition.z * Time.fixedTime / 5.0f) * 2.5M;
			gameObject.transform.position = MyGlobalPosition;
		}

		// Verify round-trip: convert the transform's local position back to global coordinates.
		// Comparing CurrentGlobalPosition and GlobalFromTransform in the inspector shows
		// whether precision is being maintained through the conversion pipeline.
		FetchedGlobalPosition = Vector3g.FromLocalVector3(gameObject.transform.position);

		CurrentGlobalPosition = MyGlobalPosition.ToString ();
		GlobalFromTransform = FetchedGlobalPosition.ToString();
	}
}
