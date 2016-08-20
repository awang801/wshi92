using UnityEngine;
using System.Collections;

public class Bank:MonoBehaviour{
    private int money = 100;

    public void setMoney(int amount)
    {
        money = amount;
    }
    public void addMoney(int amount)
    {
        money = money + amount;
    }


    public void subractMoney(int amount)
    {
        money = money - amount;
    }

    public int getMoney()
    {
        return money;
    }
}
