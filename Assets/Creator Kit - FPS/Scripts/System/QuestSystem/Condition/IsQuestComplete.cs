using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Condition/IsQuestComplete", fileName = "IsQuestComplete_")]
public class IsQuestComplete : Condition
{
    [SerializeField]
    private Quest target;

    public override bool IsPass(Quest quest) => QuestSystem.Instance.ContainsInCompleteQuests(target);
}
