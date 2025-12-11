using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    public GameObject Bullet = null;

    [Range(0, 10)] public int shootDamage;
    [Range(0, 1000)] public int shootDist;
    [Range(0.1f, 4)] public float shootRate;
    [Range(0, 50)] public int recoil;
    public int ammoCur;
    [Range(5, 50)] public int ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootSoundVol;


}
