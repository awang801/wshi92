using UnityEngine;
using System.Collections;

public class Lives:MonoBehaviour{
    private int lives = 30;

    public void setLives(int amount)
    {
        lives = amount;
    }
    public void loseLife()
    {
        lives = lives - 1;
    }

	public void addLives(int amount)
    {
        lives = lives + amount;
    }

    public void loseLives(int amount)
    {
        lives = lives - amount;
    }

    public int getLives()
    {
        return lives;
    }
}
