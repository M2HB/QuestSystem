using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    private Quest[] quests;

    private QuestSystem questSystem;

    private void Start()
    {
        questSystem = QuestSystem.Instance;
        foreach (var quest in quests)
        {
            if (quest.IsAcceptable && !QuestSystem.Instance.ContainsInCompleteQuests(quest))
            {
                questSystem.Register(quest);
            }
        }
        
        QuestCompletedEventRegister();
    }

    private void GiveNextQuest(Quest completedQuest)
    {
        completedQuest.onCompleted -= GiveNextQuest;

        foreach (var quest in quests)
        {
            if (questSystem.ActiveQuests.Any(x => x.CodeName == quest.CodeName)) continue;
            
            //Debug.Log($"####### Quest {quest.name} => IsAcceptable {quest.IsAcceptable} Completetable {quest.IsComplete}");
            if (quest.IsAcceptable && !QuestSystem.Instance.ContainsInCompleteQuests(quest))
            {
                QuestSystem.Instance.Register(quest);
            }
        }
        
        QuestCompletedEventRegister();
    }

    private void QuestCompletedEventRegister()
    {
        var activeQuests = questSystem.ActiveQuests;
        foreach (var activeQuest in activeQuests)
        {
            activeQuest.onCompleted += GiveNextQuest;
        }
    }
}
