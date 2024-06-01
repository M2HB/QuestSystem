using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;


// 타겟을 알려주는 용도
// 퀘스트를 알려주는 물음표 기능도 가능

public class QuestTargetMarker : MonoBehaviour
{
    //타겟을 가진 태스크를 찾아와 감시
    [SerializeField]
    private TaskTarget target;

    [SerializeField]
    private MakerMaterialData[] makerMaterialDatas;

    //마커를 표시해야 할 퀘스트와 테스크를 저장하는 딕셔너리
    private Dictionary<Quest, Task> targetTasksByQuest = new Dictionary<Quest, Task>();
    
    //마커가 항상 플레이어를 보게 하기 위한 Transform
    private Transform cameraTransform;
    
    //카테고리따라 이미지를 다르게 보여주기위한 렌더러
    private Renderer renderer;
    
    //진행중인 테스크의 카운트
    private int currentRunningTargetTaskCount;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        
        QuestSystem.Instance.onQuestRegistered += TryAddTargetQuest;
        foreach (var quest in QuestSystem.Instance.ActiveQuests)
        {
            TryAddTargetQuest(quest);
        }
    }

    private void Update()
    {
        var rotation = Quaternion.LookRotation((cameraTransform.position - transform.position).normalized);
        transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y + 180f, 0);
    }

    private void OnDestroy()
    {
        QuestSystem.Instance.onQuestRegistered -= TryAddTargetQuest;
        foreach ((Quest quest, Task task) in targetTasksByQuest)
        {
            quest.onNewTaskGroup -= UpdateTargetTask;
            quest.onCompleted -= RemoveTargetQuest;
            task.onStateChanged -= UpdateRunningTargetTaskCount;
        }
    }

    //등록된 퀘스트를 확인하여 Target일경우 감시하는 함수
    private void TryAddTargetQuest(Quest quest)
    {
        if (target != null && quest.ContainsTarget(target))
        {
            quest.onNewTaskGroup += UpdateTargetTask;
            quest.onCompleted += RemoveTargetQuest;
            
            UpdateTargetTask(quest, quest.CurrentTaskGroup);
        }
    }



    //감시중인 테스크를 교체하는 함수
    private void UpdateTargetTask(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        targetTasksByQuest.Remove(quest);
        var task = currentTaskGroup.FindTaskByTarget(target);
        if (task != null)
        {
            targetTasksByQuest[quest] = task;
            task.onStateChanged += UpdateRunningTargetTaskCount;
            
            UpdateRunningTargetTaskCount(task, task.State);
        }
    }

    //퀘스트완료시 타켓에서 지워주는 함수 
    private void RemoveTargetQuest(Quest quest) => targetTasksByQuest.Remove(quest);
    

    //Task의 상태에 따라 카운트를 조절하고 카운트가 0이면 마커를 끄고 0이상이면 마커를 켜주는 역할
    private void UpdateRunningTargetTaskCount(Task task, TaskState currentState, TaskState prevState = TaskState.Inactive)
    {
        if (currentState == TaskState.Running)
        {
            renderer.material = makerMaterialDatas.First(x => x.category == task.Category).makerMaterial;
            currentRunningTargetTaskCount++;
        }
        else
        {
            currentRunningTargetTaskCount--;
        }
        
        gameObject.SetActive(currentRunningTargetTaskCount !=0);
    }
    
    //카테고리에 따라 Material이 다른 마커 데이터
    [Serializable]
    private struct MakerMaterialData
    {
        public Category category;
        public Material makerMaterial;
    }

}
