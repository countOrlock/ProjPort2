using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    [Header("----- Gun Fire1 -----")]
    public Mesh gunMesh;
    public Mesh slideMesh;
    public Mesh hammerMesh;
    public Mesh magMesh;
    public Material gunMaterial;
    public GameObject Bullet = null;
    public bool shootLaser;

    [Range(0, 10)] public int shootDamage;
    [Range(0, 1000)] public int shootDist;
    [Range(0f, 4)] public float shootRate;
    [Range(0.1f, 10)] public float reloadRate;
    [Range(0, 50)] public int recoil;
    public int ammoCur;
    [Range(1, 2000)] public int ammoMax;
    public int magsCur;
    [Range(0, 70)] public int magsMax;
    [Range(0f, 1f)] public float zoomMod;

    public ParticleSystem hitEffect;
    public ParticleSystem shootEffect;
    public AudioClip[] shootSound;
    public AudioClip[] missFireSound;
    public AudioClip[] reloadSound;
    [Range(0, 1)] public float shootSoundVol;
    [Range(0, 1)] public float missFireSoundVol;
    [Range(0, 1)] public float reloadSoundVol;

    [Header("----- Gun Fire2 -----")]
    public bool HasSecondary;
    public GameObject Bullet2 = null;
    public bool shootLaser2;
    [Range(0, 10)] public int shootDamage2;
    [Range(0, 1000)] public int shootDist2;
    [Range(0f, 4)] public float shootRate2;

    public ParticleSystem hitEffect2;
    public ParticleSystem shootEffect2;
    public AudioClip[] shootSound2;
    [Range(0, 1)] public float shootSoundVol2;

    [Header("-----Animations-----")]
    public AnimatorOverrideController gunAnims;
}
