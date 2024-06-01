using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI questTitleText;

    [SerializeField]
    private TaskDescriptor taskDescriptorPrefab;

    private Dictionary<Task, TaskDescriptor> taskDescriptorByTask = new Dictionary<Task, TaskDescriptor>();

    private Quest targetQuest;

    public void Setup(Quest targetQuest, Color titleColor)
    {
        this.targetQuest = targetQuest;

        questTitleText.text = targetQuest.Category == null
            ? targetQuest.DisplayName
            : $"[{targetQuest.Category.DisplayName}] {targetQuest.DisplayName}";

        questTitleText.color = titleColor;

        targetQuest.onNewTaskGroup += UpdateTaskDescriptos;
        targetQuest.onCompleted += DestroySelf;

        var taskGroups = targetQuest.TaskGroups;
        UpdateTaskDescriptos(targetQuest, taskGroups[0]);

        if (taskGroups[0] != targetQuest.CurrentTaskGroup)
        {
            for (int i = 1; i < taskGroups.Count; i++)
            {
                var taskGroup = taskGroups[i];
                UpdateTaskDescriptos(targetQuest, taskGroup, taskGroups[i - 1]);

                if (taskGroup == targetQuest.CurrentTaskGroup)
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        if (targetQuest != null)
        {
            targetQuest.onNewTaskGroup -= UpdateTaskDescriptos;
            targetQuest.onCompleted -= DestroySelf;
        }

        foreach (var tuple in taskDescriptorByTask)
        {
            var task = tuple.Key;
            task.onSuccessChanged -= UpdateText;
        }
    }


    private void UpdateTaskDescriptos(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        foreach (var task in currentTaskGroup.Tasks)
        {
            var taskDescriptor = Instantiate(taskDescriptorPrefab, transform);
            taskDescriptor.UpdateText(task);
            task.onSuccessChanged += UpdateText;
            
            taskDescriptorByTask.Add(task,taskDescriptor);
        }

        if (prevTaskGroup != null)
        {
            foreach (var task in prevTaskGroup.Tasks)
            {
                var taskDescriptor = taskDescriptorByTask[task];
                taskDescriptor.UpdateTextUsingStrikeThrough(task);
            }
        }
    }

    private void UpdateText(Task task, int currentSuccess, int prevSuccess)
    {
        taskDescriptorByTask[task].UpdateText(task);
    }

    private void DestroySelf(Quest quest)
    {
        Destroy(gameObject);
    }
}
