using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManagerEnemy : MonoBehaviour
{
    private Collider[] colliders { get { return GetComponentsInChildren<Collider>(); } set { colliders = value; } }
    private Rigidbody[] rigidBodies { get { return GetComponentsInChildren<Rigidbody>(); } set { rigidBodies = value; } }
    private Animator animator { get { return GetComponentInParent<Animator>(); } set { animator = value; } }


    // Use this for initialization
    void Start()
    {
        if (colliders.Length == 0)
        {
            return;
        }
        if (rigidBodies.Length == 0)
        {
            return;
        }


        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
        foreach (Rigidbody r in rigidBodies)
        {
            r.isKinematic = true;
            //r.useGravity = false;
        }
    }
    public void Ragdoll()
    {
        if (animator == null)
        {
            return;
        }
        if (colliders.Length == 0)
        {
            return;
        }
        if (rigidBodies.Length == 0)
        {
            return;
        }

        animator.enabled = false;
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }
        foreach (Rigidbody r in rigidBodies)
        {
            r.isKinematic = false;
            //r.useGravity = true;
        }
    }

}
