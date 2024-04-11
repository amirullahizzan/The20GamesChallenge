using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] public GameObject[] PlayerGO;
    float moveSpeed = 0.2f;
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    private void Start()
    {
        gameManager.mainEvent += ControlPong;
    }

    public GameManager gameManager;
    void Update()
    {

    }

    PongBehaviour GetPongComponent(GameObject playerGO)
    {
        return playerGO.GetComponent<PongBehaviour>();
    }

    void Player1Control()
    {
        if (Input.GetKey(KeyCode.W))
        {
            GetPongComponent(PlayerGO[0]).MoveUp();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            GetPongComponent(PlayerGO[0]).MoveDown();
        }
    }


    void Player2Control()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            GetPongComponent(PlayerGO[1]).MoveUp();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            GetPongComponent(PlayerGO[1]).MoveDown();
        }
    }
    public void ControlPong()
    {
        if (PlayerGO[0])
        {
            Player1Control();
        }

        if (PlayerGO[1])
        {
            Player2Control();
        }

        GetTouchScreenControl();
    }

    void GetTouchScreenControl()
    {
        if (Input.touchCount <= 0) return;
        for (int i = 0; i< Input.touchCount ;i++)
        {
            UnityEngine.Touch touch = Input.GetTouch(i);

                MoveTouchedPongArea(touch);
        }
    }
    private void MoveTouchedPongArea(UnityEngine.Touch touchedSide)
    {
        if (touchedSide.position.x < Screen.width * 0.5f)
        {
            Vector2 initPos = PlayerGO[0].GetComponent<PongBehaviour>().InitPos;
            Vector3 convertedTouchedPos = Camera.main.ScreenToWorldPoint(new Vector3(0, touchedSide.position.y, 0));
            convertedTouchedPos.x = initPos.x;
            convertedTouchedPos.z = 0;
            PlayerGO[0].transform.position = Vector3.MoveTowards(PlayerGO[0].transform.position, convertedTouchedPos, moveSpeed);
        }
        else if (touchedSide.position.x > Screen.width * 0.5f)
        {
            Vector2 initPos = PlayerGO[1].GetComponent<PongBehaviour>().InitPos;
            Vector3 convertedTouchedPos = Camera.main.ScreenToWorldPoint(new Vector3(0, touchedSide.position.y, 0));
            convertedTouchedPos.x = initPos.x;
            convertedTouchedPos.z = 0;
            PlayerGO[1].transform.position = Vector3.MoveTowards(PlayerGO[1].transform.position, convertedTouchedPos, moveSpeed);
        }
    }
  
}
