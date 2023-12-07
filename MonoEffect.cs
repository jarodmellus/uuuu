using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TelegramSystem;
using UnityEngine.Events;
using System;
using NaughtyAttributes;

public class MonoEffect : MonoBehaviour
{
    PhotonView photonView;
    public UnityEvent onTrigger;
    
    void Awake() {
        photonView = GetComponent<PhotonView>();
        //Telegram.Register("monoEffect_" + name, photonView, new Action(TriggerRPC));
    }


    //these effects should probably not be triggered by rpc, but be 
    // effects resulting from rpcs
    [ExecuteAlways]
    [Button]
    public void Trigger() {

        if(!gameObject.activeInHierarchy) return;

        //photonView.SendTelegram("monoEffect_" + name, RpcTarget.All);
        onTrigger?.Invoke();
    }

    void TriggerRPC() {
        onTrigger?.Invoke();
    }
}
