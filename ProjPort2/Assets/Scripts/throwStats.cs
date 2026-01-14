using UnityEngine;

[CreateAssetMenu]

public class throwStats : ScriptableObject
{
    public GameObject projectile = null;
    public GameObject animObject = null;

    public string itemName;

    public int ammoCurr;
    [Range(1, 50)] public int ammoMax;

    public AudioClip[] throwSound;
    public AudioClip[] pickupSound;
    [Range(0, 1)] public float throwSoundVol;
    [Range(0, 1)] public float pickupSoundVol;
}
