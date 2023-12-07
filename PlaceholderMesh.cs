using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEditor;
using NaughtyAttributes;

[ExecuteAlways]
public class PlaceholderMesh : MonoBehaviour
{
    public GameObject original;
    public GameObject copy;
    [SerializeField]
    List<GameObject> ignoreThese = new List<GameObject>();
    [SerializeField] Material placeholderMaterial, highLightMaterial;
    [HideInInspector]
    public PlacePoint placePoint;

    void Start()
    {
        
        if (original == null) return;
        if (copy == null) return;


        GenerateMesh();

        
        if((placePoint = GetComponent<PlacePoint>())!=null) {
            if(placePoint.startPlaced) {
            copy.SetActive(false);
            }
        }
    }

    public void OnHighlight() {
        ChangeMaterial(highLightMaterial);
    }

    public void OnStopHighlight() {
        ChangeMaterial(placeholderMaterial);
    }

    public void ChangeMaterial(Material material) {
        foreach(MeshRenderer rend in copy.GetComponentsInChildren<MeshRenderer>()) {
            rend.sharedMaterial = material;
        }
    }

    [NaughtyAttributes.Button("Preview Mesh")]
    public void GenerateMesh(bool copyColliders=false)
    {
        Transform _child;
        while (copy.transform.childCount != 0)
        {
            _child = copy.transform.GetChild(0);
            _child.SetParent(null);
            DestroyImmediate(_child.gameObject);
        }

        Component[] comps = copy.GetComponents(typeof(Component));
        foreach(Component comp in comps) {
            if(comp.GetType()==typeof(Transform)) continue;
            DestroyImmediate(comp);
        }

        bool initActive = copy.activeSelf;
        CopyObjectRecursive(original, copy, true, copyColliders);
        copy.SetActive(initActive);
    }

    public void CopyObjectRecursive(GameObject _original, GameObject _copy, bool isRoot=true, bool copyColliders=false)
    {
        if (_original == null) return;
        if (_copy == null) return;

        if(ignoreThese.Contains(_original)) return;

        if(isRoot) {
            //_copy.transform.position = _original.transform.position;
            //_copy.transform.rotation = _original.transform.rotation;
        }
        else {
            _copy.transform.localPosition = _original.transform.localPosition;
            _copy.transform.localRotation = _original.transform.localRotation;
            _copy.transform.localScale = _original.transform.localScale;
        }
        
         
        //_copy.transform.localScale = _original.transform.localScale;
        _copy.SetActive(_original.activeSelf);

        MeshRenderer origRend, copyRend;
        SkinnedMeshRenderer origSkinRend;
        MeshFilter origFilt, copyFilt;


        if (_original.TryGetComponent<MeshRenderer>(out origRend) && _original.TryGetComponent<MeshFilter>(out origFilt))
        {
            copyFilt = _copy.AddComponent<MeshFilter>();
            copyRend = _copy.AddComponent<MeshRenderer>();
            copyRend.enabled = origRend.enabled;

            int matsLen = origRend.sharedMaterials.Length;

            Material[] arr = new Material[matsLen];

            for (int k = 0; k < matsLen; k++)
            {
                arr[k] = placeholderMaterial;
            }

            copyRend.sharedMaterials = arr;

            copyFilt.sharedMesh = origFilt.sharedMesh;
        }
        else if(_original.TryGetComponent<SkinnedMeshRenderer>(out origSkinRend)) {
            copyFilt = _copy.AddComponent<MeshFilter>();
            copyRend = _copy.AddComponent<MeshRenderer>();
            copyRend.enabled = origSkinRend.enabled;

            int matsLen = origSkinRend.sharedMaterials.Length;

            Material[] arr = new Material[matsLen];

            for (int k = 0; k < matsLen; k++)
            {
                arr[k] = placeholderMaterial;
            }

            copyRend.sharedMaterials = arr;
            copyFilt.sharedMesh = origSkinRend.sharedMesh;
        }

        if(copyColliders) {
            Collider[] cols = _original.GetComponents<Collider>();
            int i = 0;
            foreach(Collider col in cols) {
               _copy.AddComponent(col.GetType());
               System.Reflection.FieldInfo[] fields = col.GetType().GetFields();
               foreach(System.Reflection.FieldInfo field in fields) {
                    field.SetValue(copy.GetComponent(col.GetType()), field.GetValue(col));
               }
               _copy.GetComponents<Collider>()[i].isTrigger=true;
               i++;
            }

        }

        GameObject origChild, copyChild;
        for (int i = 0; i < _original.transform.childCount; i++)
        {
            origChild = _original.transform.GetChild(i).gameObject;
            copyChild = new GameObject("Placeholder Mesh");
            copyChild.transform.SetParent(_copy.transform);
            CopyObjectRecursive(origChild, copyChild,false);
        }
    }

    internal void GenerateMeshAtRuntime()
    {
        StartCoroutine(RuntimeMeshRoutine());
    }

    IEnumerator RuntimeMeshRoutine() {
        for (int i = 0; i < copy.transform.childCount; i++){
            Destroy(copy.transform.GetChild(i).gameObject);
        }

        yield return new WaitForEndOfFrame();


        GenerateMesh();
    }
}
