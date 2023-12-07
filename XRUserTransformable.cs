using System;
using System.Collections;
using System.Collections.Generic;
using Autohand;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using UnityEngine.Animations;
using TelegramSystem;
using Player = Photon.Realtime.Player;

public class XRUserTransformable : MonoBehaviour
{
    public Transform pivot;

    Rigidbody body;
    public Rigidbody GetBody => body;
    Grabbable grab;
    public Grabbable GetGrabbable => grab;
    PhotonView pView;
    public PhotonView GetPhotonView => pView;
    bool isHighlighted;
    public bool IsHighlighted => isHighlighted;

    public UnityEvent onPossessed, onFreed, onHighlight, onStopHighlight;

    bool leftHandhighlight, rightHandhighlight, leftHandPossess, rightHandPossess;

    GrabbableNetworkController grabNetCtrl;
    public GrabbableNetworkController GetGrabbableNetworkController => grabNetCtrl;

    Coroutine attemptOwnershipRoutine;
    Coroutine leftRoutine, rightRoutine;

    Player currentHolder;

    HashSet<Player> badHolders = new HashSet<Player>();
    float badHolderTimer;

    [Tooltip("Value is percentage of current scale local value")]
    [SerializeField]
    public Vector2 userScaleRange = new Vector2(.25f,5f);

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        grab = GetComponent<Grabbable>();
        pView = GetComponent<PhotonView>();
        grabNetCtrl = GetComponent<GrabbableNetworkController>();

        userScaleRange = userScaleRange * transform.localScale;

        if (pivot == null)
            pivot = transform;
    }

    void Start()
    {
        Telegram.Register("algebraHighlighted",pView,new Action(NetworkHighlighted));
        Telegram.Register("algebraUnhighlighted",pView,new Action(NetworkUnhighlighted));
        Telegram.Register("algebraPossessed",pView,new Action(NetworkPossessed));
        Telegram.Register("algebraFreed",pView,new Action(NetworkFreed));
        Telegram.Register("algebraAddHolder", pView, new Action<Player>(AddHolderNetworked));
        Telegram.Register("algebraRemoveHolder", pView, new Action<Player>(RemoveHolderNetworked));
        Telegram.Register("algebraForceFree", pView, new Action(ForceFree));


        
        
    }

    /*
        void OnEnable()
        {
            grabNetCtrl.OnOwnershipTransfer += OnOwnerChanged;
        }

        private void OnOwnerChanged()
        {
            if(grabNetCtrl.IsHeldNetworked && grabNetCtrl.)  
        }
    */

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient || badHolders.Count == 0) return;
        if(Time.time - badHolderTimer > 5f)
        {
            badHolderTimer = Time.time;
            foreach(Player p in badHolders)
                pView.SendTelegram("algebraForceFree", p);
        }
    }

    public void Possess(bool isLeft) {
        
        if(grabNetCtrl.IsHeldNetworked) {
            //may already be held by this player's other hand
            //return if not held by me

            if(!pView.IsMine) return; //object is already possessed by another player, stop
        }

        if (isLeft) { 
            if(leftHandPossess) return;
            leftHandPossess = true;
            //if (!grabNetCtrl.IsHeldNetworked)
            //{
                leftRoutine=StartCoroutine(PossessRoutine(isLeft));
            //}
        }
        else
        {
            if(rightHandPossess) return;
            rightHandPossess = true;
            //if (!grabNetCtrl.IsHeldNetworked)
            //{
                rightRoutine=StartCoroutine(PossessRoutine(isLeft));
            //}
        }
    }

    float _maxAttemptOwnershipTime = .25f;
    float _currentAttemptOwnershipTime = 0f;
    IEnumerator PossessRoutine(bool isLeft) {

        if(!pView.IsMine) {
            if(!_attemptingOwnership)
                AttemptOwnership();

            _currentAttemptOwnershipTime=0f;

            while(!pView.IsMine) {
                _currentAttemptOwnershipTime += Time.deltaTime;
                if(_currentAttemptOwnershipTime>=_maxAttemptOwnershipTime) {
                    Debug.LogError("Ownership could not be achieved over photon view with id: " + pView.ViewID + " in under " + _maxAttemptOwnershipTime + " seconds");
                    StopAttemptingOwnership();
                    yield break;
                }
                yield return null;
            }
        }

        pView.SendTelegram("algebraPossessed",RpcTarget.Others);
        pView.SendTelegram("algebraAddHolder", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);

        grabNetCtrl.ControlObjectXRTool(grab);


        onPossessed?.Invoke();
            
        StopHighlight(isLeft);
        UserObjectSpawner.instance.possessedHighlight.Add(gameObject);
    }

    void AddHolderNetworked(Player player)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (currentHolder == null)
        {
            Debug.Log($"Current holder for {gameObject.name} set to {player}");
            currentHolder = player;
        }
        else if (currentHolder != player)
        {
            Debug.Log($"Added bad holder for {gameObject.name} the {player}");
            badHolders.Add(player);
            pView.SendTelegram("algebraForceFree", player);
        }
    }
    void RemoveHolderNetworked(Player player)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (currentHolder == player)
        {
            Debug.Log($"Let go of {gameObject.name} and cleared bad holders");
            currentHolder = null;
            badHolders.Clear();
        }
        else if (badHolders.Contains(player))
        {
            Debug.Log($"Removed bad holder for {gameObject.name} the {player}");
            badHolders.Remove(player);
        }
    }

    void ForceFree()
    {
        grabNetCtrl.StopAllCoroutines();
        StopAllCoroutines();
        _attemptingOwnership = false;
        Free(false);
        Free(true);
    }

    public void Free(bool isLeft)
    {
        if (!grabNetCtrl.IsHeldNetworked) return;

        bool noHandPossessing = false;

        if (isLeft)
        {
            if (!leftHandPossess) return;

            leftHandPossess = false;
            if (!rightHandPossess)
                noHandPossessing = true;          
        }
        else
        {
            if (!rightHandPossess) return;
            rightHandPossess = false;
            if (!leftHandPossess)
                noHandPossessing = true;
        }

        if (noHandPossessing) {
            pView.SendTelegram("algebraFreed",RpcTarget.Others);
            pView.SendTelegram("algebraRemoveHolder", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
            //Autohand.Hand hand = isLeft ? AutoHandPlayer._Instance.handLeft : AutoHandPlayer._Instance.handRight;
            //isPossessed = false;

            //won't release object over network if held count > 0
            grabNetCtrl.ReleaseObjectXRTool(grab);
            UserObjectSpawner.instance.possessedHighlight.Remove(gameObject); 
            Highlight(isLeft);
           
            onFreed?.Invoke();
        }
    }

    public void Highlight(bool isLeft) {

        if (isLeft) { 
            if(leftHandhighlight) return;
            leftHandhighlight = true; 
        }
        else
        {
            if(rightHandhighlight) return;
            rightHandhighlight = true; 
        }

        if (!grabNetCtrl.IsHeldNetworked)
        {
            pView.SendTelegram("algebraHighlighted",RpcTarget.Others);
            onHighlight?.Invoke();
            UserObjectSpawner.instance.hoverHighlight.Add(gameObject);
            isHighlighted = true;
        }
    }

    public void StopHighlight(bool isLeft) {
        
        bool noHandHighlighting = false;

        if (isLeft) { 
            if(!leftHandhighlight) return;
            leftHandhighlight = false; 
            if(!rightHandhighlight) 
                noHandHighlighting = true;
        }
        else
        {
            if(!rightHandhighlight) return;
            rightHandhighlight = false; 
            if(!leftHandhighlight)
                noHandHighlighting = true;
        }

        if (noHandHighlighting || grabNetCtrl.IsHeldNetworked)
        {
            pView.SendTelegram("algebraUnhighlighted",RpcTarget.Others);
            onStopHighlight?.Invoke();
            UserObjectSpawner.instance.hoverHighlight.Remove(gameObject);
            isHighlighted = false;
        }
    }

    public void AttemptOwnership() {
        attemptOwnershipRoutine=StartCoroutine(AttemptOwnershipRoutine());
    }

    bool _attemptingOwnership;
    private IEnumerator AttemptOwnershipRoutine()
    {
        _attemptingOwnership = true;
        while (!pView.IsMine)
        {
            pView.RequestOwnership();
            yield return new WaitForSeconds(0.05f);
        }

        _attemptingOwnership = false;
        if (pView.OwnershipTransfer != OwnershipOption.Takeover)
            Debug.LogError($"May not work properly on {gameObject.name} because its Ownership Transfer is not set to 'Takeover'");
    }

    void StopAttemptingOwnership() {
        StopCoroutine(attemptOwnershipRoutine);
        _attemptingOwnership = false;
    }

    void NetworkHighlighted() {
        UserObjectSpawner.instance.hoverHighlight.Add(gameObject);
    }

    void NetworkUnhighlighted() {
        UserObjectSpawner.instance.hoverHighlight.Remove(gameObject);
    }

    void NetworkPossessed() {
        UserObjectSpawner.instance.hoverHighlight.Remove(gameObject);
        UserObjectSpawner.instance.possessedHighlight.Add(gameObject);

        Free(false);
        Free(true);
    }

    void NetworkFreed() {
        UserObjectSpawner.instance.possessedHighlight.Remove(gameObject);
        UserObjectSpawner.instance.hoverHighlight.Add(gameObject);
    }

    void OnDisable()
    {
        UserObjectSpawner.instance.possessedHighlight.Remove(gameObject);
        UserObjectSpawner.instance.hoverHighlight.Remove(gameObject);
        UserObjectSpawner.instance.trashHighlight.Clear();
    }
}

