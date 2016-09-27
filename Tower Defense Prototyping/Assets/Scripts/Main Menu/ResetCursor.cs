using UnityEngine;
using System.Collections;

public class ResetCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
		Cursor.visible = true;
	}
}
