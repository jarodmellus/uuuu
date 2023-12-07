using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[DescriptionAttribute("Keep track of all gameObjects with script of type T as they enter and exit a trigger Area")]
public class TriggerArea : MonoBehaviour
{
    public UnityEvent<TriggerAreaMember> onAdded, onRemoved, onChange;
    [SerializeField]
    List<TriggerAreaMember> members = new List<TriggerAreaMember>();
    public List<TriggerAreaMember> GetList => members;

    protected virtual void OnTriggerEnter(Collider other)
    {
        TriggerAreaMember cur;

        if(other.attachedRigidbody==null) return;
        if(!other.attachedRigidbody.TryGetComponent<TriggerAreaMember>(out cur)) return;
        
        Add(cur);
        
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        TriggerAreaMember cur;
        if(other.attachedRigidbody==null) return;
        if(!other.attachedRigidbody.TryGetComponent<TriggerAreaMember>(out cur)) return;
        
        Remove(cur);
        
    }

    protected void Add(TriggerAreaMember t) {
        if(!members.Contains(t)) {
            members.Add(t);
            onChange?.Invoke(t);
            onAdded?.Invoke(t);
        }
    }

    protected void Remove(TriggerAreaMember t) {
        if(members.Contains(t)) {
            members.Remove(t);
            onChange?.Invoke(t);
            onRemoved?.Invoke(t);
        } 
    }

    public void ForceRemoveMember(TriggerAreaMember t) {
        if(!members.Contains(t)) return;
        members.Remove(t);
        onChange?.Invoke(t);
        onRemoved?.Invoke(t);
        
    }

    public void ForceRemoveAll() {
        members.Clear();
    }

    public bool CheckIfIsMember(TriggerAreaMember t) {
        return members.Contains(t);  
    }

    public List<TriggerAreaMember> GetMembers() {
        return members;
    }


}
