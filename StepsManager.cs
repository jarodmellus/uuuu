using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;
using System.Linq;

public class StepsManager : MonoBehaviour
{
    [SerializeField]
    string activityName;
    int currentStep = -1;
    public int CurrentStep => currentStep;
    List<GameObject> steps = new List<GameObject>();

    [SerializeField]
    bool startOnEnable = true;

    public UnityEvent onNext, onPrevious, onStart, onQuit, onComplete;

    void Awake()
    {
        GenerateSteps();
    }

    void OnEnable()
    {
        if(startOnEnable) {
            Begin();
        }
    }

    void OnDisable()
    {
        //Quit();
    }

    public void Begin(){
        currentStep = -1;
        Next();
        onStart?.Invoke();
        
        if(activityName.Length>0)
            LabReport.Instance?.AddWithTimeStamp("Began " + activityName);
    }

    [Button]
    public void Quit() {
        foreach(GameObject step in steps) {
            step.SetActive(false);
        }

        currentStep = -1;
        onQuit?.Invoke();

        if(activityName.Length>0)
            LabReport.Instance?.AddWithTimeStamp("Quit " + activityName);
    }

#if UNITY_EDITOR
    [Button("Next")]
    public void EditorNext() {
        GenerateSteps();
        Next();
    }

    [Button("Previous")]
    public void EditorPrevious() {
        GenerateSteps();
        Previous();
    }
#endif

    void GenerateSteps() {
        if(!gameObject.activeInHierarchy) return;

        steps = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<StepManagerIgnore>()==null)
            {
                GameObject cur = transform.GetChild(i).gameObject;
                steps.Add(cur);
                cur.SetActive(false);
            }
        }
    }

    public void Next()
    {
        if (!gameObject.activeInHierarchy) return;

        if (currentStep >= steps.Count)
        {
            return;
        };

        if (currentStep != -1)
        {
            steps[currentStep].SetActive(false);
            if (activityName.Length > 0)
            {
                LabReport.Instance?.AddWithTimeStamp($"[{activityName}] Completed step labeled: \" {steps[currentStep].name}\"");
            }
        }



        currentStep++;
        if (currentStep != steps.Count)
        {
            steps[currentStep].SetActive(true);    
            if(currentStep==0)
                onStart?.Invoke();
        }
        else
        {
            onComplete?.Invoke();

            if(activityName.Length>0)
                LabReport.Instance?.AddWithTimeStamp("Completed " + activityName);

            Quit();
        }

        onNext?.Invoke();
    }

    public void Previous() {
        if(!gameObject.activeInHierarchy) return;

        if(currentStep==-1) return;

        OnStepPrevious onStepPrevious;
        if(steps[currentStep].TryGetComponent<OnStepPrevious>(out onStepPrevious))
           onStepPrevious.onStepPrevious?.Invoke();

        if(currentStep!=transform.childCount)
            steps[currentStep].SetActive(false);
        currentStep--;
        if(currentStep!=-1)
            steps[currentStep].SetActive(true);
        else
            Quit();

        onPrevious?.Invoke();
    }

    [Button]
    public void Restart() {
        steps[currentStep].SetActive(false);
        onQuit?.Invoke();
        currentStep = 0;
        steps[0].gameObject.SetActive(true);
        onStart?.Invoke();
    }
}
