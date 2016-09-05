using UnityEngine;
using System.Collections;

public class RangeIndicatorRotate : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		this.transform.Rotate (new Vector3 (0f, (30f * Time.deltaTime), 0f));
	}
}
