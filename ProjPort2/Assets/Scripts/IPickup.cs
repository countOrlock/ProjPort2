using UnityEngine;

public interface IPickup
{
    public void getGunStats(gunStats gun);

    public void getQuestItem(GameObject quest);

    public void getThrowStats(throwStats item);
}
