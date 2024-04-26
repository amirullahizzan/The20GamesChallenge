using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickManager : MonoBehaviour
{
    [SerializeField] Grid grid;

    Vector2 mousePosition2D;
    GameObject draggedCard = null;
    CardSystem cardSys;
    GameObject hitCard = null;
    CardData draggedCardData = null;
    CardData hitCardData = null;
    StageManager stageManager;
    AudioSource audioSource;
    [SerializeField] AudioClip cardPickSFX;
    [SerializeField] AudioClip cardDropSFX;
    private void Awake()
    {
        cardSys = GetComponent<CardSystem>();
        stageManager = GetComponent<StageManager>();
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    List<GameObject> draggedCards = new();
    void Update()
    {
        mousePosition2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (!draggedCard)
            {
                Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.zero);
                if (hitInfo)
                {
                    if (hitInfo.collider.CompareTag("Card"))
                    {
                        CardClickProcess(hitInfo);
                        audioSource.PlayOneShot(cardPickSFX);
                    }
                    else if (hitInfo.collider.CompareTag("Deck"))
                    {
                        cardSys.AttemptDrawCards();
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && draggedCard) // On Release
        {
            GetCardDataFromRay(GetCastRay());

            if (hitCard && cardSys.IsHitCardHigherValueByOne(draggedCardData, hitCardData))
            {
                int downIndex = 1;
                GameObject cardLeftoverAboveDragged = GetCardAbove(draggedCardData); //Left behind card
                if (cardLeftoverAboveDragged)
                {
                    cardLeftoverAboveDragged.GetComponent<CardData>().TryRevealCard();
                    if (!cardSys.FrontLineCardsGOList.Contains(cardLeftoverAboveDragged)) cardSys.AddToFrontline(cardLeftoverAboveDragged);
                }
                foreach (GameObject card in draggedCards)
                {
                    ArrangeDraggedTargetPosToHitObject(card, hitCard, downIndex * grid.cellSize.y, CardSystem.zSorterModifier * downIndex);
                    MoveArrays(card, hitCard);
                    downIndex++;
                }
                cardSys.RemoveFrontlineStatus(hitCardData.gameObject);
                if (cardSys.IsFullSet(hitCardData.Row))
                {
                    cardSys.ProcessFullSet(draggedCardData);
                }
            }
            else if (hitSlot && cardSys.IsEmptySlotHit(hitSlotData.Row))
            {
                //print("1 " + "" + draggedCardData.Column);
                if (!IsTheLastCardInColumn(draggedCardData))
                {
                    GameObject cardLeftoverAboveDragged = GetCardAbove(draggedCardData); //Left behind card
                    if (cardLeftoverAboveDragged)
                    {
                        cardLeftoverAboveDragged.GetComponent<CardData>().TryRevealCard();
                        if (!cardSys.FrontLineCardsGOList.Contains(cardLeftoverAboveDragged)) cardSys.AddToFrontline(cardLeftoverAboveDragged);
                    }
                }

                int downIndex = 1;
                foreach (GameObject card in draggedCards)
                {
                    ArrangeDraggedTargetPosToHitObject(card, hitSlot, downIndex * grid.cellSize.y, CardSystem.zSorterModifier * downIndex);

                    MoveArrays(card, hitSlot);

                    downIndex++;
                }

            }
            else
            {
                foreach (CardData cardData in draggedCardsData)
                {
                    PutBackDraggedCard(cardData);
                }
            }
            RestoreDraggedRaycast(draggedCard);
            ReleaseDraggedObject();
        }

        if (draggedCard)
        {
            int draggedIndex = 0;
            foreach (GameObject card in draggedCards)
            {
                card.transform.position = PositionYBelowFirstCard(draggedCards[0], card, grid.cellSize.y * draggedIndex);

                draggedIndex++;
            }

        }

    }



    bool IsTheLastCardInColumn(CardData card)
    {
        return card.Column == 0;
    }


    void CardClickProcess(RaycastHit2D hitInfo)
    {

        draggedCardData = hitInfo.collider.GetComponent<CardData>();
        if (draggedCardData.IsRevealed)
        {
            if (cardSys.IsAFrontLineCard(hitInfo.collider.gameObject) || cardSys.IsAllNextCardsInOrder(draggedCardData))
            {
                draggedCard = hitInfo.collider.gameObject; //Assign Dragged Card
                draggedCards.Add(draggedCard);
                draggedCardsData.Add(draggedCardData);

                if (cardSys.IsAFrontLineCard(hitInfo.collider.gameObject))
                {
                    draggedCardData.SetDragged(true);
                    draggedCardData.DisableRaycastTrigger();
                    print("frontline status : " + cardSys.IsAFrontLineCard(hitInfo.collider.gameObject));
                }
                else if (cardSys.IsAllNextCardsInOrder(draggedCardData))
                {
                    AddDraggedToList(draggedCardData);
                    SetDraggedAllCardsBelow();
                }
                foreach (CardData cardData in draggedCardsData)
                {
                    cardData.DisableRaycastTrigger();
                    cardData.standbyPosition = cardData.transform.position;
                }

                int sortIndex = 0;
                foreach (GameObject card in draggedCards)
                {
                    PrioritizeZPosFront(card, CardSystem.zSorterModifier * sortIndex - 1.0f);
                    sortIndex++;
                }
            }

        }
    }




    private void MoveArrays(GameObject _draggedCard, GameObject _hitCard)
    {
        CardData _draggedCardData = _draggedCard.GetComponent<CardData>();
        CardData hitData = _hitCard.GetComponent<CardData>();

        var (columnBeforeUnassign, rowBeforeUnassign) = cardSys.GetCardArray(_draggedCardData);

        cardSys.UnAssignFromArray(columnBeforeUnassign, rowBeforeUnassign);

        var (columnHit, rowHit) = cardSys.GetCardArray(hitData);
        cardSys.AssignDraggedToNewArray(_draggedCard, _draggedCardData, columnHit, rowHit);

        var (columnAfterAssign, rowAfterAssign) = cardSys.GetCardArray(_draggedCardData);
        _draggedCardData.SaveColumnRowIndex(columnAfterAssign, rowAfterAssign);
    }


    GameObject GetCardAbove(CardData thisCardData)
    {
        if (thisCardData.Column - 1 < 0) { return null; }
        return cardSys.CardsDataArray[thisCardData.Column - 1, thisCardData.Row].gameObject;
    }
    List<CardData> draggedCardsData = new();
    void AddDraggedToList(CardData clickedCardData)
    {
        for (int j = clickedCardData.Column + 1; j < CardSystem.MAX_CARDS_COLUMNS; j++)
        {
            if (cardSys.CardsGOArray[j, clickedCardData.Row] == null) return;
            draggedCards.Add(cardSys.CardsGOArray[j, clickedCardData.Row]);
            draggedCardsData.Add(cardSys.CardsDataArray[j, clickedCardData.Row]);
        }
    }
    private void SetDraggedAllCardsBelow()
    {
        foreach (CardData card in draggedCardsData)
        {
            card.SetDragged(true);
        }
    }

    Vector3 GetMousePos2DWithStoredZ(GameObject inputObject)
    {
        float storedZPos = inputObject.transform.position.z;
        return inputObject.transform.position = new Vector3(mousePosition2D.x, mousePosition2D.y, storedZPos);
    }

    Vector3 PositionYBelowFirstCard(GameObject anchorCard, GameObject otherCard, float desiredYOffset)
    {
        Vector3 mousePos2D = GetMousePos2DWithStoredZ(anchorCard);
        otherCard.transform.position = new Vector3(mousePos2D.x,
            mousePos2D.y - desiredYOffset,
            otherCard.transform.position.z);
        return otherCard.transform.position;
    }

    RaycastHit2D GetCastRay()
    {
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.zero);
        return hitInfo;
    }
    void GetCardDataFromRay(RaycastHit2D hitInfo)
    {
        if (hitInfo)
        {
            if (hitInfo.collider.CompareTag("Card"))
            {
                if (!cardSys.IsAFrontLineCard(hitInfo.collider.gameObject))
                {
                    return;
                }
                else
                {
                    hitCard = hitInfo.collider.gameObject;
                    hitCardData = hitCard.GetComponent<CardData>();
                }
            }
            else if (hitInfo.collider.CompareTag("EmptySlot"))
            {
                hitSlot = hitInfo.collider.gameObject;
                hitSlotData = hitSlot.GetComponent<CardData>();
            }
        }
    }
    private GameObject hitSlot = null;
    private CardData hitSlotData = null;
    void ArrangeDraggedTargetPosToHitObject(GameObject card, GameObject hitObject, float newYPos, float newZSortPos)
    {
        AdjustZImmediatelyToHit(card, hitObject, newZSortPos);
        CardData hitObjData = hitObject.GetComponent<CardData>();
        Vector3 _targetPos = new Vector3(0, 0, 0);
        _targetPos.x = hitObjData.transform.position.x; // Fly Towards

        if (hitObjData.CompareTag("EmptySlot"))
        {
            _targetPos.y = hitObjData.transform.position.y - newYPos + grid.cellSize.y;
        }
        else
        {
            _targetPos.y = hitObjData.transform.position.y - newYPos;
        }



        CardData cardData = card.GetComponent<CardData>();
        cardData.standbyPosition = card.transform.position;
        cardData.SetTargetPos(_targetPos);
    }
    void PrioritizeZPosFront(GameObject card, float newZSortPos)
    {
        card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, //Unchanged
                   card.transform.position.z + newZSortPos
                   );
    }

    void AdjustZImmediatelyToHit(GameObject card, GameObject _hitCard, float newZSortPos)
    {
        card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, //Unchanged
                    _hitCard.transform.position.z + newZSortPos
                    );

    }

    void RestoreDraggedRaycast(GameObject card)
    {
        Collider2D _draggedCardCollider = card.GetComponent<Collider2D>();
        _draggedCardCollider.enabled = true;
    }

    void PutBackDraggedCard(CardData _draggedCardData)
    {
        //const float putBackCardSpeed = 12.0f;
        _draggedCardData.SetTargetPos(_draggedCardData.standbyPosition);
    }

    void ReleaseDraggedObject()
    {
        foreach (GameObject card in draggedCards)//Checks All this and next columns
        {
            RestoreDraggedRaycast(card);
            CardData cardData = card.GetComponent<CardData>();
            card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y,
            cardData.standbyPosition.z);
        }
        foreach (CardData cardData in draggedCardsData)
        {
            cardData.SetDragged(false);
        }
        audioSource.PlayOneShot(cardDropSFX);
        draggedCards.Clear();
        draggedCardsData.Clear();
        draggedCard = null;
        draggedCardData = null;
        hitCard = null;
        hitCardData = null;
    }



}

