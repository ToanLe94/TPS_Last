using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieStats : CharacterStats
{
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }
    private void Update()
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
        animator.CrossFadeInFixedTime("zombie death", 0.5f);
        RemoveColliders(GetComponents<Collider>());
        RemoveColliders(GetComponentsInChildren<Collider>());

    }

}
