using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    public GameObject Bullet = null;
    public bool shootLaser;

    [Range(0, 10)] public int shootDamage;
    [Range(0, 1000)] public int shootDist;
    [Range(0f, 4)] public float shootRate;
    [Range(0.1f, 10)] public float reloadRate;
    [Range(0, 50)] public int recoil;
    public int ammoCur;
    [Range(1, 200)] public int ammoMax;
    public int magsCur;
    [Range(1, 70)] public int magsMax;
    [Range(0f, 1f)] public float zoomMod;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public AudioClip[] reloadSound;
    [Range(0, 1)] public float shootSoundVol;
    [Range(0, 1)] public float reloadSoundVol;
}
