using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(Animator))]
public class AIZombie : MonoBehaviour
{

	private UnityEngine.AI.NavMeshAgent navmesh;
	private CharacterMovement characterMove { get { return GetComponent<CharacterMovement> (); } set { characterMove = value; } }
	private Animator animator { get { return GetComponent<Animator> (); } set { animator = value; } }
	private CharacterStats characterStats { get { return GetComponent<CharacterStats> (); } set { characterStats = value; } }
    private WeaponHandler weaponHandler { get { return GetComponent<WeaponHandler>(); } set { weaponHandler = value; } }
    

	public enum AIState { Patrol, Attack, Chasing }
	public AIState aiState;

    public Transform righthandBoneIK;
    [HideInInspector]
    public Action isEnemyLookAround;


    [System.Serializable]
	public class PatrolSettings
	{
		public WaypointBase[] waypoints;
        public bool isStandingPatrol = false;
	}
	public PatrolSettings patrolSettings;

    [System.Serializable]
    public class ChasingSetting
    {
        public float waiTimeToAttack = 1f;
        public float waiTimeToLookAround = 1f;
    }
    public ChasingSetting chasingSetting;

    [System.Serializable]
	public class SightSettings
	{
		public LayerMask sightLayers;
		public float sightRange = 20f;
        public float attackRange = 1;

		public float fieldOfView = 120f;
		public float eyeheight = 0.7f;

	}
	public SightSettings sight;

    [System.Serializable]
    public class AttackSettings
    {
        public Transform NeckTranform;
        public GameObject Blood;
        public Transform BloodBittingTransform;


    }
    public AttackSettings attack;

    private float currentWaitTime;
	private int waypointIndex;
	private Transform currentLookTransform;
	private bool walkingToDest;
    private bool setDestination;
    private bool reachedDestination;

	private float forward;

	private Transform target;
	private Vector3 targetLastKnownPosition;
	private CharacterStats[] allCharacters;
    bool isLookAround;
    bool isAttacking;
    bool isBittingAtNeckTarget;
	// Use this for initialization
	void Start () {
        //navmesh = GetComponentInChildren<UnityEngine.AI.NavMeshAgent> ();
        
        navmesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //navmesh.updatePosition = false;
        isEnemyLookAround += EnemyLookAround;

        if (navmesh == null) {
			Debug.LogError ("We need a navmesh to traverse the world with.");
			enabled = false;
            
			return;
		}

		//if (navmesh.transform == this.transform) {
		//	Debug.LogError ("The navmesh agent should be a child of the character: " + gameObject.name);
		//	enabled = false;
		//	return;
		//}

		navmesh.speed = 0;
		navmesh.acceleration = 0;
		navmesh.autoBraking = false;

		if (navmesh.stoppingDistance == 0) {
			Debug.Log ("Auto settings stopping distance to 1.3f");
			navmesh.stoppingDistance = 1.3f;
		}

		GetAllCharacters ();
	}

    //private void EnemyLookAround(bool isLookAround)
    //{
    //    if (target == null)
    //    {
    //        animator.SetBool("LookAround", isLookAround);
    //        StartCoroutine("ResetLookAround");
    //    }
       
    //}
    //IEnumerator ResetLookAround()
    //{
    //    yield return new WaitForSeconds(6);
    //    animator.SetBool("LookAround", false);

    //}
    void GetAllCharacters () {

		allCharacters = GameObject.FindObjectsOfType<CharacterStats>();
	}
	
	// Update is called once per frame
	void Update () {
        //allCharacters = GameObject.FindObjectsOfType<CharacterStats>();

        //TODO: Animate the strafe when the enemy is trying to shoot us.
        //characterMove.Animate (forward, 0);
        animator.SetFloat("Speed", forward);
        //animator.SetBool("LookAround", isLookAround);
        //animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsBitting", isAttacking);

        //navmesh.transform.position = transform.position;

        LookForTarget();
        //weaponHandler.Aim(aiming);


        switch (aiState)
        {
		    case AIState.Patrol:
			    Patrol ();
			    break;
            case AIState.Chasing:
                Chasing();
                break;
            case AIState.Attack:
                 Attacking();
                 break;
		}
	}

	void LookForTarget ()
    {
		if (allCharacters.Length > 0)
        {
			foreach (CharacterStats c in allCharacters)
            {
				if (c != characterStats && c.gameObject.tag=="Grim" && c.faction != characterStats.faction && c == ClosestEnemy())
                {
                    RaycastHit hit;
                    //Vector3 start = transform.position + (transform.up * sight.eyeheight);
                    //Vector3 dir = (c.transform.position + c.transform.up * c.transform.GetComponent<CharacterMovement>().heightCharacter / 2) - start;
                    Vector3 start = transform.position;
                    Vector3 dir = c.transform.position - start;
                    float sightAngle = Vector3.Angle(dir, transform.forward);
                    //Debug.Log("angle Dir vs tranform.forward" + sightAngle);
                    //Debug.DrawRay(start, dir, Color.red, sight.sightRange);
                    if (Physics.Raycast(start, dir, out hit, sight.sightRange, sight.sightLayers))
                    {

                        if (hit.transform.tag != "Grim")
                        {
                            
                            if (target != null)
                            {
                                targetLastKnownPosition = target.position;
                                target = null;
                            }
                            continue;
                        }
                        else if (sightAngle < sight.fieldOfView && hit.collider.GetComponent<CharacterStats>())
                        {

                            target = hit.transform;
                            targetLastKnownPosition = Vector3.zero;
                        }
                        else
                        {
                            if (target != null)
                            {
                                targetLastKnownPosition = target.position;
                                target = null;
                            }

                        }

                    }
                    else
                    {
                        if (target != null)
                        {
                            targetLastKnownPosition = target.position;
                            target = null;
                        }
                    }
                }
			}
		}
	}

	CharacterStats ClosestEnemy ()
    {
		CharacterStats closestCharacter = null;
		float minDistance = Mathf.Infinity;
		foreach (CharacterStats c in allCharacters)
        {
			if (c != characterStats && c.faction != characterStats.faction)
            {
				float distToCharacter = Vector3.Distance (c.transform.position, transform.position);
				if (distToCharacter < minDistance)
                {
					closestCharacter = c;
					minDistance = distToCharacter;
				}
			}
		}

		return closestCharacter;
	}
    void PatrolBehaviour()
    {
        //aiming = false;
        isAttacking = false;
        isLookAround = false;

    }
    void Patrol ()
    {
        PatrolBehaviour();

        if (target == null && targetLastKnownPosition == Vector3.zero)
        {
            aiState = AIState.Patrol;
            if (patrolSettings.isStandingPatrol == false)
            {
                if (!navmesh.isOnNavMesh)
                {
                    Debug.Log("We're off the navmesh");
                    return;
                }

                if (patrolSettings.waypoints.Length == 0)
                {
                    return;
                }

                navmesh.SetDestination(patrolSettings.waypoints[waypointIndex].destination.position);
                LookAtPosition(navmesh.steeringTarget);
                if (navmesh.remainingDistance <= navmesh.stoppingDistance)
                {
                    walkingToDest = false;
                    forward = LerpSpeed(forward, 0, 1);
                    currentWaitTime -= Time.deltaTime;

                    if (patrolSettings.waypoints[waypointIndex].lookAtTarget != null)
                        currentLookTransform = patrolSettings.waypoints[waypointIndex].lookAtTarget;
                    if (currentWaitTime <= 0)
                    {
                        waypointIndex = (waypointIndex + 1) % patrolSettings.waypoints.Length;
                    }

                }
                else
                {
                    walkingToDest = true;
                    forward = LerpSpeed(forward, 0.5f, 1);

                    currentWaitTime = patrolSettings.waypoints[waypointIndex].waitTime;
                    currentLookTransform = null;
                }
            }         
        }
        else if (target == null && targetLastKnownPosition != Vector3.zero)
        {
            var distance = Vector3.Distance(transform.position, targetLastKnownPosition);
            if (distance > sight.attackRange)
            {
                aiState = AIState.Chasing;

            }
        }
        else if (target != null && targetLastKnownPosition == Vector3.zero)
        {
            var distance = Vector3.Distance(transform.position, target.position);
            if (distance > sight.attackRange && distance < sight.sightRange)
            {
                aiState = AIState.Chasing;

            }
            else if (distance < sight.attackRange)
            {
                aiState = AIState.Attack;
            }

        }
        else
        {
            aiState = AIState.Attack;
        }
	}
    void ChasingBehaviour()
    {
        //attack.isCanShoot = false;
        isAttacking = false;

        isLookAround = false;
    }
    void Chasing()
    {
        ChasingBehaviour();

        if (target != null && targetLastKnownPosition == Vector3.zero)
        {
            navmesh.SetDestination(target.position);
            LookAtPosition(navmesh.steeringTarget);

            var distance = Vector3.Distance(transform.position, target.position);
            if (distance <= sight.attackRange-0.1f)
            {
                forward = LerpSpeed(forward, 0f, 1);
                currentWaitTime -= Time.deltaTime;

                if (currentWaitTime <= 0)
                {
                    aiState = AIState.Attack;

                }

            }
            else if (distance > sight.attackRange + 0.1f)
            {
                forward = LerpSpeed(forward, 1f, 1);
                currentWaitTime = chasingSetting.waiTimeToAttack;

            }

        }
        else if (target == null && targetLastKnownPosition != Vector3.zero)
        {
            navmesh.SetDestination(targetLastKnownPosition);
            LookAtPosition(navmesh.steeringTarget);
            var distance = Vector3.Distance(transform.position, targetLastKnownPosition);
            if (navmesh.remainingDistance <= navmesh.stoppingDistance)
            {
                forward = LerpSpeed(forward, 0, 1);

                currentWaitTime -= Time.deltaTime;
                if (currentWaitTime <= 0)
                {
                    // aistate change to patrol in enemyLookAround();
                    EnemyLookAround();
                }


            }
            else
            {
                forward = LerpSpeed(forward, 1f, 1);

                currentWaitTime = chasingSetting.waiTimeToLookAround;
            }

        }
        if (target == null && targetLastKnownPosition == Vector3.zero)
        {
            aiState = AIState.Patrol;
        }
    }
    private void EnemyLookAround()
    {
        if (target == null)
        {
            isLookAround = true;
            StartCoroutine(ResetLookAround());
        }

    }
    IEnumerator ResetLookAround()
    {
        yield return new WaitForSeconds(4);
        isLookAround = false;
        aiState = AIState.Patrol;

    }
    void Attacking()
    {
        if (target!= null)
        {
            var distance = Vector3.Distance(transform.position, target.position);

            if (distance > sight.attackRange)
            {
                aiState = AIState.Chasing;

            }
            else
            {
                AttackBehaviour();
                LookAtPosition(target.position);
                Vector3 start = transform.position + transform.up * sight.eyeheight;

                Vector3 dir = target.position - transform.position;
            }
        }
        else
        {
            aiState = AIState.Patrol;
        }
    }
    void AttackBehaviour()
    {
        walkingToDest = false;
        setDestination = false;
        reachedDestination = false;
        currentLookTransform = null;
        forward = LerpSpeed(forward, 0, 15);
        isLookAround = false;
        isAttacking = true;

    }
    public void SetTargetIsBitted()
    {
        isBittingAtNeckTarget = true;
        var distance = Vector3.Distance(transform.position, target.position);

        if (distance <= sight.attackRange)
        {
            target.GetComponent<CharacterStats>().SetIsBitted();

        }
    }
    public void SetTargetIsNotBitted()
    {
        isBittingAtNeckTarget = false;

        target.GetComponent<CharacterStats>().SetIsNotBitted();
    }
   
    public void EffectBloodBitting()
    {
        var distance = Vector3.Distance(transform.position, target.position);

        if (distance <= sight.attackRange)
        {
            target.GetComponent<CharacterStats>().Damage(5.0f);
            var obj = Instantiate(attack.Blood, attack.BloodBittingTransform.position, attack.BloodBittingTransform.rotation);
            Destroy(obj, 1.5f);
        }
      
    }
    float LerpSpeed (float curSpeed, float destSpeed, float time)
    {
		curSpeed = Mathf.Lerp (curSpeed, destSpeed, Time.deltaTime * time);
		return curSpeed;
	}

	void LookAtPosition (Vector3 pos)
    {
		Vector3 dir = pos - transform.position;
		Quaternion lookRot = Quaternion.LookRotation (dir);
		lookRot.x = 0;
		lookRot.z = 0;
		transform.rotation = Quaternion.Lerp (transform.rotation, lookRot, Time.deltaTime * 5);
	}

    private void LateUpdate()
    {
        if (target != null)
        {
            if (isBittingAtNeckTarget)
            {
                //attack.NeckTranform.position = TargetTranBitting.position;
                //attack.NeckTranform.rotation = TargetTranBitting.rotation;
            }
        }
        
    }
    void OnAnimatorIK ()
    {
		if (currentLookTransform != null && !walkingToDest)
        {
			animator.SetLookAtPosition (currentLookTransform.position);
			animator.SetLookAtWeight (1, 0, 0.5f, 0.7f);
		}
        if (isBittingAtNeckTarget==true)
        {
            

        }
       
    }
    
    
    private void OnDrawGizmos()
    {
        var center = transform.position;
        center.y += 0.01f;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(center, sight.sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, sight.attackRange);
        //Gizmos.color = Color.black;
        //Gizmos.DrawWireSphere(center, meleeRange);
    }
}


