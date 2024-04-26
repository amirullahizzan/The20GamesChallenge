using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public class EmptySlot
{

}
public class CardSystem : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> emptySlotsGO = new();
    public List<GameObject> EmptySlotsGO => emptySlotsGO;
    [SerializeField] GameObject emptySlotPrefab;

    [SerializeField] Grid grid;
    [SerializeField][Category("Prefab")] GameObject cardPrefab;
    [SerializeField] GameObject deckGO;

    public CardData[,] CardsDataArray => cardsDataArray;
    public GameObject[,] CardsGOArray => cardsGOArray;
    [SerializeField] Transform cardFirstSpawnPoint;
    [SerializeField] GameObject cardsGroup;
    [SerializeField] AudioClip distributeCardSFX;
    [SerializeField] AudioClip collectFullSetSFX;
    Score score = null;
    StageManager stageManager;

    public const int MAX_CARDS_COLUMNS = 52;
    public const int CARDS_IN_ROW = 10;

    GameObject[,] cardsGOArray = new GameObject[MAX_CARDS_COLUMNS, CARDS_IN_ROW];
    CardData[,] cardsDataArray = new CardData[MAX_CARDS_COLUMNS, CARDS_IN_ROW];
    List<GameObject> frontLineCardsGOList = new();
    public List<GameObject> FrontLineCardsGOList => frontLineCardsGOList;
    public bool IsOutOfDeck()
    {
        return deckCardsCount <= 0;
    }
    AudioSource audioSource;
    Vector2 spawnPointGridPos;
    Vector2 startingPointGridPos;
    public enum EGameMode
    {
        OneSuit = 0,
    }
    public enum ESuit
    {
        Spade = 1, Clubs, Diamond, Heart,
    }
    public enum ERank
    {
        One = 1, //Ace
        Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King,
    }
    EGameMode gameMode = EGameMode.OneSuit;

    int totalCards = 104;
    public int TotalCards => totalCards;
    int tableauCardsCount = 54;

    int deckCardsCount;

    private void Awake()
    {
        score = GetComponent<Score>();
        stageManager = GetComponent<StageManager>();
        spriteLoader = GetComponent<SpriteLoader>();
        audioSource = GetComponent<AudioSource>();
        //AwakeDebug();
        startingPointGridPos = SnapToNearestGrid(cardFirstSpawnPoint.position);
        deckCardsCount = totalCards - tableauCardsCount;
    }
    void AwakeDebug()
    {
        totalCards = 40;
        tableauCardsCount = 20;
    }


    void Start()
    {
        ConstructStartingCards();
        CreateCardsRoles();
        ShuffleCardsRoleExperimental(roleTuple); //Disable on debug
        AssignCardsRoles(roleTuple);
        InitFrontlineCards();

        spawnPointGridPos = startingPointGridPos;

        CreateEmptySlotsToList();
        DistributeEmptySlots();
        DistributeInitCards();
        RevealFrontCards();
        SortCardsZPosToFront();
    }

    [SerializeField] Transform emptySlotsGroup;
    void CreateEmptySlotsToList()
    {
        for (int i = 0; i < CARDS_IN_ROW; i++)
        {
            GameObject emptySlotGO = Instantiate(emptySlotPrefab, transform.position, transform.rotation, emptySlotsGroup);
            emptySlotsGO.Add(emptySlotGO);
            CardData emptySlotsData = emptySlotGO.GetComponent<CardData>();
            emptySlotsData.SaveColumnRowIndex(-1, i);
        }

    }


    void Update()
    {
        if (stageManager.IsGameStart)
        {
            for (int j = 0; j < MAX_CARDS_COLUMNS; j++)
            {
                for (int i = 0; i < CARDS_IN_ROW; i++)
                {
                    if (!cardsDataArray[j, i]) continue;
                    if (!cardsDataArray[j, i].HasArrived() && !cardsDataArray[j, i].IsDragged)
                    {
                        cardsDataArray[j, i].UpdateFlyTowards();
                    }
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < 10; i++)
            {
                //print(frontLineCardsGOList[i].name);
                print(cardsDataArray[1, 0].name);
            }

        }


    }

    void ConstructStartingCards()
    {
        int cardsConstructed = 0;
        for (int j = 0; j < MAX_CARDS_COLUMNS; j++)
        {
            for (int i = 0; i < CARDS_IN_ROW; i++)
            {
                GameObject instantiatedCardGO = Instantiate(cardPrefab, startingPointGridPos, Quaternion.identity, cardsGroup.transform);
                cardsGOArray[j, i] = instantiatedCardGO;

                CardData cardData = instantiatedCardGO.GetComponent<CardData>();
                cardsDataArray[j, i] = cardData;

                cardsDataArray[j, i].SaveColumnRowIndex(j, i);

                cardsConstructed++;
                if (cardsConstructed >= tableauCardsCount)
                {
                    return;
                }
            }
        }
    }

    void InitFrontlineCards()
    {
        int constructedCard = 0;
        for (int j = 0; j < MAX_CARDS_COLUMNS; j++)
        {
            for (int i = 0; i < CARDS_IN_ROW; i++)
            {
                if (cardsGOArray[j + 1, i] == null)
                {
                    frontLineCardsGOList.Add(cardsGOArray[j, i]);
                }
                constructedCard++;
                if (constructedCard >= tableauCardsCount) return;
            }
        }

    }

    int cardsSpreaded = 0;
    public void DistributeEmptySlots()
    {
        spawnPointGridPos.x = startingPointGridPos.x;
        for (int i = 0; i < CARDS_IN_ROW; i++)
        {
            spawnPointGridPos.x = startingPointGridPos.x;
            spawnPointGridPos.x += grid.cellSize.x * i;
            emptySlotsGO[i].transform.position = new Vector3(spawnPointGridPos.x, spawnPointGridPos.y, 1);
        }
    }
    public void PlayDistributeCardSFX()
    {
        audioSource.PlayOneShot(distributeCardSFX);
    }


    public void DistributeInitCards()
    {

        for (int j = 0; j < MAX_CARDS_COLUMNS; j++)
        {
            spawnPointGridPos.x = startingPointGridPos.x;
            for (int i = 0; i < CARDS_IN_ROW; i++)
            {
                if (cardsSpreaded >= tableauCardsCount) return;
                spawnPointGridPos.x = startingPointGridPos.x;
                spawnPointGridPos.x += grid.cellSize.x * i;
                cardsDataArray[j, i].SetTargetPos(spawnPointGridPos);
                cardsSpreaded++;
            }
            spawnPointGridPos.y -= grid.cellSize.y;
        }
    }
    public void UnAssignFromArray(int column, int row)
    {
        cardsGOArray[column, row] = null;
        cardsDataArray[column, row] = null; //Assign on the next slot
    }
    public void AssignDraggedToNewArray(GameObject draggedCard, CardData draggedCardData, int hitColumn, int hitRow)
    {
        if (IsASlotColumn(hitColumn))
        {
            for (int h = 0; h < MAX_CARDS_COLUMNS; h++)
            {
                if (cardsGOArray[h, hitRow] == null)
                {
                    cardsGOArray[h, hitRow] = draggedCard; //Assign on the next slot
                    cardsDataArray[h, hitRow] = draggedCardData; //Assign on the next slot
                    return;
                }
            }
        }
        else
        {
            for (int h = hitColumn + 1; h < MAX_CARDS_COLUMNS; h++)
            {
                if (cardsGOArray[h, hitRow] == null)
                {
                    cardsGOArray[h, hitRow] = draggedCard; //Assign on the next slot
                    cardsDataArray[h, hitRow] = draggedCardData; //Assign on the next slot
                    return;
                }
            }
        }
    }
    bool IsASlotColumn(int columnCheck)
    {
        return columnCheck < 0;
    }
    public bool IsAFrontLineCard(GameObject checkedGameObject)
    {
        foreach (GameObject frontLineGO in frontLineCardsGOList)
        {
            if (checkedGameObject == frontLineGO) return true;
        }
        return false;
    }


    List<(ESuit suitList, ERank rankList)> roleTuple = new();
    public List<(ESuit suitList, ERank rankList)> RoleTuple => roleTuple;


    public void CreateCardsRoles()
    {
        int rankLength = System.Enum.GetValues(typeof(ERank)).Length;
        int suitLength = System.Enum.GetValues(typeof(ESuit)).Length;

        for (int i = 0; i < totalCards;) // assign all cards
        {
            for (int suitIndex = 1; suitIndex < suitLength + 1; suitIndex++)
            {
                ESuit currentSuit = (ESuit)suitIndex;
                for (int rankIndex = 1; rankIndex < rankLength + 1; rankIndex++)
                {
                    ERank currentRank = (ERank)rankIndex;
                    var tupleData = (currentSuit, currentRank);
                    roleTuple.Add(tupleData);
                    i++;
                }
                if (gameMode == EGameMode.OneSuit)
                {
                    break;
                }
            }
        }

    }
    void ShuffleCardsRoleExperimental(List<(ESuit suits, ERank ranks)> tuplePoolToShuffle) //TODO
    {
        for (int i = tuplePoolToShuffle.Count - 1; i >= 0; i--) //Count to Index
        {
            int randomizedIndex = UnityEngine.Random.Range(0, tuplePoolToShuffle.Count - 1);
            (ESuit, ERank) temp = roleTuple[i];
            roleTuple[i] = roleTuple[randomizedIndex];
            roleTuple[randomizedIndex] = temp;
        }

    }
    int cardAssignedIndex = 0;
    SpriteLoader spriteLoader;
    public void AssignCardsRoles(List<(ESuit suits, ERank ranks)> shuffledTuplePool)
    {
        for (int j = 0; j < MAX_CARDS_COLUMNS; j++) // assign all cards
        {
            for (int i = 0; i < CARDS_IN_ROW; i++) // assign all cards
            {
                if (cardsDataArray[j, i] == null) continue;
                if (!cardsDataArray[j, i].IsColumnRowUnassigned())
                {
                    cardsDataArray[j, i].AssignSuitRank(shuffledTuplePool[cardAssignedIndex].suits, shuffledTuplePool[cardAssignedIndex].ranks);
                    cardsDataArray[j, i].UpdateSprite(spriteLoader);
                    cardsDataArray[j, i].RenameSelf();
                    cardsDataArray[j, i].name += cardAssignedIndex;

                    cardAssignedIndex++;
                }
            }
        }
    }

    public void AssignCardsRolesToFrontline(List<(ESuit suits, ERank ranks)> shuffledTuplePool)
    {
        foreach (GameObject frontlineCard in FrontLineCardsGOList)
        {
            CardData flCardData = frontlineCard.GetComponent<CardData>();
            if (!flCardData.IsColumnRowUnassigned())
            {
                flCardData.AssignSuitRank(shuffledTuplePool[cardAssignedIndex].suits, shuffledTuplePool[cardAssignedIndex].ranks);
                flCardData.UpdateSprite(spriteLoader);
                //flCardData.RenameSelf();
                cardAssignedIndex++;
            }
            else
            {
                print("Column row is unassigned! -999");
            }
        }

    }


    //public void AssignCardsRoles()
    //{
    //    int rankLength = System.Enum.GetValues(typeof(ERank)).Length;
    //    int suitLength = System.Enum.GetValues(typeof(ESuit)).Length;
    //    int rankIndex = 1;
    //    int suitIndex = 1;
    //    int roleAssignedCards = 0;
    //    int cardNumber = 0;
    //    for (int j = 0; j < MAX_CARDS_COLUMNS; j++)
    //    {
    //        for (int i = 0; i < CARDS_IN_ROW; i++)
    //        {
    //            ESuit currentSuit = (ESuit)suitIndex;
    //            suitIndex++;
    //            if (suitIndex > suitLength || gameMode == EGameMode.OneSuit) suitIndex = 1;
    //
    //            ERank currentRank = (ERank)rankIndex;
    //            //print(currentRank);
    //            rankIndex++;
    //            if (rankIndex > rankLength) rankIndex = 1;
    //            cardsDataArray[j, i].AssignSuitRank(currentSuit, currentRank);
    //            cardsDataArray[j, i].UpdateSprite(spriteLoader);
    //            cardsDataArray[j, i].RenameSelf();
    //            cardsDataArray[j, i].name += " " + cardNumber;
    //            roleAssignedCards++;
    //            if (roleAssignedCards >= tableauCardsCount) return;
    //        }
    //    }
    //}

    public void RevealFrontCards()
    {
        foreach (GameObject card in frontLineCardsGOList)
        {
            if (IsAFrontLineCard(card))
            {
                card.GetComponent<CardData>().TryRevealCard();
            }
        }
    }

    public (int column, int row) GetCardArray(CardData cardData)
    {
        if (cardData.Column == -1) return (cardData.Column, cardData.Row);
        for (int j = 0; j < MAX_CARDS_COLUMNS; j++) //Checks for Slots
        {
            for (int i = 0; i < CARDS_IN_ROW; i++)
            {
                if (cardData == cardsDataArray[j, i]) return (j, i);
            }
        }
        Debug.LogError("Something is wrong!");
        return (-999, -999);
    }

    public const float zSorterModifier = -0.1f; //Minus is forward
    public Vector3 SortOneCardZPosToFront(GameObject targetGO, float desiredZValue)
    {
        return targetGO.transform.position = new Vector3(
                            targetGO.transform.position.x,
                            targetGO.transform.position.y,
                            desiredZValue);
    }

    public void SortCardsZPosToFront()
    {
        int cardsSorted = 0;
        for (int j = 0; j < MAX_CARDS_COLUMNS; j++)
        {
            float zSorterPos = zSorterModifier * j; //Minus forward
            for (int i = 0; i < CARDS_IN_ROW; i++)
            {
                if (!cardsGOArray[j, i]) continue;
                cardsGOArray[j, i].transform.position =
                    SortOneCardZPosToFront(cardsGOArray[j, i], zSorterPos);
                cardsSorted++;
                if (cardsSorted >= tableauCardsCount) return;
            }
        }
    }

    public bool IsHitCardHigherValueByOne(CardData targetData, CardData hitData)
    {
        return targetData.RankInt + 1 == hitData.RankInt;
    }
    public bool IsAllNextCardsInOrder(CardData draggedData)
    {
        for (int h = draggedData.Column; h < MAX_CARDS_COLUMNS; h++)//Checks All Next columns
        {
            if (cardsDataArray[h + 1, draggedData.Row] == null)
            {
                return true;
            }
            else if (cardsDataArray[h, draggedData.Row].RankInt == cardsDataArray[h + 1, draggedData.Row].RankInt + 1)
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    private void RevealCardAbove(int column, int row)
    {

        if (column - 1 < 0)
        { return; }
        cardsDataArray[column - 1, row].TryRevealCard();
    }
    public bool IsFullSet(int searchRowIndex)
    {
        CardData lastKingData = GetLatestKingInColumn(searchRowIndex);
        if (!lastKingData)
        {
            return false;
        }
        int totalSetMatch = 0;

        for (int h = lastKingData.Column + 1; h < MAX_CARDS_COLUMNS; h++) //Check downward
        {
            if (cardsDataArray[h, searchRowIndex] == null) { break; }
            if (cardsDataArray[h, searchRowIndex].RankInt == (int)ERank.Queen - totalSetMatch)
            {
                totalSetMatch++;
            }
        }

        //int fullSetValue = 1;
        int fullSetValue = (int)ERank.Queen;
        return totalSetMatch >= fullSetValue; //Total of 13 Pair + King
    }



    public void DestroyFullSetAndRevealNew(int targetCheckRow)
    {

        foreach (GameObject sussy in frontLineCardsGOList)
        {
            print("Previous Members : " + sussy);
        }

        CardData latestKingData = GetLatestKingInColumn(targetCheckRow);
        RevealCardAbove(latestKingData.Column, latestKingData.Row); //Sussy
        for (int cardsUnderKingIndex = latestKingData.Column + 1; cardsUnderKingIndex < MAX_CARDS_COLUMNS; cardsUnderKingIndex++)
        {
            if (CardsGOArray[cardsUnderKingIndex, latestKingData.Row] == null) { break; }
            GameObject destroyGameObject = cardsGOArray[cardsUnderKingIndex, latestKingData.Row];
            Destroy(destroyGameObject);

            if (CardsGOArray[cardsUnderKingIndex + 1, latestKingData.Row] == null)
            {
                if (latestKingData.Column - 1 >= 0 && CardsGOArray[latestKingData.Column - 1, latestKingData.Row]) //Check if there is a card above King
                {
                    frontLineCardsGOList.Add(CardsGOArray[latestKingData.Column - 1, latestKingData.Row]);
                    //                    print("ADDED : " + CardsGOArray[latestKingData.Column - 1, latestKingData.Row].name + " " + (latestKingData.Column - 1) + " , " + latestKingData.Row);
                    if (frontLineCardsGOList.Count > 10)
                    {
                        print("frontline Counts reaches more than 10 = " + frontLineCardsGOList.Count);
                    }
                }
                frontLineCardsGOList.Remove(CardsGOArray[cardsUnderKingIndex, latestKingData.Row]);
                print("REMOVES  : " + CardsGOArray[cardsUnderKingIndex, latestKingData.Row].name + cardsUnderKingIndex + " " + latestKingData.Row);
                print("frontline Counts is now " + frontLineCardsGOList.Count);
            }
            //Null always last
            CardsGOArray[cardsUnderKingIndex, latestKingData.Row] = null;
            CardsDataArray[cardsUnderKingIndex, latestKingData.Row] = null;
        }


        Destroy(latestKingData.gameObject);
        frontLineCardsGOList.Remove(CardsGOArray[latestKingData.Column, latestKingData.Row]);
        CardsGOArray[latestKingData.Column, latestKingData.Row] = null;
        CardsDataArray[latestKingData.Column, latestKingData.Row] = null;

        audioSource.PlayOneShot(collectFullSetSFX);

    }


    CardData GetLatestKingInColumn(int rowIndex)
    {
        CardData latestKingDataFound = null;
        for (int h = 0; h < MAX_CARDS_COLUMNS; h++) //Check downward
        {
            if (cardsDataArray[h, rowIndex] == null) break;
            if (!cardsDataArray[h, rowIndex].IsRevealed) continue;

            if (cardsDataArray[h, rowIndex].RankInt == (int)ERank.King)//Found King
            {
                latestKingDataFound = cardsDataArray[h, rowIndex]; //Replace Previous king found
            }
        }

        return latestKingDataFound;
    }
    public void AddToFrontline(GameObject targetGO)
    {
        FrontLineCardsGOList.Add(targetGO);
    }
    public void AddToFrontline(int _column, int _row)
    {
        FrontLineCardsGOList.Add(CardsGOArray[_column, _row]);
    }
    public void RemoveFrontlineStatus(GameObject targetGO)
    {
        FrontLineCardsGOList.Remove(targetGO);
    }
    Vector2 SnapToNearestGrid(Vector2 vector3ToSnap)
    {
        Vector3Int nearestCellPosition = grid.WorldToCell(vector3ToSnap);
        // Convert the snapped grid cell position back to world space
        Vector2 snappedWorldPosition = grid.CellToWorld(nearestCellPosition);
        return snappedWorldPosition;
    }

    int objectCounter = 0;
    List<GameObject> cardsOnEmptySlot = null;
    public void ConstructCardsFromDeck()
    {
        cardsOnEmptySlot = new();
        for (int i = 0; i < CARDS_IN_ROW; i++) //Sometimes there are only 9 fl... or less
        {
            if (cardsGOArray[0, i] == null) //Construct FL on empty spot
            {
                GameObject emptyslotCardGO = Instantiate(cardPrefab, startingPointGridPos, Quaternion.identity, cardsGroup.transform);
                cardsGOArray[0, i] = emptyslotCardGO;
                CardData emptySlotFrontData = emptyslotCardGO.GetComponent<CardData>();
                emptySlotFrontData.SaveColumnRowIndex(0, i);

                cardsGOArray[emptySlotFrontData.Column, i] = emptyslotCardGO;
                cardsDataArray[emptySlotFrontData.Column, i] = emptySlotFrontData;


                cardsOnEmptySlot.Add(emptyslotCardGO);

                objectCounter++;
                tableauCardsCount++;
                deckCardsCount--;
            }
        }

        for (int i = 0; i < frontLineCardsGOList.Count; i++) //Sometimes there are only 9 fl... or less
        {
            CardData frontlineData = frontLineCardsGOList[i].GetComponent<CardData>(); //list array is not in order
            GameObject instantiatedCardGO = Instantiate(cardPrefab, startingPointGridPos, Quaternion.identity, cardsGroup.transform);
            cardsGOArray[frontlineData.Column + 1, frontlineData.Row] = instantiatedCardGO;

            CardData cardData = instantiatedCardGO.GetComponent<CardData>();
            cardsDataArray[frontlineData.Column + 1, frontlineData.Row] = cardData;
            cardsDataArray[frontlineData.Column + 1, frontlineData.Row].SaveColumnRowIndex(frontlineData.Column + 1, frontlineData.Row);
            cardData.name += objectCounter.ToString();


            objectCounter++;
            tableauCardsCount++;
            deckCardsCount--;
        }

    }
    public void DistributeNewCards()
    {
        for (int n = 0; n < CARDS_IN_ROW; n++)
        {
            Vector3 spawnerPos = startingPointGridPos;
            spawnerPos.x = spawnerPos.x+(grid.cellSize.x * n);
            for (int m = 0; m < cardsOnEmptySlot.Count; m++)
            {
                if (cardsGOArray[0, n] == cardsOnEmptySlot[m])
                {
                    Vector3 emptySlotPos = new Vector3(spawnerPos.x, spawnerPos.y, -0.6969f); //Throw away Z now for debugs
                    cardsDataArray[0, n].SetTargetPos(emptySlotPos);
                    print(cardsDataArray[0, n].name + " : distributed to empty slot");
                }
            }
        }
        for (int i = 0; i < frontLineCardsGOList.Count; i++)
        {
            if (cardsSpreaded >= tableauCardsCount)
            {
                print("Returning! Something is wrong!");
                return;
            }

            CardData newFrontData = frontLineCardsGOList[i].GetComponent<CardData>(); //HAS NO COLUMN NOR ROW YET


            Vector3 oldFrontPos = cardsDataArray[newFrontData.Column, newFrontData.Row].transform.position;
            Vector3 newFrontPos = new Vector3(oldFrontPos.x, oldFrontPos.y - grid.cellSize.y, -0.6969f); //Throw away Z now for debugs
            cardsDataArray[newFrontData.Column + 1, newFrontData.Row].SetTargetPos(newFrontPos);
            //print("the rest of the cards : " + cardsDataArray[cardData.Column, cardData.Row].name + " " + cardData.Column + " " + cardData.Row); //CLONE 0

            cardsSpreaded++;
        }
        audioSource.PlayOneShot(distributeCardSFX);
    }
    public void TransferFrontlineStatus()
    {
        List<CardData> previousFrontlinesData = new();

        for (int i = 0; i < frontLineCardsGOList.Count; i++)
        {
            CardData frontlineData = frontLineCardsGOList[i].GetComponent<CardData>();

            if (cardsGOArray[frontlineData.Column + 1, frontlineData.Row])
            {
                previousFrontlinesData.Add(frontlineData);
                print("previous frontlines added!" + frontlineData.column + " , " + frontlineData.row); //4 ,2 clone 1
                frontLineCardsGOList.Add(cardsGOArray[frontlineData.Column + 1, frontlineData.Row]);
                cardsDataArray[frontlineData.Column + 1, frontlineData.Row].SaveColumnRowIndex(frontlineData.Column + 1, frontlineData.Row);
                print("22222!" + frontlineData.column + " , " + frontlineData.row);
            }
        }
        print(previousFrontlinesData.Count);

        for (int i = 0; i < previousFrontlinesData.Count; i++)
        {
            CardData previousData = previousFrontlinesData[i];
            {
                frontLineCardsGOList.Remove(cardsGOArray[previousData.Column, previousData.Row]);
            }
        }

        for (int i = 0; i < cardsOnEmptySlot.Count; i++)
        {
            frontLineCardsGOList.Add(cardsOnEmptySlot[i]);
        }

        previousFrontlinesData.Clear();
        cardsOnEmptySlot.Clear();
    }


    public bool IsEmptySlotHit(int _rowCheckIndex)
    {
        return IsLastColumnEmpty(_rowCheckIndex);
    }
    public bool IsLastColumnEmpty(int rowCheckIndex)
    {
        if (CardsGOArray[0, rowCheckIndex] == null)
        {
            return true;
        }
        return false;
    }

    public void CheckFullSetOnNewCards()
    {
        List<CardData> destroyCardList = new();
        foreach (GameObject card in frontLineCardsGOList)
        {
            CardData cardData = card.GetComponent<CardData>();
            destroyCardList.Add(cardData);
        }

        foreach (CardData cardDataToDestroy in destroyCardList)
        {
            if (IsFullSet(cardDataToDestroy.Row))
            {
                ProcessFullSet(cardDataToDestroy);
            }

            destroyCardList = null;
        }
    }

    internal void AttemptDrawCards()
    {
        if (!stageManager.IsGameStart)
        {
            return;
        }
        ConstructCardsFromDeck();
        DistributeNewCards();
        TransferFrontlineStatus();

        AssignCardsRolesToFrontline(roleTuple);
        SortCardsZPosToFront();
        RevealFrontCards();
        if (IsOutOfDeck())
        {
            DestroyDeck();
        }
        CheckFullSetOnNewCards();
    }

    public void ProcessFullSet(CardData affectorDataToRemove)
    {
        score.AddScore();
        frontLineCardsGOList.Remove(CardsGOArray[affectorDataToRemove.Column, affectorDataToRemove.Row]); //Remove DraggedCard
        score.CopyLastCardAsFoundation(affectorDataToRemove.Row);
        DestroyFullSetAndRevealNew(affectorDataToRemove.Row);
    }

    void DestroyDeck()
    {
        Destroy(deckGO);
    }
}
