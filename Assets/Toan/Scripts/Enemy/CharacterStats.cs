using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;


public class CharacterStats : MonoBehaviour {

    protected CharacterController charactercontroller { get { return GetComponent<CharacterController>(); } set { charactercontroller = value; } }
    protected RagdollManager ragdollManager { get { return GetComponentInChildren<RagdollManager>(); } set { ragdollManager = value; } }

    [Range(0, 100)] public float health = 100;
	public bool  isZombie;
    public int faction;
    public MonoBehaviour [] scriptsToDisable;
    protected Animator animator;

    //variable chi dung cho player 
     GrimAnimator grimAnimator;
    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        grimAnimator = GetComponent<GrimAnimator>();
    }

    // Update is called once per frame
    void Update () {
		health = Mathf.Clamp (health, 0, 100);
    }

    public virtual void Damage(float damage)
    {
        //if (gameObject.tag!= "Player")
        //{
        //    AI ai = GetComponent<AI>();
        //    if (ai)
        //    {
        //        ai.isEnemyLookAround();
        //    }

        //}
        health -= damage;
       
        if (health <=0)
        {
            Die();
            //player_NW.Die();

        }
    }
  
    public void UpdateChangeHealth(float newvalue)
    {
        health = newvalue;
    }
    public void SetIsBitted()
    {
        animator.SetBool("IsBitted", true);
        animator.CrossFadeInFixedTime("BittingReaction",0.2f);

    }
    public void SetIsAttacked()
    {

        if (grimAnimator.GetIsPistol())
        {
            animator.CrossFadeInFixedTime("Rifle_Hit_L_RM", 1);

        }
        else if (!grimAnimator.GetIsPistol())
        {
            animator.CrossFadeInFixedTime("Rifle_Hit_L_RM", 1);

        }
    }
    public void SetIsNotBitted()
    {
        animator.SetBool("IsBitted", false);
    }
    public virtual void Die()
    {
        if (!isZombie)
        {
            if (charactercontroller != null)
            {
                charactercontroller.enabled = false;

            }
            RemoveColliders(GetComponents<Collider>());

            if (ragdollManager != null) // is model human
            {
                ragdollManager.Ragdoll();
            }

            if (gameObject.tag=="Player")
            {
                GetComponent<UserInput>().TurnOffAllCrosshairs();

            }
            if (scriptsToDisable.Length == 0)
            {
                Debug.Log("All scripts still working on this character but this is dead.");
                return;
            }
            foreach (MonoBehaviour script in scriptsToDisable)
            {

                script.enabled = false;
            }
        }
        else
        {
            //if (scriptsToDisable.Length == 0)
            //{
            //    Debug.Log("All scripts still working on this character but this is dead.");
            //    return;
            //}
            //foreach (MonoBehaviour script in scriptsToDisable)
            //{
            //    script.enabled = false;
            //}
            ////animator.SetTrigger("Dead");
            //RemoveColliders(GetComponents<Collider>());
            //RemoveColliders(GetComponentsInChildren<Collider>());

        }



    }
    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public bool IsDead
    {
        get
        {
            return health <= 0;            
        }
    }
    protected void RemoveColliders(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
