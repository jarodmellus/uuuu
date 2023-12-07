using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ValueCurve : MonoBehaviour
{
    [SerializeField]
    float animationTime = 0f;
    [SerializeField]
    AnimationCurve scaleCurve;
    [SerializeField]
    float animationDuration=1f;
    [SerializeField]
    bool playOnEnable = false;
    [SerializeField]
    bool resetValueOnDisable = false;
    [SerializeField]
    bool loop = false;

    public UnityEvent<float> onValueChange;
    public UnityEvent onZero, onOne;

    void OnEnable()
    {
        if(playOnEnable)
            StartCoroutine(PlayForwardRoutine());
    }
    void OnDisable()
    {
        StopAllCoroutines();
        if (resetValueOnDisable)
            ResetValue();
    }

    public void PlayForward() {
        StopAllCoroutines();
        StartCoroutine(PlayForwardRoutine());
    }

    public void Restart() {
        animationTime=0f;
        PlayForward();
    }
    
    public void PlayBackward() {
        StopAllCoroutines();
        StartCoroutine(PlayBackwardRoutine());  
    }

    public void Pause() {
        StopAllCoroutines();
    }

    public void ResetValue() {
        animationTime = 0f;
    }

    public void ResetAndPause() {
        StopAllCoroutines();
        animationTime = 0f;
    }
    

    IEnumerator PlayForwardRoutine() {
        do {
            while(animationTime<1f) {
                float val = scaleCurve.Evaluate(animationTime);
                animationTime += Time.deltaTime / animationDuration;
                onValueChange?.Invoke(val);
                yield return new WaitForEndOfFrame();
            }

            onOne?.Invoke();
            if(loop) {
                animationTime = 0f;
            }
        } while(loop);

        
        animationTime = 1f;
        onValueChange?.Invoke(scaleCurve.Evaluate(animationTime));
        onOne?.Invoke();
    }

    IEnumerator PlayBackwardRoutine() {
        do {
            while(animationTime>0f) {
                float val = scaleCurve.Evaluate(animationTime);
                if(animationDuration<=0) yield break;
                animationTime -= Time.deltaTime / animationDuration;
                onValueChange?.Invoke(val);
                yield return new WaitForEndOfFrame();
            }

            onZero?.Invoke();
            if(loop) {
                animationTime = 1f;
            }
        } while(loop);

        animationTime = 0f;
        onValueChange?.Invoke(scaleCurve.Evaluate(animationTime));
        
    }
}

