using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleSO Data", menuName = "ScriptableObjects/Obstacle Data", order = 1)]
public class ObstacleSO : ScriptableObject
{
    [SerializeField] public ObstacleStat stat = new ObstacleStat();
    public int GetHP => stat.hp;
    public ESpawnType mySpawnType;
    public ESpawnPosition mySpawnPos;

    public enum ESpawnType
    {
        Pillar = 0,
        Box,
        Trash,
    }
    public enum ESpawnPosition
    {
        Top = 0,
        Bottom,
        Middle,
    }

    public void TakeDamage(int damageReceived)
    {
        stat.hp -= damageReceived;
    }

}

[System.Serializable]
public class ObstacleStat
{
    public int hp;
}
