using UnityEngine;

[CreateAssetMenu]

public class throwStats : ScriptableObject
{
    public GameObject projectile = null;
    public GameObject animObject = null;

    public string itemName;

    public int ammoCurr;
    [Range(1, 50)] public int ammoMax;

    [Header("----- Power Up -----")]
    public bool isPowerUP;
    public bool Drunk;
    public float drunkTime;
    public enum powerUpType { damage, speed, jumpHeight, doubleJump, healing }
    public powerUpType _powerUpType;
    public float powerUpTime;
    public float powerUpAmount;
    public int powerUpAmountInt;

    public AudioClip[] throwSound;
    public AudioClip[] pickupSound;
    [Range(0, 1)] public float throwSoundVol;
    [Range(0, 1)] public float pickupSoundVol;
}
