using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kungfu : MonoBehaviour
{
    public GameObject blood_PS;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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

        if (other.tag == "Enemy")
        {

            if (player.GetComponent<Animator>().GetInteger("Combo")!= 0 )
            {
                Debug.Log("Kung fu hit");
                var animE = other.gameObject.GetComponent<Animator>();
                animE.SetInteger("Hurt", 1);
                other.GetComponent<AI>().attack.isCanShoot = false;
                other.gameObject.GetComponent<CharacterStats>().Damage(2);
                CreateBlood(other.transform.position + new Vector3(0, 0.6f, 0));
            }
           


        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {

            var anim = other.gameObject.GetComponent<Animator>();
            anim.SetInteger("Hurt", 0);


        }
    }
}
