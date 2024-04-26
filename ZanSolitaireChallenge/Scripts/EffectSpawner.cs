using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{

    [SerializeField] GameObject effectPrefab;
    // Update is called once per frame
    Cooldown spawnCooldown = new(0.02f);
    int randomizedDir;
    void Start()
    {
    }
    void Update()
    {
        if (!spawnCooldown.IsCooldown)
        {
            Instantiate(effectPrefab);
        }
    }

}