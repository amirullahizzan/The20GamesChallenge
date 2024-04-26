using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<Sprite> spriteSpadeList = new List<Sprite>();
    [SerializeField] List<Sprite> spriteClubList = new List<Sprite>();
    [SerializeField] List<Sprite> spriteDiamondList = new List<Sprite>();
    [SerializeField] List<Sprite> spriteHeartList = new List<Sprite>();
    List<List<Sprite>> suitList;

    private void Awake()
    {
        suitList = new List<List<Sprite>>()
        {
            spriteSpadeList,
            spriteClubList,
            spriteDiamondList,
            spriteHeartList,
        };
    }
    // Update is called once per frame

    public Sprite GetSpriteFromSuitRank(int suitInt, int rankInt)
    {
        return suitList[suitInt - 1][rankInt - 1];
    }


}
