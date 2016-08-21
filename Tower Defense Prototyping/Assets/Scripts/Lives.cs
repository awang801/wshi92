using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Lives:MonoBehaviour{
    private int lives = 30;
	public Text livesText;

	void Start()
	{
		updateText ();
	}

    public void setLives(int amount)
    {
        lives = amount;
		updateText ();
    }
    public void loseLife()
    {
        lives = lives - 1;
		updateText ();
    }

	public void addLives(int amount)
    {
        lives = lives + amount;
		updateText ();
    }

    public void loseLives(int amount)
    {
        lives = lives - amount;
		updateText ();
    }

    public int getLives()
    {
        return lives;
    }

	void updateText()
	{
		if (livesText != null) {
			livesText.text = "Lives: " + lives;
		}
	}
}
