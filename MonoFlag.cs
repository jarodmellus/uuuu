using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using TelegramSystem;
using Photon.Pun;
using System;

public class MonoFlag : MonoBehaviourPun
{
    
    public UnityEvent onCheckTrue, onCheckFalse, onSetTrue, onSetFalse;
    [SerializeField]
    bool condition = false;
    [SerializeField]
    bool keepValueOnDisable = false;

    void OnEnable()
    {
        if(!keepValueOnDisable)
            condition=initialValue;
    }

    void Start()
    {
        //Telegram.Register("monoflag_set", photonView, new Action<bool>(SetRPC));
        //Telegram.Register("monoflag_check", photonView, new Action<bool>(CheckRPC));
    }

    public bool Get() {
        return condition;
    }

    void SetRPC(bool value) {
        if(value == condition) return;

        condition=value;
        if(condition)
            onSetTrue?.Invoke();
        else    
            onSetFalse?.Invoke();
    }

    public void Set(bool value) {
        if(!gameObject.activeInHierarchy) return;

        SetRPC(value);

        //photonView.SendTelegram("monoflag_set",RpcTarget.Others, value);
    }

    public void Toggle() {
        Set(!condition);
    }

    public void CheckAllListeners() {
        if(!gameObject.activeInHierarchy) return;

        if(condition) {
            onCheckTrue?.Invoke();
        }
        else {
            onCheckFalse?.Invoke();
        }
    }

/*
    [NaughtyAttributes.Button]
    public void Set() {
        Set(setValue);
    }
    [SerializeField] bool setValue;
  */

    public bool initialValue;
    public void ResetFlag() {
        Set(initialValue);
    }

    [NaughtyAttributes.Button]
    public void EditorSetTrue() {
        Set(true);
    }

    [NaughtyAttributes.Button]
    public void EditorSetFalse() {
        Set(false);
    }
}
