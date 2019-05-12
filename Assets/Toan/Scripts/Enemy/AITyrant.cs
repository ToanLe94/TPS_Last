using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(Animator))]
public class AITyrant : MonoBehaviour
{

	private UnityEngine.AI.NavMeshAgent navmesh;
	private CharacterMovement characterMove { get { return GetComponent<CharacterMovement> (); } set { characterMove = value; } }
	private Animator animator { get { return GetComponent<Animator> (); } set { animator = value; } }
	private CharacterStats characterStats { get { return GetComponent<CharacterStats> (); } set { characterStats = value; } }
    private WeaponHandler weaponHandler { get { return GetComponent<WeaponHandler>(); } set { weaponHandler = value; } }
    

	public enum AIState { Patrol, Attack, Chasing }
	public AIState aiState;

    public enum AIStateAttack { Idle, Turn, WalkAndAttack,RunAndAttack, JumpAttack ,None =5 }
    public AIStateAttack aiStateAttack;
    public Transform righthandBoneIK;
    [HideInInspector]
    public Action isEnemyLookAround;

    [System.Serializable]
    public class SightSettings
    {
        public LayerMask sightLayers;
        public float sightRange = 20f;
        public float attackRange = 8.4f;

        public float WalkingComboRange;
        public float RunningAttackRange;
        public float StandingComboRange;
        public float JumpAttackRange;

        public float fieldOfView = 120f;
        public float eyeheight = 0.7f;

    }
    public SightSettings sight;
    [System.Serializable]
	public class PatrolSettings
	{
		public WaypointBase[] waypoints;
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
    public class AttackSettings
    {
        public GameObject Blood;
        public float waiTimeIdle = 4f;
        public Transform head_Transform;
        public Transform currentLookPosition;
        public Transform currentLookMaxRightPos;
        public Transform currentLookMaxLeftPos;
        public Quaternion lastRotationHead;
        public GameObject effectAttack;
        public CollidePlayer collidePlayer;

        [HideInInspector]
        public Vector3 FocusToPointAttack;
    }
    public AttackSettings attack;

    private float currentWaitTime;
	private int waypointIndex;
	private Transform currentLookTransform;
     

	private float forward;

	private Transform target;
	private Vector3 targetLastKnownPosition;
	private CharacterStats[] allCharacters;
    //bool isLookAround;
    //bool isAttacking;
    //bool isBittingAtNeckTarget;
	// Use this for initialization
	void Start () {
        //navmesh = GetComponentInChildren<UnityEngine.AI.NavMeshAgent> ();
        
        navmesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navmesh.updatePosition = false;

        //isEnemyLookAround += EnemyLookAround;

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
        navmesh.nextPosition = transform.position;

        //TODO: Animate the strafe when the enemy is trying to shoot us.
        //characterMove.Animate (forward, 0);
        animator.SetFloat("Speed", forward);
        //animator.SetBool("LookAround", isLookAround);
        //animator.SetBool("IsAttacking", isAttacking);

        LookForTarget();
        switch (aiState)
        {
		    case AIState.Patrol:
			    Patrol ();
			    break;
            case AIState.Chasing:
                Chasing();
                break;
            case AIState.Attack:
                 //Attacking();
                switch (aiStateAttack)
                {
                    case AIStateAttack.Idle:
                        Attacking_IdleState();
                        break;
                    case AIStateAttack.Turn:
                        break;
                    case AIStateAttack.WalkAndAttack:
                        Attacking_WalkAndAttackState();
                        break;
                    case AIStateAttack.RunAndAttack:
                        Attacking_RunAndAttackState();
                        break;
                    case AIStateAttack.JumpAttack:
                        Attacking_JumpAttackState();
                        break;
                    case AIStateAttack.None:
                        //if (forward >0)
                        //{
                        //    if (navmesh.remainingDistance <= navmesh.stoppingDistance)
                        //    {
                        //        //walkingToDest = false;
                        //        forward = LerpSpeed(forward, 0, 3);

                        //        if (forward <= 0.1)
                        //        {
                        //            forward = 0;
                        //            aiStateAttack = AIStateAttack.Idle;
                        //        }

                        //    }
                        //    else
                        //    {
                        //        //walkingToDest = true;
                        //        LookAtPosition(navmesh.steeringTarget);
                        //        forward = LerpSpeed(forward, 1f, 3);
                        //    }
                        //}
                        //else
                        //{
                        //    LookAtPosition(target.position);
                        //}
                        LookAtPosition(target.position);
                        break;
                    default:
                        break;
                }
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
                    Debug.DrawRay(start, dir, Color.red, sight.sightRange);
                    var distance = Vector3.Distance(transform.position, c.transform.position);
                    if (distance < sight.attackRange)
                    {
                        target = c.transform;
                        targetLastKnownPosition = Vector3.zero;
                        aiState = AIState.Attack;

                    }
                    else if (Physics.Raycast(start, dir, out hit, sight.sightRange, sight.sightLayers))
                    {
                       
                        if (hit.transform.tag != "Grim")
                        {

                            if (target != null) // xet them dieu kien vat can co chieu cao qua tam mat zombie 
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
        //isAttacking = false;
        //isLookAround = false;

    }
    void Patrol ()
    {
        PatrolBehaviour();

        if (target == null && targetLastKnownPosition == Vector3.zero)
        {
            aiState = AIState.Patrol;


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
                //walkingToDest = false;
                forward = LerpSpeed(forward, 0, 3);
                currentWaitTime -= Time.deltaTime;

                if (patrolSettings.waypoints[waypointIndex].lookAtTarget != null)
                    currentLookTransform = patrolSettings.waypoints[waypointIndex].lookAtTarget;
                if (currentWaitTime <= 0)
                {
                    forward = 0;
                    waypointIndex = (waypointIndex + 1) % patrolSettings.waypoints.Length;
                }

            }
            else
            {
                //walkingToDest = true;
                forward = LerpSpeed(forward, 0.5f, 3);

                currentWaitTime = patrolSettings.waypoints[waypointIndex].waitTime;
                currentLookTransform = null;
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
        //isAttacking = false;

        //isLookAround = false;
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
                forward = LerpSpeed(forward, 0f, 2);
                currentWaitTime -= Time.deltaTime;

                if (currentWaitTime <= 0)
                {
                    aiState = AIState.Attack;

                }

            }
            else if (distance > sight.attackRange + 0.1f)
            {
                forward = LerpSpeed(forward, 1f, 2);
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
                forward = LerpSpeed(forward, 0, 2);

                currentWaitTime -= Time.deltaTime;
                if (currentWaitTime <= 0)
                {
                    // aistate change to patrol in enemyLookAround();
                    //EnemyLookAround();
                }


            }
            else
            {
                forward = LerpSpeed(forward, 1f, 2);

                currentWaitTime = chasingSetting.waiTimeToLookAround;
            }

        }
        if (target == null && targetLastKnownPosition == Vector3.zero)
        {
            aiState = AIState.Patrol;
        }
    }
    //private void EnemyLookAround()
    //{
    //    if (target == null)
    //    {
    //isLookAround = true;
    //        StartCoroutine(ResetLookAround());
    //    }

    //}
    //IEnumerator ResetLookAround()
    //{
    //    yield return new WaitForSeconds(4);
    //    isLookAround = false;
    //    aiState = AIState.Patrol;

    //}
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
                //LookAtPosition(target.position);
                Vector3 start = transform.position + transform.up * sight.eyeheight;
                Vector3 dir = target.position - transform.position;
            }
        }
        else
        {
            aiState = AIState.Patrol;
        }
    }
    void Attacking_IdleState()
    {
        if (target!= null)
        {
            forward = LerpSpeed(forward, 0, 2);
            if (forward < 0.1)
            {
                forward = 0;
                currentWaitTime -= Time.deltaTime;
                if (currentWaitTime <=0)
                {
                    aiStateAttack = AIStateAttack.Turn;
                    Attacking_TurnState();

                }
            }
            else
            {
                currentWaitTime = attack.waiTimeIdle;
            }
                
        }
    }

    void Attacking_TurnState()
    {
        if (target!= null)
        {
            Vector3 start = transform.position;
            Vector3 dir = target.transform.position - start;
            float signedAngle = Vector3.SignedAngle(dir, transform.forward, Vector3.up);
            //Debug.Log("SignedAngle :" + signedAngle);
            float turn = signedAngle / 180f;

            if (turn >=0.2f || turn <=-0.2f)
            {
                SetRateTurn(turn);
                

            }
            else
            {
                StopTurnAndChangeState();
            }
        }
    }
    public void StopTurnAndChangeState() // call wwhen end animation turn
    {
        Debug.Log("stop turn :" );

        animator.SetFloat("Turn", 0);

        //aiStateAttack = AIStateAttack.RunAndAttack; // test for run attack

        aiStateAttack = (AIStateAttack)UnityEngine.Random.Range(2, 5);
        if (aiStateAttack == AIStateAttack.None)
        {
            aiStateAttack = AIStateAttack.JumpAttack;
        }
        Debug.Log("aistateAttack" + aiStateAttack);
        if (aiStateAttack == AIStateAttack.RunAndAttack || aiStateAttack == AIStateAttack.JumpAttack)
        {
            //animator.SetTrigger("Scream");
            animator.CrossFadeInFixedTime("Scream", 0.25f);
            if (aiStateAttack == AIStateAttack.RunAndAttack)
            {
                var distance = Vector3.Distance(transform.position, target.position);
                if (distance<= sight.RunningAttackRange)
                {
                    aiStateAttack = AIStateAttack.WalkAndAttack;
                }
            }
        }
    }
    public void SetRateTurn(float turn)
    {
        animator.SetFloat("Turn", turn);
    }
    public void Attacking_JumpAttackState()
    {
        if (target != null)
        {
            if (isAnimationRunning("Scream", 0) == false)
            {
                navmesh.SetDestination(target.position);
                var distance = Vector3.Distance(transform.position, target.position);
                if (distance <= sight.JumpAttackRange)
                {
                    //forward = LerpSpeed(forward, 0f, 5);

                    //if (forward <= 0.1f)
                    //{
                    //    forward = 0.0f;
                    //    animator.SetTrigger("JumpAttack");
                    //    forward = 0.0f;
                    //    //animator.CrossFadeInFixedTime("JumpAttack", 0.5f);
                    //    aiStateAttack = AIStateAttack.None;

                    //}
                    animator.SetTrigger("JumpAttack");
                    forward = 0.0f;
                    //animator.CrossFadeInFixedTime("JumpAttack", 0.5f);
                    aiStateAttack = AIStateAttack.None;
                }
                else if (distance > sight.JumpAttackRange /*+ 0.1f*/)
                {
                    LookAtPosition(navmesh.steeringTarget); // lookatplayer when move
                    forward = LerpSpeed(forward, 1f, 5);
                    currentWaitTime = chasingSetting.waiTimeToAttack;

                }


            }
            else
            {
                LookAtPosition(target.position); // look at player when scream

            }
        }

    }
    public float distanceTemp ;
    public float distanceright ;

    public bool canRunAttack = true;

    public void Attacking_RunAndAttackState()
    {
        if (target!= null)
        {
            if (canRunAttack)
            {
                if (isAnimationRunning("Scream", 0) == false)
                {
                    var dir = target.position + transform.right.normalized * distanceright - transform.position;
                    //var runTowardPos = dir.normalized * distanceTemp + target.position + target.right.normalized* distanceTemp;
                    var runTowardPos = target.position + transform.right.normalized * distanceright + dir.normalized * distanceTemp;
                    Debug.DrawLine(transform.position, runTowardPos, Color.blue);

                    navmesh.SetDestination(runTowardPos);
                    LookAtPosition(navmesh.steeringTarget);
                    var distance = Vector3.Distance(transform.position, target.position);
                    if (distance <= sight.RunningAttackRange)
                    {
                        //animator.SetBool("IsRunningAttack", true);
                        animator.SetTrigger("RunAttack");
                        canRunAttack = false;
                        //aiStateAttack = AIStateAttack.None;
                    }
                    else if (distance > sight.RunningAttackRange /*+ 0.1f*/)
                    {
                        LookAtPosition(navmesh.steeringTarget);
                        forward = LerpSpeed(forward, 1f, 3);
                    }
                }
                else
                {
                    LookAtPosition(target.position); // look at player when scream
                }
            }
            else
            {
                if (navmesh.remainingDistance <= navmesh.stoppingDistance)
                {
                    //walkingToDest = false;
                    forward = LerpSpeed(forward, 0, 3);

                    if (forward <= 0.1)
                    {
                        forward = 0;
                        canRunAttack = true;
                        currentWaitTime = attack.waiTimeIdle;
                        aiStateAttack = AIStateAttack.Idle;
                    }
                }
                else
                {
                    //walkingToDest = true;
                    LookAtPosition(navmesh.steeringTarget);
                    forward = LerpSpeed(forward, 1f, 3);
                }
            }
            
        }

    }
    //public void StopRunAttackAndChangeToAttackingIdle()
    //{
    //    animator.SetBool("IsRunningAttack", false);
    //}

    public void Attacking_WalkAndAttackState()
    {
        if (target != null)
        {

            if (isAnimationRunning("Scream", 0) == false)
            {
                //if (isAnimationRunning("One Hand Club Combo", 0) == false && isAnimationRunning("Dual Weapon Combo", 0) == false)
                //{
                navmesh.SetDestination(target.position);
                var distance = Vector3.Distance(transform.position, target.position);
                if (distance <= sight.WalkingComboRange)
                {
                    //forward = LerpSpeed(forward, 0f, 5);
                    //if (forward <= 0.1f)
                    //{
                    //    forward = 0.0f;
                    //    int Kindattack = UnityEngine.Random.Range(1, 3);
                    //    Debug.Log("kindAttack : " + Kindattack);

                    //    if (Kindattack == 1)
                    //    {
                    //        animator.SetTrigger("WalkAttack1");
                    //        //animator.CrossFadeInFixedTime("One Hand Club Combo", 0.5f);
                    //    }
                    //    else if (Kindattack == 2)
                    //    {
                    //        animator.SetTrigger("WalkAttack2");
                    //        //animator.CrossFadeInFixedTime("Dual Weapon Combo", 0.5f);
                    //    }
                    //    else
                    //    {
                    //        animator.SetTrigger("WalkAttack1");
                    //        //animator.CrossFadeInFixedTime("Dual Weapon Combo", 0.5f);

                    //    }
                    //    aiStateAttack = AIStateAttack.None;
                    //}
                    int Kindattack = UnityEngine.Random.Range(1, 3);
                    Debug.Log("kindAttack : " + Kindattack);

                    if (Kindattack == 1)
                    {
                        animator.SetTrigger("WalkAttack1");
                        //animator.CrossFadeInFixedTime("One Hand Club Combo", 0.5f);
                    }
                    else if (Kindattack == 2)
                    {
                        animator.SetTrigger("WalkAttack2");
                        //animator.CrossFadeInFixedTime("Dual Weapon Combo", 0.5f);
                    }
                    else
                    {
                        animator.SetTrigger("WalkAttack1");
                        //animator.CrossFadeInFixedTime("Dual Weapon Combo", 0.5f);
                    }
                    forward = 0.0f;
                    aiStateAttack = AIStateAttack.None;
                }
                else if (distance > sight.WalkingComboRange /*+ 0.1f*/)
                {
                    LookAtPosition(navmesh.steeringTarget); // lookatplayer when move
                    forward = LerpSpeed(forward, 0.5f, 5);
                    currentWaitTime = chasingSetting.waiTimeToAttack;
                }
                //}

            }
            else
            {
                LookAtPosition(target.position); // look at player when scream
            }
        }

    }
    public void ChangeToAttacking_IdleState()
    {
        aiStateAttack = AIStateAttack.Idle;
        currentWaitTime = attack.waiTimeIdle;
    }
    void AttackBehaviour()
    {
        //walkingToDest = false;
        currentLookTransform = null;
        forward = LerpSpeed(forward, 0, 15);
        //isLookAround = false;
        //isAttacking = true;


    }
    public void EffectBloodBitting()
    {
        var distance = Vector3.Distance(transform.position, target.position);

        if (distance <= sight.attackRange)
        {
            target.GetComponent<CharacterStats>().Damage(5.0f);
            target.GetComponent<CharacterStats>().SetIsAttacked();
            var obj = Instantiate(attack.Blood, target.position +Vector3.up*0.9f, target.rotation);
            Destroy(obj, 1.5f);

        }

    }
    //public void SetTargetIsBitted()
    //{
    //    isBittingAtNeckTarget = true;
    //    var distance = Vector3.Distance(transform.position, target.position);

    //    if (distance <= sight.attackRange)
    //    {
    //        target.GetComponent<CharacterStats>().SetIsBitted();

    //    }
    //}
    //public void SetTargetIsNotBitted()
    //{
    //    isBittingAtNeckTarget = false;

    //    target.GetComponent<CharacterStats>().SetIsNotBitted();
    //}
   
   
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
        if (aiStateAttack == AIStateAttack.Idle)
        {

        }
        else if (aiStateAttack == AIStateAttack.Turn)
        {
            //Debug.DrawRay(attack.head_Transform.position, attack.head_Transform.forward, Color.green);
            Vector3 start = transform.position;
            Vector3 dir = target.transform.position - start;
            float sightAngle = Vector3.Angle(dir, animator.GetBoneTransform(HumanBodyBones.Head).forward);
            Debug.DrawRay(start, dir, Color.yellow);
            Debug.DrawRay(animator.GetBoneTransform(HumanBodyBones.Head).position, animator.GetBoneTransform(HumanBodyBones.Head).forward, Color.green);

            if (sightAngle >= -100 && sightAngle <= 100)
            {
                attack.currentLookPosition.position = Vector3.Lerp(attack.currentLookPosition.position, target.position + Vector3.up * 2, Time.deltaTime);
            }
            else if (sightAngle < -100)
            {
                
                attack.currentLookPosition.position = Vector3.Lerp(attack.currentLookPosition.position, attack.currentLookMaxRightPos.position, Time.deltaTime);
            }
            else if (sightAngle > 100)
            {
                attack.currentLookPosition.position = Vector3.Lerp(attack.currentLookPosition.position, attack.currentLookMaxLeftPos.position, Time.deltaTime);
            }
            attack.lastRotationHead = animator.GetBoneTransform(HumanBodyBones.Head).localRotation;

            //animator.SetLookAtPosition(attack.currentLookPosition.position); ///animator.setLookatPosition chi xet duy nhat o OnanimatorIK
            //animator.SetLookAtWeight(1, 0, 1, 1);                           /// be xuong truc tiep chi o lateupdate 
        }
        else
        {
            //attack.lastRotationHead = Quaternion.Slerp(attack.lastRotationHead, animator.GetBoneTransform(HumanBodyBones.Head).localRotation, Time.deltaTime);
            //Debug.Log("attack.lastRotationHead after slerp: " + attack.lastRotationHead.eulerAngles);

            attack.head_Transform.localRotation = attack.lastRotationHead;   
            attack.currentLookPosition.position = animator.GetBoneTransform(HumanBodyBones.Head).position + animator.GetBoneTransform(HumanBodyBones.Head).forward * 3;

        }

    }
    public float speedOfHeadLookTarget=2;
    void OnAnimatorIK ()
    {
        if (target!=null)
        {
            if (aiStateAttack == AIStateAttack.Idle )
            {
                Vector3 start = transform.position ;
                Vector3 dir = target.transform.position  - start;
                float sightAngle = Vector3.SignedAngle(dir, transform.forward,Vector3.up);
                if (sightAngle >= -100 && sightAngle <=100)
                {
                    attack.currentLookPosition.position = Vector3.Lerp(attack.currentLookPosition.position, target.position + Vector3.up * 2, Time.deltaTime/ speedOfHeadLookTarget);
                }
                else if (sightAngle<-100)
                {
                    attack.currentLookPosition.position = Vector3.Lerp(attack.currentLookPosition.position, attack.currentLookMaxRightPos.position, Time.deltaTime / speedOfHeadLookTarget);

                }
                else if (sightAngle >100)
                {
                    attack.currentLookPosition.position = Vector3.Lerp(attack.currentLookPosition.position, attack.currentLookMaxLeftPos.position, Time.deltaTime / speedOfHeadLookTarget);

                }

                animator.SetLookAtPosition(attack.currentLookPosition.position);
                animator.SetLookAtWeight(1, 0, 1, 1);

            }
            else if ( aiStateAttack == AIStateAttack.Turn)
            {
                animator.SetLookAtPosition(attack.currentLookPosition.position);
                animator.SetLookAtWeight(1, 0, 1, 1);
            }
            else
            {

                //Vector3 start = transform.position;
                //Vector3 dir = target.transform.position - start;
                //float signedAngle = Vector3.SignedAngle(dir, transform.forward, Vector3.up);
                //if (signedAngle >100)
                //{
                //    attack.currentLookPosition.position = animator.GetBoneTransform(HumanBodyBones.Head).position + animator.GetBoneTransform(HumanBodyBones.Head).forward * 3;
                //}
                //else if (signedAngle <-100)
                //{
                //    attack.currentLookPosition.position = animator.GetBoneTransform(HumanBodyBones.Head).position + animator.GetBoneTransform(HumanBodyBones.Head).forward * 3;


                //}
                //else if (signedAngle >-100 && signedAngle <100)
                //{
                //    attack.currentLookPosition.position = animator.GetBoneTransform(HumanBodyBones.Head).position + animator.GetBoneTransform(HumanBodyBones.Head).forward * 3;
                //}
           
            }
        }
        
       
    }
    public bool isAnimationRunning(string name, int indexlayer)
    {
        return animator.GetCurrentAnimatorStateInfo(indexlayer).IsName(name);
    }
    public void TurnOnEffectAttack()
    {
        attack.effectAttack.SetActive(true);
    }
    public void TurnOffEffectAttack()
    {
        attack.effectAttack.SetActive(false);
    }
    public void TurnOnCollider()
    {
        attack.collidePlayer.TurnOnCollider();
    }

    public void TurnOffCollider()
    {
        attack.collidePlayer.TurnOffCollider();

    }
    private void OnDrawGizmos()
    {
        var center = transform.position;
        center.y += 0.01f;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(center, sight.sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, sight.attackRange);


        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, sight.StandingComboRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, sight.WalkingComboRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, sight.JumpAttackRange);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(center, sight.RunningAttackRange);

        //Gizmos.color = Color.black;
        //Gizmos.DrawWireSphere(center, meleeRange);
    }
}


