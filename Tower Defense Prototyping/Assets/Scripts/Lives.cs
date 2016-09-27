using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Lives : NetworkBehaviour{

    private int lives;

	public Text livesText;

	public Lives()
	{
		lives = 30;
	}

	public Lives(int startAmount)
	{
		lives = startAmount;
	}

	void Update()
	{
		if (isLocalPlayer) {
			if (livesText == null) {
				livesText = GameObject.Find ("LivesText").GetComponent<Text> ();
				updateText ();
				Debug.Log ("Lives Text set to local player!");
			}
		}
	}

    public void setLives(int amount)
    {
        lives = amount;
		updateText ();
    }

	[ClientRpc]
	public void RpcLoseLife()
	{
		loseLife ();
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

	public void updateText()
	{
		if (livesText != null) {
			livesText.text = lives.ToString();
		}
	}
}
