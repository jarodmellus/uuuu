using System;
using System.Collections.Generic;
using CW.Common;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using Autohand;

public class AnimalMovement : MonoBehaviour {

	Animal animal;
	[SerializeField] Animator animator;
	[SerializeField] Transform destination;
	[SerializeField] Transform noTarget;
	[SerializeField] CwFollow destinationFollow;
	[SerializeField] float maxSpeed=2f;
	[SerializeField] float minimumDistanceForMaxSpeed=5f;
	[SerializeField] float walkSpeedMultiplierOffset = 1f;
	[SerializeField] float weightMultipler = 1.5f;

	[Range(0f,1f)][SerializeField] float brakeLerp = .85f;
	

	public bool moving = false;
	public bool following = false;
	public bool sitting = false;
	public bool sleeping = false;
	public bool laying = false;


	[SerializeField]
	CwFollow headTarget;
	[SerializeField]
	LookAtIK lookAtIK;
	public void SetMoving(bool val) { 
		moving = val;
		if(moving) {
			animal.GetGrabbable.enabled=false;
			animal.GetRigidbody.isKinematic=true;
		} else {
			animal.GetGrabbable.enabled=true;
			animal.GetRigidbody.isKinematic=false;
		}
	}

	public void SetFollowing(bool val) => following = val;
	public void SetSitting(bool val) {
		sitting = val;
		animator.SetBool("sitting",sitting);
	}
	public void SetSleeping(bool val) {
		sleeping = val;
		animator.SetBool("sleeping",sleeping);
	}
	public void SetLaying(bool val) {
		laying = val;
		animator.SetBool("laying",laying);
	}

	NavMeshAgent agent;

	void Awake()
	{
		animal=GetComponent<Animal>();
		agent = GetComponent<NavMeshAgent>();
		destination.SetParent(null);
		headTarget.transform.SetParent(null);

	}

	private void Start() {
		SetMoving(moving);
		SetFollowing(following);
		SetSitting(sitting);
		SetSleeping(sleeping);
		SetLaying(laying);
	}

	void OnDestroy()
    {
		if(destination.gameObject!=null)
        	Destroy(destination.gameObject);
    }

	void FixedUpdate() {
		if (moving)
		{
			if (following)
			{
				FollowTarget();
			}
		}
	}

	public void FollowTarget() {
		if (destination == null) return;
		agent.destination = destination.position;
		float dist = Vector3.Distance(new Vector3(transform.position.x,0,transform.position.z), new Vector3(destination.position.x,0,destination.position.z));

		if (dist <= agent.stoppingDistance)
		{
			agent.speed = Mathf.Lerp(agent.speed,0,brakeLerp);
		}
		else
		{
			agent.speed = Mathf.Lerp(agent.speed,Mathf.Clamp(maxSpeed * (dist / minimumDistanceForMaxSpeed), 0f, maxSpeed),.5f);
		}

		animator.SetFloat("speed",(agent.speed/maxSpeed) + walkSpeedMultiplierOffset);
		animator.SetLayerWeight(1,(agent.speed/maxSpeed) * weightMultipler);
	}

	public void SetDestination(Transform newDestination) {
		
		destinationFollow.Target = newDestination;
		//destinationFollow.enabled = true;
		//destinationFollow.enabled = true;
		headTarget.Target = destination;
	}

	public void SetNoDestination() {
		
		//destinationFollow.enabled = false;
		
		//destinationFollow.enabled = false;
		headTarget.Target = noTarget;
		//destinationFollow.Target = null;
	}

	public void LookAtPlayer() {
		destinationFollow.Target = AutoHandPlayer.Instance.headCamera.transform;
		headTarget.Target = destination;
	}
}