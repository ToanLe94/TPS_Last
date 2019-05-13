using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidePlayer : MonoBehaviour
{
    public GameObject blood_PS;
    public GameObject player;
    public AITyrant aiTyrant;
    public Collider col1;
    public Collider col2;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    void CreateBlood(Vector3 pos)
    {
        if (blood_PS)
        {
            GameObject bloodEffect = Instantiate(blood_PS, pos, new Quaternion(0, 0, 0, 0));
            Destroy(bloodEffect, 3f);
        }

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Grim")
        {
            //if (player.GetComponent<Animator>().GetInteger("Combo") != 0)
            //{
            //    Debug.Log("Kung fu hit");
            //    var animE = other.gameObject.GetComponent<Animator>();
            //    animE.SetInteger("Hurt", 1);
            //    other.GetComponent<AI>().attack.isCanShoot = false;
            //    other.gameObject.GetComponent<CharacterStats>().Damage(2);
            //    CreateBlood(other.transform.position + new Vector3(0, 0.6f, 0));
            //}
            
            if (aiTyrant.isAnimationRunning("WalkAttack1", 0) || aiTyrant.isAnimationRunning("WalkAttack2", 0) || aiTyrant.isAnimationRunning("JumpAttack", 0)|| aiTyrant.isAnimationRunning("RunAttack", 1))
            {
                player.GetComponent<CharacterStats>().Damage(10);
                CreateBlood(player.transform.position + new Vector3(0, 0.9f, 0));
                player.GetComponent<CharacterStats>().SetIsAttacked();
                //player.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, force));

            }


        }

    }
    public float force =5f;
    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Grim"))
        {

            if (aiTyrant.isAnimationRunning("WalkAttack1", 0) || aiTyrant.isAnimationRunning("WalkAttack2", 0) || aiTyrant.isAnimationRunning("JumpAttack", 0) || aiTyrant.isAnimationRunning("RunAttack", 1))
            {
                player.GetComponent<CharacterStats>().Damage(10);
                CreateBlood(player.transform.position + new Vector3(0, 0.9f, 0));
                player.GetComponent<CharacterStats>().SetIsAttacked();
                player.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, force));

            }
        }

    }

    public void TurnOnCollider()
    {
        col1.enabled = true;
        col2.enabled = true;
    }

    public void TurnOffCollider()
    {
        col1.enabled = false;
        col2.enabled = false;
    }
    //public void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Enemy")
    //    {

    //        var anim = other.gameObject.GetComponent<Animator>();
    //        anim.SetInteger("Hurt", 0);


    //    }
    //}
}
