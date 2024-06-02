using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Ammo", fileName = "AmmoReward_")]
public class AmmoReward : Reward
{
    [SerializeField] private AmmoRewardType ammoRewardType;
    
    public override void Give(Quest quest)
    {
        Controller.Instance.AddAmmo((int)ammoRewardType, Quantity);
    }
}

public enum AmmoRewardType
{
    HandGun = 0,
    Blast = 1,
    Grenade = 2
}