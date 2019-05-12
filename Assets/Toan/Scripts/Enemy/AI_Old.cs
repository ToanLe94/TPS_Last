using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(Animator))]
public class AI_Old : MonoBehaviour
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
    public Action<bool> isEnemyLookAround;


    [System.Serializable]
	public class PatrolSettings
	{
		public WaypointBase[] waypoints;
	}
	public PatrolSettings patrolSettings;

	[System.Serializable]
	public class SightSettings
	{
		public LayerMask sightLayers;
		public float sightRange = 20f;
        public float shootingRange = 8;

		public float fieldOfView = 120f;
		public float eyeheight = 0.7f;

	}
	public SightSettings sight;

    [System.Serializable]
    public class AttackSettings
    {
        public float fireChance = 0.1f;

        public float rateVertical_LookatTarget = 0.3f;
        public float ratehorizontal_LookatTarget = 0.1f;
        public float ratehorizontal_LookatTarget_rifle = -0.3f;
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

    private bool aiming;

    public Vector3 rotatespine;
	// Use this for initialization
	void Start () {
		navmesh = GetComponentInChildren<UnityEngine.AI.NavMeshAgent> ();
        isEnemyLookAround += EnemyLookAround;

        if (navmesh == null) {
			Debug.LogError ("We need a navmesh to traverse the world with.");
			enabled = false;
            
			return;
		}

		if (navmesh.transform == this.transform) {
			Debug.LogError ("The navmesh agent should be a child of the character: " + gameObject.name);
			enabled = false;
			return;
		}

		navmesh.speed = 0;
		navmesh.acceleration = 0;
		navmesh.autoBraking = false;

		if (navmesh.stoppingDistance == 0) {
			Debug.Log ("Auto settings stopping distance to 1.3f");
			navmesh.stoppingDistance = 1.3f;
		}

		GetAllCharacters ();
	}

    private void EnemyLookAround(bool isLookAround)
    {
        if (target == null)
        {
            animator.SetBool("LookAround", isLookAround);
            StartCoroutine("ResetLookAround");
        }
       
    }
    IEnumerator ResetLookAround()
    {
        yield return new WaitForSeconds(6);
        animator.SetBool("LookAround", false);

    }
    void GetAllCharacters () {

		allCharacters = GameObject.FindObjectsOfType<CharacterStats>();
	}
	
	// Update is called once per frame
	void Update () {
        allCharacters = GameObject.FindObjectsOfType<CharacterStats>();

        //TODO: Animate the strafe when the enemy is trying to shoot us.
        characterMove.Animate (forward, 0);
        //navmesh.transform.position = transform.position;

        LookForTarget ();
        weaponHandler.Aim(aiming);


        switch (aiState)
        {
		    case AIState.Patrol:
			    Patrol ();
			    break;
            case AIState.Attack:
                 FireAtEnemy();
                 break;
		}
	}

	void LookForTarget ()
    {
		if (allCharacters.Length > 0)
        {
			foreach (CharacterStats c in allCharacters)
            {
				if (c != characterStats && c.gameObject.tag=="Player" && c.faction != characterStats.faction && c == ClosestEnemy())
                {
					RaycastHit hit;
					Vector3 start = transform.position + (transform.up* sight.eyeheight);
					Vector3 dir = (c.transform.position + c.transform.up * c.transform.GetComponent<CharacterMovement>().heightCharacter/2) - start;
					float sightAngle = Vector3.Angle (dir, transform.forward);
                    //Debug.Log("angle Dir vs tranform.forward" + sightAngle);
                    Debug.DrawRay(start, dir, Color.red, sight.sightRange);
                    if (Physics.Raycast (start, dir, out hit, sight.sightRange, sight.sightLayers))
                    {
                        if (hit.transform.tag != "Player")
                        {
                            if (target != null)
                            {
                                targetLastKnownPosition = target.position;
                                target = null;
                            }
                            
                            break;
                        }
                        if (hit.transform.tag == "Player" && sightAngle < sight.fieldOfView && hit.collider.GetComponent<CharacterStats>())
                        {
                            target = hit.transform;
                            targetLastKnownPosition = Vector3.zero;
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

	void Patrol ()
    {
        if (target == null)
        {
            aiState = AIState.Patrol;
            PatrolBehaviour();


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
                forward = LerpSpeed(forward, 0, 15);

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
                forward = LerpSpeed(forward, 0.5f, 15);
                currentWaitTime = patrolSettings.waypoints[waypointIndex].waitTime;
                currentLookTransform = null;
            }
        }
        else
        {
            aiState = AIState.Attack;
        }
	}

    void FireAtEnemy()
    {
        if (target!= null)
        {
            AttackBehaviour();
            LookAtPosition(target.position);
            Vector3 start = transform.position + transform.up*sight.eyeheight;

            Vector3 dir = target.position - transform.position;
            //Debug.Log("target position " + target.position);
            //Debug.DrawRay(start, dir, Color.green);

            Ray ray = new Ray(start, dir);
            if (UnityEngine.Random.value <= attack.fireChance )
            {
                weaponHandler.FireCurrentWeapon(ray);
            }
        }
        else
        {
            aiState = AIState.Patrol;
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
            //Debug.Log("goc xoayyyyyyyyyyyyyyyyyyyyyyy:" + Vector3.Angle(transform.forward, weaponHandler.currentWeapon.weaponSettings.bulletSpawn.forward));
            //Debug.Log("goc xoayyyyyyyyyyyyyyyyyyyyyyy vector3:" + Quaternion.FromToRotation(transform.forward, weaponHandler.currentWeapon.weaponSettings.bulletSpawn.forward).eulerAngles);
            //Vector3 rotate = Quaternion.FromToRotation(transform.forward, weaponHandler.currentWeapon.weaponSettings.bulletSpawn.forward).eulerAngles;
            if (aiming)
            {
                Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
               
                Quaternion lookRot = Quaternion.LookRotation(target.position/*+target.up* target.GetComponent<CharacterMovement>().heightCharacter/2*/ - transform.position);
                lookRot.y = 0;

                //Debug.Log("goc moi ben AI :" + lookRot.eulerAngles);
                //Debug.Log("goc moi ben AI Quaterion :" + lookRot.x);


                if (weaponHandler.currentWeapon.weaponType == Weapon.WeaponType.Rifle)
                {
                    spine.Rotate(new Vector3(lookRot.eulerAngles.x, 30, 0));
                    //30 lay tu debug o linecode 265 den 267 .vi animation lam cho bspawn goc khong trung voi Ray Fire(Ray ray) o class Weapon

                }
                else if (weaponHandler.currentWeapon.weaponType == Weapon.WeaponType.Pistol)
                {
                    //4,26,5 lay tu debug o linecode 265 den 267 .vi animation lam cho bspawn goc khong trung voi Ray Fire(Ray ray) o class Weapon
                    spine.Rotate(lookRot.eulerAngles.x+ 4, 26,  5);
         
                }

            }
        }
        
    }
    void OnAnimatorIK () {
		if (currentLookTransform != null && !walkingToDest)
        {
			animator.SetLookAtPosition (currentLookTransform.position);
			animator.SetLookAtWeight (1, 0, 0.5f, 0.7f);
		}
        else if (target != null)
        {
            //cach 2: toi line code 325
            //Debug.Log("goc xoayyyyyyyyyyyyyyyyyyyyyyy:" + Vector3.Angle(transform.forward, weaponHandler.currentWeapon.weaponSettings.bulletSpawn.forward));
            //Debug.Log("goc xoayyyyyyyyyyyyyyyyyyyyyyy vector3:" + Quaternion.FromToRotation(transform.forward, weaponHandler.currentWeapon.weaponSettings.bulletSpawn.forward).eulerAngles);

            //if (weaponHandler.currentWeapon.weaponType == Weapon.WeaponType.Rifle)
            //{
            //    float dist = Vector3.Distance(target.position, transform.position);

            //    Vector3 posLookat_maxdistance = target.transform.position - transform.right * 0.3f - transform.up;

            //    Vector3 poslookat = target.transform.position + target.up * target.GetComponent<CharacterMovement>().heightCharacter / 2 - transform.up * attack.rateVertical_LookatTarget * dist - transform.right * attack.ratehorizontal_LookatTarget_rifle * dist;
            //    animator.SetLookAtPosition(poslookat);
            //    animator.SetLookAtWeight(1, 1, 0.3f, 0.2f);
            //}
            //else if (weaponHandler.currentWeapon.weaponType == Weapon.WeaponType.Pistol)
            //{
            //    float dist = Vector3.Distance(target.position, transform.position);

            //    Vector3 posLookat_maxdistance = target.transform.position - transform.right * 0.3f - transform.up;

            //    Vector3 poslookat = target.transform.position + target.up * target.GetComponent<CharacterMovement>().heightCharacter / 2 - transform.up * attack.rateVertical_LookatTarget * dist - transform.right * attack.ratehorizontal_LookatTarget * dist;
            //    animator.SetLookAtPosition(poslookat);
            //    animator.SetLookAtWeight(1, 1, 0.3f, 0.2f);
            //}
            


            //if (dist >1 )
            //{
            //    Vector3 posLookat_maxdistance = target.transform.position - transform.right * 0.3f - transform.up;


            //    animator.SetLookAtPosition(target.transform.position - transform.right * 0.3f - transform.up);
            //    animator.SetLookAtWeight(1, 1, 0.3f, 0.2f);


            //    //righthandBoneIK.position = target.transform.position;
            //    //animator.SetLookAtPosition(righthandBoneIK.position);
            //    //animator.SetLookAtWeight(1, 1);


            //    Vector3 start = transform.position + transform.up * sight.eyeheight;
            //    Vector3 dir = target.position - transform.position;
            //    Debug.DrawRay(start, dir, Color.blue);
            //    Ray ray = new Ray(start, dir);
            //    Transform bulletspawn = weaponHandler.currentWeapon.weaponSettings.bulletSpawn;
            //    Vector3 dirFrombulletSpawn = ray.GetPoint(weaponHandler.currentWeapon.weaponSettings.range) - bulletspawn.position;
            //    Debug.DrawRay(bulletspawn.position, dirFrombulletSpawn, Color.cyan);
            //    Debug.DrawRay(bulletspawn.position, bulletspawn.forward, Color.yellow);

            //    //Transform rightHand_boneT = animator.GetBoneTransform(HumanBodyBones.RightHand);
            //    //animator.SetIKRotation(AvatarIKGoal.RightHand,  Quaternion.FromToRotation(bulletspawn.forward, dirFrombulletSpawn) * rightHand_boneT.rotation);
            //    //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            //    //Quaternion qua= Quaternion.FromToRotation(bulletspawn.forward, dirFrombulletSpawn);
            //    //Quaternion newRot = Quaternion.Euler(qua.eulerAngles + righthandBoneIK.eulerAngles);
            //    //animator.SetIKRotation(AvatarIKGoal.RightHand, newRot);
            //    //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            //}
            //else
            //{
            //    animator.SetLookAtPosition(target.transform.position + target.up * target.GetComponent<CharacterMovement>().heightCharacter/2);
            //    animator.SetLookAtWeight(1, 1, 0.3f, 0.2f);

            //}

        }

        if (weaponHandler.currentWeapon && weaponHandler.currentWeapon.userSettings.leftHandIKTarget && weaponHandler.getWeaponType() == 2 && !weaponHandler.getReload() && !weaponHandler.getSettingWeapon())
        {

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            Transform target = weaponHandler.currentWeapon.userSettings.leftHandIKTarget;
            Vector3 targetPos = target.position;
            Quaternion targetRot = target.rotation;
            animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPos);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, targetRot);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }
    void PatrolBehaviour()
    {
        aiming = false;

    }
    void AttackBehaviour()
    {
        aiming = true;
        walkingToDest = false;
        setDestination = false;
        reachedDestination = false;
        currentLookTransform = null;
        forward = LerpSpeed(forward, 0, 15);
        animator.SetBool("LookAround", false);
    }
    private void OnDrawGizmos()
    {
        //var center = transform.position;
        //center.y += 0.01f;
        //Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(center, sight.sightRange);

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(center, sight.shootingRange);
        //Gizmos.color = Color.black;
        //Gizmos.DrawWireSphere(center, meleeRange);
    }
}


