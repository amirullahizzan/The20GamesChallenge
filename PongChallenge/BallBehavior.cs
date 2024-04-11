using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetRandomPosOnMiddle();
        SetForceRandomDir();
    }
    public float minSpawnPos = -5;
    public float maxSpawnPos = 5;

    public void SetForceRandomDir()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        float MINIMUM_FORCE = 15.0f;
        float forceMultiplier = 20.0f;

        Vector2 randomizedForce = new Vector2(0, 0);
        while(randomizedForce.x == 0 || randomizedForce.y == 0)
        {
        randomizedForce.x = Random.Range(-1,2);
        randomizedForce.y = Random.Range(-1,2);
        }
        randomizedForce *= Random.Range(MINIMUM_FORCE, 20) * forceMultiplier;
        rb.AddForce(randomizedForce);
    }
    
    public void SetRandomPosOnMiddle()
    {
        float randomizedYPos = Random.Range(minSpawnPos, maxSpawnPos);
        transform.position = new Vector2(transform.position.x, randomizedYPos);
    }

}
