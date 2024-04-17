using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleBehaviour : MonoBehaviour
{
    public ObstacleSO obstacleSO;
    public AudioClip hitSFX;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Bullet"))
        {
            obstacleSO.TakeDamage(3);
            if(IsDestroyed())
            {
                Score scoreOnDestroy = GameObject.Find("GameManager").GetComponent<GameManager>().score;
                scoreOnDestroy.AddScore(3);
                Destroy(gameObject);
            }
        }
    }

    bool IsDestroyed()
    {
        return obstacleSO.GetHP <=0;
    }
}
