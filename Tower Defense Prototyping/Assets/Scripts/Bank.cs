using UnityEngine;
using System.Collections;

public class Bank : MonoBehaviour {
    private int money;


    void Start () {
        money = 0;
	}


    void addMoney(int amount)
    {
        money = money + amount;
    }


    void subractMoney(int amount)
    {
        money = money - amount;
    }

    int getMoney()
    {
        return money;
    }
	// Update is called once per frame
	void Update () {
	
	}
}
