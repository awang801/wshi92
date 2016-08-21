using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	float health;

	public Transform target;
	NavMeshAgent agent;
	NavMeshPath path;

	public GameObject attackPlayer;
	public GameObject sendPlayer;

	bool ableToFind;
	bool calculating;

	Bank bank;

	MouseFunctions mFunc;

	void Awake()
	{
		mFunc = GameObject.Find("GameManager").GetComponent<MouseFunctions>();

		agent = GetComponent<NavMeshAgent>();
	}

	// Use this for initialization
	void Start () {
		
		health = 3;

		bank = attackPlayer.GetComponent<Bank>();


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setTarget (Transform targetT)
	{
		target = targetT;
		agent.SetDestination (target.position);
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
        
		Instantiate (Resources.Load ("Enemies/EnemyDeath"), transform.position, transform.rotation);
		Destroy (gameObject);
        bank.addMoney(15);
    }

    public void Finish()
    {
        Instantiate(Resources.Load("Enemies/EnemyDeath"), transform.position, transform.rotation);
        Destroy(gameObject);
    }


	//FROM OLD ENEMY SCRIPT, MIGHT NOT NEED
	/*void Update()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogWarning("Agent has an incomplete path? " + gameObject);
            agent.SetDestination(target.position);
        }
        if (agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            Debug.LogWarning("Agent has no valid path " + gameObject);
            agent.SetDestination(target.position);
        }

        if (agent.hasPath == false && path != null)
        {
            agent.path = path;
            agent.Resume();

            ableToFind = agent.CalculatePath(target.position, calcPath);
            calculating = true;
            Debug.Log("CALCULATING NEW PATH, SETTING TEMPORARY");
        }
        else if (path != agent.path)
        {
            path = agent.path;
        }
        if (calculating == true && agent.pathPending == false && ableToFind == true)
        {
            calculating = false;
            agent.path = calcPath;

            Debug.Log("**NEW PATH SET!");
        }
    }
*/
}
