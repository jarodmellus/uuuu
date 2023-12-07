using System.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using NaughtyAttributes;

public class MonoCondition : MonoBehaviour
{
    
    [SerializeField] MonoFlag flagToSet;

    public UnityEvent onCondition;
    
    public enum _Operator {
        NONE,
        AND,
        OR
    }

    public List<MonoFlag> flags = new List<MonoFlag>();
    [SerializeField]
    [NaughtyAttributes.EnumFlags]
    _Operator _operator = _Operator.AND;

    [SerializeField] bool getFlagsFromChildren = false;

    public void OnEnable() {
        if(getFlagsFromChildren) GetFlagsFromChildren();
        SubscribeToFlags();
    }

    public void OnDisable() {
        UnsubscribeFromFlags();
    }

    [Button]
    public void GetFlagsFromChildren() {
        flags.Clear();
        MonoFlag[] _flags = GetComponentsInChildren<MonoFlag>();
        flags = _flags.ToList();
        if (flags.Contains(flagToSet)) flags.Remove(flagToSet);

    }

    public void SubscribeToFlags() {
        if (flagToSet != null)
        {
            flags.Remove(flagToSet);
        }

        foreach(MonoFlag flag in flags) {
     
            //if(flag==null) continue;

            flag.onSetFalse.AddListener(Check);
            flag.onSetTrue.AddListener(Check);
            //flag.onCheckTrue.AddListener(Check);
            //flag.onCheckFalse.AddListener(Check);
        }
    }

    public void UnsubscribeFromFlags() {
        foreach(MonoFlag flag in flags) {
            //if(flag==null) continue;

            flag.onSetFalse.RemoveListener(Check);
            flag.onSetTrue.RemoveListener(Check);
            //flag.onCheckTrue.RemoveListener(Check);
            //flag.onCheckFalse.RemoveListener(Check);
            
        }
    }

    public void CheckAllListeners() {
        foreach(MonoFlag mf in flags) {
            //if(mf==null) continue;
            mf.CheckAllListeners();
        }
    }

    public void Check() {
        bool result = false;
        switch(_operator) {
            case _Operator.NONE:
                result=flags.All(c => c.Get() == false);
                break;
            case _Operator.AND:
                result=flags.All(c => c.Get() == true);
                break;
            case _Operator.OR:
                result=flags.Any(c=> c.Get() == true);
                break;
        }

        if(result) {
            onCondition?.Invoke();
        }

        if(flagToSet!=null)
            flagToSet.Set(result);
    }

}

