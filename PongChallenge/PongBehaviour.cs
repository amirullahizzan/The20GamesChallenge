using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerControl playerControl;
    public Vector2 InitPos;
    private void Awake()
    {
        playerControl = GameObject.Find("ControlManager").GetComponent<PlayerControl>();
    }
    void Start()
    {
        InitPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    bool IsOOBTop()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        return transform.position.y > 5 - boxCollider.size.y * 0.5f;
    }
    public AudioClip BounceSFX;
    bool IsOOBBottom()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        return transform.position.y < (5 - boxCollider.size.y*0.5f) * -1;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ball")
        {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.audioSource.PlayOneShot(BounceSFX, 1.0f);
        }
        
    }


    public void MoveUp()
    {
        if (IsOOBTop())
        {
            return;
        }
        transform.position += Vector3.up * playerControl.GetMoveSpeed();
    }

    public void MoveDown()
    {
        if (IsOOBBottom())
        {
            return;
        }
        transform.position += Vector3.down * playerControl.GetMoveSpeed();
    }
}
