using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	float health;


	// Use this for initialization
	void Start () {
		health = 3;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Damage(float dmg)
	{
		health -= dmg;
		if (health <= 0) {
			Death ();
		}
	}

	void Death()
	{
        Bank bank;
		Instantiate (Resources.Load ("Enemies/EnemyDeath"), transform.position, transform.rotation);
		Destroy (gameObject);
        bank = GameObject.Find("GameManager").GetComponent<Bank>();
        bank.addMoney(15);
        Debug.Log(bank.getMoney());
    }

    public void Finish()
    {
        Instantiate(Resources.Load("Enemies/EnemyDeath"), transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
