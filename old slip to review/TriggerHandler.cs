using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public delegate void E_OnTriggerEnter();
    public delegate void E_OnTriggerExit();
    public delegate void E_OnTriggerStay();
    public event E_OnTriggerEnter onTriggerEnter;
    public event E_OnTriggerExit onTriggerExit;    
    public event E_OnTriggerStay onTriggerStay;

    private void OnTriggerEnter(Collider other)
    {
        if(onTriggerEnter!=null)    
            onTriggerEnter.Invoke();  
        
    }

    private void OnTriggerStay(Collider other)
    { 
        if(onTriggerStay!=null)    
            onTriggerStay.Invoke();   
    }

    private void OnTriggerExit(Collider other)
    {
        if(onTriggerExit!=null)     
            onTriggerExit.Invoke();
    }

}
