using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoStateActivity : MonoBehaviour
{
    public List<MonoStateController> activities = new List<MonoStateController>();

    public void Begin() {
        for(int i = 0; i < activities.Count; i++) {
            if(i!=activities.Count-1) {
                activities[i].onExitState.AddListener(()=>NextActivity(activities[i],activities[i+1]));
            }
        }
    }

    public void NextActivity(MonoStateController from, MonoStateController to) {
        from.gameObject.SetActive(false);
        to.gameObject.SetActive(true);
        to.Restart();
    }
}
