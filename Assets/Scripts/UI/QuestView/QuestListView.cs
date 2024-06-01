using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class QuestListView : MonoBehaviour
{
    //현재 가진 Quest들을 Text Button으로 보여주는 요소
    [SerializeField]
    private TextMeshProUGUI elementTextPrefab;

    private Dictionary<Quest, GameObject> elementsByQuest = new Dictionary<Quest, GameObject>();
    private ToggleGroup toggleGroup;

    private void Awake()
    {
        toggleGroup = GetComponent<ToggleGroup>();
    }

    public void AddElement(Quest quest, UnityAction<bool> onClicked)
    {
        var element = Instantiate(elementTextPrefab, transform);
        element.text = quest.DisplayName;

        var toggle = element.GetComponent<Toggle>();
        toggle.group = toggleGroup;
        toggle.onValueChanged.AddListener(onClicked);
        
        elementsByQuest.Add(quest, element.gameObject);
    }

    public void RemoveElement(Quest quest)
    {
        Destroy(elementsByQuest[quest]);
        elementsByQuest.Remove(quest);
    }
}
