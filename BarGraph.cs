using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VXRLabs {
    public class BarGraph : MonoBehaviour
    {
        public HorizontalLayoutGroup layoutGroup;
        [SerializeField] BarGraphBar barPrefab;
        Dictionary<string,BarGraphBar> elements = new Dictionary<string,BarGraphBar>();
        public float barMultiplier=1f;
        [HideInInspector]
        public float barScale = 1f;
        public float lerpTime=.9f;
        public float barWaitTime = .02f;

        void OnEnable() {
            _changeMadeThisFrame = true;
        }


        public void AddElement(string id, float val, string label) {
            if(elements.ContainsKey(id)) return;
            BarGraphBar newBar = Instantiate(barPrefab,layoutGroup.transform);
            //BarGraphBar newBar = newBarObj.GetComponent<BarGraphBar>();
            newBar.SetGraph(this);
            elements.Add(id, newBar);
            ChangeElement(id,val,label);
        }

        public void AddElements(params (string id, float val, string label)[] elems) {
            foreach((string id, float val, string label) elem in elems) {
                AddElement(elem.id,elem.val,elem.label);
            }
        }

        public void AddElements(params (string id, float val)[] elems) {
            foreach((string id, float val) elem in elems) {
                AddElement(elem.id,elem.val,elem.id);
            }
        }

        public void ChangeElements(params (string id, float val, string label)[] elems) {
            foreach((string id, float val, string label) elem in elems) {
                ChangeElement(elem.id,elem.val,elem.label);
            }            
        }

        public void ChangeElements(params (string id, float val)[] elems) {
            foreach((string id, float val) elem in elems) {
                ChangeElement(elem.id,elem.val,elem.id);
            }            
        }

        public void RemoveElement(string id) {
            if(!elements.ContainsKey(id)) return;
            Destroy(elements[id]);
            elements.Remove(id);
        }

        public void ChangeElement(string id, float val, string label) {
            if(!elements.ContainsKey(id)) return;
            elements[id].SetValue(val);
            elements[id].SetLabel(label);
            _changeMadeThisFrame=true;
        }

        bool _changeMadeThisFrame;

        void LateUpdate() {
            if(!_changeMadeThisFrame) return;
            _changeMadeThisFrame=false;
            float max = elements.Select(x => x.Value.Value).Max();
            barScale = (max==0f) ? 0f : (1f / (max)) * barMultiplier;
            foreach(BarGraphBar bar in elements.Values) {
                bar.UpdateBar();
            }
            
        }


    }
}
