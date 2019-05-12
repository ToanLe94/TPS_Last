using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : CharacterStats
{
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, 100);


    }
    public override void Die()
    {

        if (scriptsToDisable.Length == 0)
        {
            Debug.Log("All scripts still working on this character but this is dead.");
            return;
        }
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            script.enabled = false;
        }
        //int kindDead = UnityEngine.Random.Range(1, 2);
        //if (kindDead == 1)
        //{

        //    animator.CrossFadeInFixedTime("Creature_armature1|death_1", 0.5f);

        //}
        //else if (kindDead == 2)
        //{
        //    animator.CrossFadeInFixedTime("Creature_armature1|death_2", 0.5f);

        //}

        animator.CrossFadeInFixedTime("death", 0.5f);
        RemoveColliders(GetComponents<Collider>());
        RemoveColliders(GetComponentsInChildren<Collider>());

    }
}
