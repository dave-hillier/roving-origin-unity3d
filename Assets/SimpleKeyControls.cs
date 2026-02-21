using UnityEngine;

public class SimpleKeyControls : MonoBehaviour {

	public Vector3g MyGlobalPosition;

	void Start () {
	}

	void Update () {
		MyGlobalPosition.x += (decimal)(Input.GetAxis ("Horizontal") * 0.1f);
		MyGlobalPosition.z += (decimal)(Input.GetAxis ("Vertical") * 0.1f);

		if (Input.GetKey (KeyCode.Space))
			MyGlobalPosition.y += 0.1m;
		if (Input.GetKey (KeyCode.LeftShift))
			MyGlobalPosition.y -= 0.1m;

		transform.position = MyGlobalPosition;

		float max = 3.0f;

		if (System.Math.Abs (transform.position.x) > max) {
			CoordinateRemap.DesiredLocalOrigin = new Vector3g(
				CoordinateRemap.DesiredLocalOrigin.x - (decimal)transform.position.x,
				CoordinateRemap.DesiredLocalOrigin.y,
				CoordinateRemap.DesiredLocalOrigin.z);
		}
		if (System.Math.Abs (transform.position.y) > max) {
			CoordinateRemap.DesiredLocalOrigin = new Vector3g(
				CoordinateRemap.DesiredLocalOrigin.x,
				CoordinateRemap.DesiredLocalOrigin.y - (decimal)transform.position.y,
				CoordinateRemap.DesiredLocalOrigin.z);
		}
		if (System.Math.Abs (transform.position.z) > max) {
			CoordinateRemap.DesiredLocalOrigin = new Vector3g(
				CoordinateRemap.DesiredLocalOrigin.x,
				CoordinateRemap.DesiredLocalOrigin.y,
				CoordinateRemap.DesiredLocalOrigin.z - (decimal)transform.position.z);
		}
	}
}
