using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestReporter : MonoBehaviour
{
    [SerializeField]
    private Category category;

    [SerializeField]
    private TaskTarget target;

    [SerializeField]
    private int successCount;

    [SerializeField]
    private string[] colliderTags;

    private void OnTriggerEnter(Collider other)
    {
        ReportIfPassCondition(other);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ReportIfPassCondition(collision);
    }

    public void Report()
    {
        Debug.Log($"####### Report To Quest System Category => {category}, Target=>{target}, SuccessCount=>{successCount}");
        QuestSystem.Instance.ReceiveReport(category, target,successCount);
    }

    private void ReportIfPassCondition(Component other)
    {
        Debug.Log($"####### PassingCondition {colliderTags.Any()} TargetTag{other.tag}");
        if (colliderTags.Any(x => other.CompareTag(x)))
        {
            Debug.Log($"####### Compare True {other.tag}");
            Report();
        }

    }

}
