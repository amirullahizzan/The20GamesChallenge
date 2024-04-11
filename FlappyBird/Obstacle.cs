using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Start is called before the first frame update

    void Move()
    {
        float moveSpeed = transform.position.x - 0.07f;
        transform.position = new Vector3(moveSpeed, transform.position.y, 0);
    }

    // Update is called once per frame
    bool IsOffScreenLeft()
    {
        return transform.position.x <= -6;
    }
    void Update()
    {
        PlayerControl playerControl = GameObject.Find("Player").GetComponent<PlayerControl>() ;
        if(playerControl.IsGameOver())
        {
            return;
        }
        if (IsOffScreenLeft())
        {
            Destroy(gameObject);
        }
        Move();   
    }

    private void OnDestroy()
    {
    }
}
