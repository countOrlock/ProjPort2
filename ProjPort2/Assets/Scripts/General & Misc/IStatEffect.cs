using UnityEngine;

public interface IStatEff
{
    public void fire(float time, int hpRate);

    public void slow(float time, float slowAmount);
}
