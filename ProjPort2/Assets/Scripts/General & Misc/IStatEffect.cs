using UnityEngine;

public interface IStatEff
{
    public void fire(float time, int hpRate);

    public void slow(float time, float slowAmount);

    public void damageUP(float time, int damageAmount);

    public void speedUP(float time, float speedAmount);

    public void jumpUP(float time, float jumpAmount);

    public void jumpDouble(float time, int jumpAdd);

    public void healthUP(float time, int healthAmount);

    public void drunk(float time, int drunkStacks);
}
