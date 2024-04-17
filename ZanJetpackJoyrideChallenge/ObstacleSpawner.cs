using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameManager gameManager;
    public Transform obstacleTopPos;
    public Transform obstacleMiddlePos;
    public Transform obstacleBottomPos;
    public GameObject obstaclePillarPrefab;
    public GameObject obstacleTrashPrefab;
    public GameObject obstacleBoxPrefab;
    public ObstacleSO.ESpawnType currentObjType = 0;
    ObstacleSO.ESpawnPosition currentPosType = 0;
    List<GameObject> obstacleGOList = new List<GameObject>();
    Animator backgroundAnimator;

    //float MIN_SPEED_MODIFIER = 0.02f;
    //float MAX_SPEED_MODIFIER = 0.07f;
    private float BASE_obstacleSpeed = 1.9f;
    private float obstacleSpeedModifier = 1.9f;
    public float ObstacleSpeedModifier => obstacleSpeedModifier;
    private Cooldown spawnCooldown = new(2.0f);

    private void Awake()
    {
        backgroundAnimator = gameManager.backgroundAnimator;
    }
    public void SpawnObstacleInCooldown()
    {
        if (!spawnCooldown.IsCooldown)
        {
            SpawnObstacle(GetRandomizedObstaclePrefab());
            spawnCooldown.StartCooldown();
        }
    }

    GameObject GetRandomizedObstaclePrefab()
    {
        int enumLength = Enum.GetValues(typeof(ObstacleSO.ESpawnType)).Length;
        currentObjType = (ObstacleSO.ESpawnType)UnityEngine.Random.Range(0, enumLength);
        switch (currentObjType)
        {
            case ObstacleSO.ESpawnType.Pillar:
                return obstaclePillarPrefab;
            case ObstacleSO.ESpawnType.Trash:
                return obstacleTrashPrefab;
            case ObstacleSO.ESpawnType.Box:
                return obstacleBoxPrefab;
            default:
                Debug.LogWarning("Undefined Enum Prefab!");
                return null;
        }
    }
    Transform GetSpawnPosition(GameObject _obstacleGO)
    {
        ObstacleSO obstacleSO = _obstacleGO.GetComponent<ObstacleBehaviour>().obstacleSO;
        currentPosType = obstacleSO.mySpawnPos;
        switch (currentPosType)
        {
            case ObstacleSO.ESpawnPosition.Top:
                return obstacleTopPos;
            case ObstacleSO.ESpawnPosition.Middle:
                return obstacleMiddlePos;
            case ObstacleSO.ESpawnPosition.Bottom:
                return obstacleBottomPos;
            default:
                Debug.LogWarning("Undefined Enum SpawnPos!");
                return transform;
        }
    }


    void SpawnObstacle(GameObject obstacleGO)
    {
        Transform obsTransform = GetSpawnPosition(obstacleGO);
        GameObject instantiatedObstacle = Instantiate(obstacleGO, obsTransform.position, obsTransform.rotation, obsTransform);
        obstacleGOList.Add(instantiatedObstacle);
        Destroy(instantiatedObstacle, 20.0f);
    }

    public void UpdateObstacles()
    {
        obstacleGOList.RemoveAll(go => go == null);
        foreach (GameObject go in obstacleGOList)
        {
            if (go) MoveGOToLeftScreen(go);
            Vector3 posInViewport = Camera.main.WorldToViewportPoint(go.transform.position);
            if (posInViewport.x <= 0)
            {
                Destroy(go);
            }

        }
    }


    void MoveGOToLeftScreen(GameObject go)
    {
        go.transform.position += obstacleSpeedModifier * Vector3.left * Time.deltaTime; //   
    }
    public void SpeedUpObstacle(float addedSpeed)
    {
        obstacleSpeedModifier = BASE_obstacleSpeed + addedSpeed;
        backgroundAnimator.SetFloat("moveSpeed", 0.1f * obstacleSpeedModifier);
    }
    public void RevertObstacleSpeed()
    {
        obstacleSpeedModifier = BASE_obstacleSpeed;
        backgroundAnimator.SetFloat("moveSpeed", 0.1f * obstacleSpeedModifier);
    }

}
