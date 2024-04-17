using UnityEngine;
[System.Serializable]
public class Cooldown
{
    public Cooldown(float initMaxCooldown )
    {
        maxCooldown = initMaxCooldown;
    }
    private float maxCooldown = 2.0f;
    //private float minCooldown = 0.9f;
    private float cooldownTick = 0.0f;
    public bool IsCooldown => Time.time < cooldownTick;
    public void SetNewCooldown(float newCooldownValue) => maxCooldown = newCooldownValue;
    public void StartCooldown() => cooldownTick = Time.time + maxCooldown;
}
