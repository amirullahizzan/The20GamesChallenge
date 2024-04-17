using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bulletSpawnerPos;
    public GameManager gameManager;
    [SerializeField] PlayerStat playerStat;

    AudioSource audioSource;
    Rigidbody2D rb;
    public BulletSpawner bulletSpawner = new();
    float upwardSpeedMult = 1200f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

  

    private void FixedUpdate()
    {
        if (gameManager.IsPressed && gameManager.IsGameStart)
        {
            rb.AddForce(Vector2.up * upwardSpeedMult * Time.deltaTime);
            if (!bulletSpawner.cooldown.IsCooldown) SpawnBullet();
        }
    }

    void SpawnBullet()
    {
        GameObject instantiatedBullet = Instantiate(bulletSpawner.bulletPrefab, bulletSpawnerPos.transform.position, bulletSpawnerPos.transform.rotation);
        Destroy(instantiatedBullet, 2.0f);
        bulletSpawner.ShootBullet(instantiatedBullet);
        bulletSpawner.cooldown.StartCooldown();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            ObstacleBehaviour obsBehaviour = other.gameObject.GetComponent<ObstacleBehaviour>();
            audioSource.PlayOneShot(obsBehaviour.hitSFX, 0.8f);
            Destroy(other.gameObject);
            playerStat.LifePoint--;
            playerStat.UpdateLifePointText();
            playerStat.CheckDeath(gameManager);
        }

    }
}

[System.Serializable]
public class BulletSpawner
{
    public Cooldown cooldown = new(0.02f);
    public AudioClip bulletShotSFX;
    public AudioSource audioSource;
    public GameObject bulletPrefab;
    Cooldown audioShootCooldown = new(0.08f);
    private void PlayReplaceShootAudio()
    {
        if (!audioShootCooldown.IsCooldown)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(bulletShotSFX, 0.3f);
            audioShootCooldown.StartCooldown();
        }
    }
    public void ShootBullet(GameObject bulletInstance)
    {
        PlayReplaceShootAudio();
        Rigidbody2D bulletRb = bulletInstance.GetComponent<Rigidbody2D>();
        float randomXDir = UnityEngine.Random.Range(-0.8f, 0.8f);
        Vector2 downDir = new Vector2(randomXDir, -1);
        const float bulletForce = 75000.0f;
        bulletRb.AddForce(downDir * bulletForce * Time.deltaTime);
    }

}

[System.Serializable]
class PlayerStat
{
    public TMP_Text lifePointText;
    int lifePoint = 3;
    public int LifePoint
    {
        get => lifePoint;
        set => lifePoint = value;
    }
    public void UpdateLifePointText()
    {
        lifePointText.text = "Life Point: " + LifePoint;
    }
    public void CheckDeath(GameManager thisGameManager)
    {
        if (LifePoint <= 0)
        {
            thisGameManager.OnPlayerDeath();
        }
    }
}