using UnityEngine;
using System.Collections;

public class Lives : MonoBehaviour {
    private int lives;
    // Use this for initialization
    void Start()
    {
        lives = 30;
    }

    void loseLife()
    {
        lives = lives - 1;
    }

	void addLives(int amount)
    {
        lives = lives + amount;
    }

    void loseLives(int amount)
    {
        lives = lives - amount;
    }

    int getLives()
    {
        return lives;
    }
	// Update is called once per frame
	void Update () {
	
	}
}
