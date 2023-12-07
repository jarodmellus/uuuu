using Autohand;
using UnityEngine;
using UnityEngine.Events;

public class ExperimentalGrabbableDial : MonoBehaviour
{
    [Header("Snap")]
    [Tooltip("Value will snap to value of nearest angle")]
    [Range(0, 180)] public int snapAngleAmount = 25;
    [SerializeField] bool invertValue;

    [Header("Min/Max")]
    [Range(-720, 720)] public float MinimumAngle = -360;
    [Range(-720, 720)] public float MaximumAngle = 360;

    [Header("Haptic Feedback")]
    [Range(0, 1)][SerializeField] float HapticForce = 0.25f;
    [Range(0, 1)][SerializeField] float HapticDuration = 0.25f;

    [Space]
    public UnityEvent<float> AngleChangedEvent;
    public UnityEvent<float> AngleValueEvent;

    public float representedMinValue,representedMaxValue;
    float _currentRepresentedValue;

    private Grabbable _grab;
    protected float _currentDialAngle;
    private float _currentSnappedAngle, _currentValue;
    private int _revolutions;
    protected bool _hasStartAngle = false;
    protected Vector3 _startAngles, _startPos;
    private Autohand.Hand _currentHand;
    Rigidbody rigidbody;
    HingeJoint hingeJoint;

    [SerializeField]
    protected bool mustBeGrabbedToUpdate=true;

    bool isLocked;
    public UnityEvent onLock, onUnlock;

    public void SetLocked(bool val) {
        if (val == isLocked) return;

        isLocked = val;

        if(isLocked) {
            rigidbody.isKinematic = true;
            onLock?.Invoke();
        }
        else {
            rigidbody.isKinematic = false;
            onUnlock?.Invoke();
        }
    }

    public void ToggleLocked() {
        SetLocked(!isLocked);
    }

    private void Awake() {
        hingeJoint = GetComponent<HingeJoint>();
        rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void OnEnable()
    {
        if(!_grab) _grab = GetComponent<Grabbable>();
        if (!_grab) this.enabled = false;

        if (_grab)
        {
            _grab.onGrab.AddListener(Grabbed);
            _grab.onRelease.AddListener(Released);
        }
    }

    protected virtual void SetStartAngle()
    {
        _startPos = transform.localPosition;
        _startAngles = transform.localEulerAngles;
        _currentDialAngle = transform.localEulerAngles.y;
        CalculateValue();
        _hasStartAngle = true;
    }

    private void OnDisable()
    {
        if (_grab)
        {
            _grab.onGrab.RemoveListener(Grabbed);
            _grab.onRelease.RemoveListener(Released);
        }
    }

    public float DialTransformAngle() => _currentDialAngle;
    public float GetCurrentAngle() => _currentSnappedAngle;
    public float GetNormalizedValueUnclamped() => _currentValue;
    public float GetNormalizedValue() => Mathf.Clamp(_currentValue, 0, 1);


    protected float CalculateValue()
    {
        var value = (CalculateSnappedAngle() - MinimumAngle) / (MaximumAngle - MinimumAngle);
        value = invertValue ? -value : value;
        
        if (_currentValue != value)
        {
            _currentValue = value;
            _currentRepresentedValue = representedMinValue + (GetNormalizedValue() * (representedMaxValue-representedMinValue));
            AngleChangedEvent?.Invoke(CalculateSnappedAngle());
            AngleValueEvent?.Invoke((representedMinValue + (value * (representedMaxValue-representedMinValue))));
            if (_currentHand) _currentHand.PlayHapticVibration(HapticDuration, HapticForce);
        }
        else _currentValue = value;
        return value;
    }
    private float CalculateDialAngle()
    {
        var newAngle = transform.localEulerAngles.y;
        var angleDifference = Mathf.Clamp(Mathf.Abs(newAngle - _currentDialAngle),0f,snapAngleAmount);
        var newRevolutions = _revolutions;

        if (angleDifference > 270f)
        {
            if (_currentDialAngle < newAngle)
                newRevolutions--;
            else if (newAngle < _currentDialAngle)
                newRevolutions++;
        }

        var toReturn = (newAngle + 360f * newRevolutions) - _startAngles.y;
        if (toReturn < MinimumAngle || toReturn > MaximumAngle)
        {
            transform.localEulerAngles = new Vector3(_startAngles.x, _currentDialAngle, _startAngles.z);
        }
        else
        {
            _currentDialAngle = newAngle;
            _revolutions = newRevolutions;
        }

        return (_currentDialAngle + 360f * _revolutions) - _startAngles.y;
    }
    private float CalculateSnappedAngle()
    {
        //_currentSnappedAngle = (snapAngleAmount > 0) ? Mathf.Round(CalculateDialAngle() / snapAngleAmount) * snapAngleAmount : CalculateDialAngle();
        _currentSnappedAngle = Mathf.Round(CalculateDialAngle() / (float)snapAngleAmount) * snapAngleAmount;
        return _currentSnappedAngle;
    }

    private void LateUpdate()
    {
        if (isLocked) return;

        if (!_hasStartAngle)
        {
            SetStartAngle();
        }

        if (IsGrabbed() || !mustBeGrabbedToUpdate)
        {
            CalculateValue();
            transform.transform.localPosition = _startPos;
        }
        else
        {
            if (transform.localPosition != _startPos)
                transform.localPosition = _startPos;
        }
        Quaternion nrot = Quaternion.Euler(_startAngles.x,_startAngles.y + _currentSnappedAngle, _startAngles.z);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, nrot,.5f);
    }

    public virtual bool IsGrabbed() => (mustBeGrabbedToUpdate) ? ((_grab) ? _grab.IsHeld() : false) : true;

    private void Grabbed(Autohand.Hand h, Grabbable g)
    {
        if (isLocked) return;

       _currentHand = h;
        g.body.isKinematic = false;
    }
    private void Released(Autohand.Hand h, Grabbable g)
    {
        _currentHand = null;
        g.body.isKinematic = true;
    }
    
    public float GetRepresentedValue() {
        return _currentRepresentedValue;
    }
}