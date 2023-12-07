using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class MonoEffectSelection : MonoBehaviour {

    [SerializeField] int effectIndex=0;
    public void SetEffectIndex(int index) => effectIndex = index;

    [System.Serializable]
    public class TriggerData {
        public string label="";
        public UnityEvent onTrigger;

        public TriggerData(string _label, UnityEvent _event) {
            label = _label;
            onTrigger = _event;
        }
    }
    [SerializeField] List<TriggerData> effects = new List<TriggerData>();

    Dictionary<string, int> stringToIndexMap = new Dictionary<string, int>(); 
    Dictionary<string, TriggerData> stringToDataMap = new Dictionary<string, TriggerData>(); 
    void Awake() {
        foreach(TriggerData effect in effects) {
            AddEffect(effect.label,effect.onTrigger,true);
        }

        if(effects.Count>0) {
            currentEffect = effects[0];
        }
    }

    public void AddEffect(string label, UnityEvent _event, bool alreadyInList = false) {
        TriggerData newData = new TriggerData(label, _event);
        if(!alreadyInList)
            effects.Add(newData);

        stringToIndexMap.Add(label, effects.Count-1);
        stringToDataMap.Add(label, newData);
    }

    //public void RemoveEffect(string label) {

    //}

    TriggerData currentEffect;

    [ExecuteAlways]
    [Button]
    public void Trigger() {

        if(!gameObject.activeInHierarchy) return;
        currentEffect.onTrigger?.Invoke();
    }

    public void Trigger(int newIndex) {
        SetIndex(newIndex);
        Trigger();
    }

    public void Trigger(string label) {
        SetIndex(label);
        Trigger();
    }

    public void SetIndex(int i) {
        if (effectIndex >= effects.Count) return;
        currentEffect = effects[i];
        effectIndex = i;
    }

    public void SetIndex(string label) {
        if (stringToIndexMap.ContainsKey(label))
        {
            currentEffect = stringToDataMap[label];
            effectIndex = stringToIndexMap[label];
        }
        else
        {
            currentEffect = null;
            effectIndex = -1;
        }
    }
}