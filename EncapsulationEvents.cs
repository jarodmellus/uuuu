using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class EncapsulationEvents : MonoBehaviour {
    public Collider[] innerColliders, outerColliders;
    Bounds innerBounds, outerBounds;
    public UnityEvent onFullyEncapsulated, onFullyUnencapsulated;
    [SerializeField]
    bool checkVolume=false;
    [Header("Volume")]
    [ShowIf("checkVolume")]
    public UnityEvent<float> onEncapsulatedVolumeChange;
    [SerializeField]
    [ShowIf("checkVolume")]
    //bool useMinimumVolumeEvents=false;
    //[ShowIf("useMinimumVolumeEvents")]
    public UnityEvent onOverMinimumVolume, onUnderMinimumVolume;
    //[ShowIf("useMinimumVolumeEvents")]
    [SerializeField]
    [ShowIf("checkVolume")]
    float minimumVolume=.75f;

    [SerializeField] float skipTime = .1f;
    float _currentTime = 0f;
    bool _isFullyEncapsulated = false;
    bool _isOverMinimumVolume = false;

    [SerializeField]
    [DisableIf("True")]
    float _currentVolume;

    [SerializeField] private bool showGizmos;

    void Start() {
        innerBounds = EncapsulationCheck.CombineColliderBounds(innerColliders);
        outerBounds = EncapsulationCheck.CombineColliderBounds(outerColliders);
    }


    void FixedUpdate() {
        if(_currentTime < skipTime) {
            _currentTime+=Time.fixedDeltaTime;
            return;
        }
        else {
            _currentTime = 0f;
        }

        if(checkVolume) {
            float vol = EncapsulationCheck.GetNormalizedEncapsulationVolume(innerColliders, outerColliders);
            _currentVolume = vol;
            onEncapsulatedVolumeChange?.Invoke(vol);

            if(_isOverMinimumVolume) {
                if(vol < minimumVolume) {
                    onUnderMinimumVolume?.Invoke();
                    _isOverMinimumVolume=false;
                }
            }
            else {
                if(vol >= minimumVolume) {
                    onOverMinimumVolume?.Invoke();  
                    _isOverMinimumVolume=true;  
                }
            }
        }

        bool isEncapsulated = EncapsulationCheck.AreBoundEncapsulatedByOtherBounds(innerColliders,outerColliders);
        if(_isFullyEncapsulated) {
            if(!isEncapsulated) {
                onFullyEncapsulated?.Invoke();
                _isFullyEncapsulated=false;
            }
            else {
                onFullyUnencapsulated?.Invoke();
                _isFullyEncapsulated=true;
            }
        }
    }
    
    void OnDrawGizmos() {
        if (!showGizmos) return;

        if (innerColliders.Length > 0)
        {
            Gizmos.color = new Color(0f,1f,0f,.5f);
            Gizmos.DrawCube(EncapsulationCheck.CombineColliderBounds(innerColliders).center, EncapsulationCheck.CombineColliderBounds(innerColliders).size);
        }

        if (outerColliders.Length > 0)
        {
            Gizmos.color = new Color(0f,0f,1f,.5f);
            Gizmos.DrawCube(EncapsulationCheck.CombineColliderBounds(outerColliders).center, EncapsulationCheck.CombineColliderBounds(outerColliders).size);
        }
    }
}