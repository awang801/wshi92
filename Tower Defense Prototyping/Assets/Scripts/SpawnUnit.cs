using UnityEngine;
using System.Collections;

public class SpawnUnit : MonoBehaviour
{

    public GameObject spawnUnit;

    public Transform[] waypoints;

    public int startIndex;

    public float moveSpeed;

    public float spawnDelay;

    private float timeSinceSpawn;

    // Use this for initialization
    void Start()
    {
        startIndex = 0;
        moveSpeed = 10f;
        spawnDelay = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        if (timeSinceSpawn >= spawnDelay)
        {
            Spawn();
            timeSinceSpawn = 0;
        }
    }

    void Spawn()
    {
        GameObject newUnit = ((GameObject)(Instantiate(spawnUnit, transform.position, transform.rotation)));

        UnitMove newUnitMover = newUnit.GetComponent<UnitMove>();

        newUnitMover.Setup(waypoints, startIndex, moveSpeed);

    }
}
