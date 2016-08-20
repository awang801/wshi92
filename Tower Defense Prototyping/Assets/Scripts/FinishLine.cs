using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {
    Lives lives;
	// Use this for initialization
	void Start () {
        lives = GameObject.Find("GameManager").GetComponent<Lives>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Enemy")) {
            other.gameObject.GetComponent<Unit>().Finish();
            lives.loseLife();
            Debug.Log(lives.getLives());
        } 
	}
}
