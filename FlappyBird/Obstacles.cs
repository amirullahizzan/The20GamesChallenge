using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Vector3 upperPos;
    public Vector3 downPos;

    // Update is called once per frame
        const float SPAWN_COOLDOWN = 2.0f;
        float spawnTimer = 0;

    private void Start()
    {
        upperPos.x = transform.position.x;
        downPos.x = transform.position.x;
    }
    bool IsSpawnTime()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
        spawnTimer = SPAWN_COOLDOWN;
        return true;
        }
        return false;
    }
    public int totalSpawned = 0;
    int spawnRand;


    void Update()
    {
        PlayerControl playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
        if (!playerControl.IsGame())
        {
            return;
        }
        spawnRand = Random.Range(0,2);
        GameObject instantiatedObstacle;
        if (!IsSpawnTime()) return;
        if (spawnRand == 0)
        {
            instantiatedObstacle = Instantiate(obstaclePrefab, upperPos, transform.rotation);
        }
        else
        {
            instantiatedObstacle = Instantiate(obstaclePrefab, downPos, transform.rotation);
        }

    }


}
