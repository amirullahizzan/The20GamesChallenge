using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.ComponentModel;

public class CardData : MonoBehaviour
{
    [HideInInspector] Grid grid;
    public int column = -999;
    public int Column => column;
    public int row = -999;
    public int Row => row;

    Vector3 targetPos;
    CardSystem.ESuit suit = CardSystem.ESuit.Spade;
    int SuitInt => (int)suit;
    CardSystem.ERank rank = CardSystem.ERank.One;
    public int RankInt => (int)rank;

    SpriteRenderer cardTexSpriteRen;
    public bool isRevealed = false;
    public bool IsRevealed => isRevealed;
    const float flySpeed = 25.0f;
    [HideInInspector] public Vector3 standbyPosition;
    bool isDragged = false;
    public bool IsDragged => isDragged;
    public bool SetDragged(bool _isDragged) => isDragged = _isDragged;


    private void Awake()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        if(gameObject.CompareTag("Card"))
        {
        cardTexSpriteRen = transform.Find("CardTex").GetComponent<SpriteRenderer>();
        }
    }

    public void SaveColumnRowIndex(int _column, int _row)
    {
        column = _column;
        row = _row;
    }

    public bool IsColumnRowUnassigned()
    {
        return Column == -999 && Row == -999;
    }

    public void PrintRowColumn()
    {
        print(gameObject.name + " : " + Row + " , " + Column);
    }

    public void AssignSuitRank(CardSystem.ESuit _suit, CardSystem.ERank _rank)
    {
        suit = _suit;
        rank = _rank;
    }
    public void UpdateSprite(SpriteLoader spriteLoader)
    {
        cardTexSpriteRen.sprite = spriteLoader.GetSpriteFromSuitRank(SuitInt, RankInt);
    }


    public void UpdateSpriteLegacy()
    {
        TMP_Text suitText = transform.Find("SuitText").GetComponent<TMP_Text>();
        suitText.text = suit.ToString();

        TMP_Text rankText = transform.Find("RankText").GetComponent<TMP_Text>();
        if (IsRankCourtCard(rank))
        {
            rankText.text = GetCourtCardString(rank);
        }
        else
        {
            int intRank = (int)rank;
            rankText.text = intRank.ToString();
        }
    }


    bool IsRankCourtCard(CardSystem.ERank _rank)
    {
        return (int)_rank > 10;
    }
    string GetCourtCardString(CardSystem.ERank _rank)
    {
        switch (_rank)
        {
            case CardSystem.ERank.Jack:
                return "J";
            case CardSystem.ERank.Queen:
                return "Q";
            case CardSystem.ERank.King:
                return "K";
            default:
                return default;
        }
    }

    public void UpdateFlyTowards()
    {
        if (isDragged) return;
        float storedZPos = transform.position.z;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, flySpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, storedZPos);
    }

    public void SetTargetPos(Vector3 newTargetPos)
    {
        targetPos = newTargetPos;
    }

    public void TryRevealCard()
    {
        if (isRevealed)
        {
            return;
        }
        isRevealed = true;
        GameObject cardBackGO = transform.Find("CardBack").gameObject;
        if (!cardBackGO) { print("Object not found!"); return; }
        Destroy(cardBackGO);
    }

    public void RenameSelf()
    {
        gameObject.name = "Card : " + suit + rank;
    }

    public bool HasArrived()
    {
        float xGap = Mathf.Abs(transform.position.x - targetPos.x);
        float yGap = Mathf.Abs(transform.position.y - targetPos.y);
        return xGap <= 0.01f && yGap <= 0.01f;
        //return Vector3.Distance(transform.position, targetPos) <= 0.01f;
    }

    public void DisableRaycastTrigger()
    {
        Collider2D coll2D = GetComponent<Collider2D>();
        coll2D.enabled = false;
    }

}
