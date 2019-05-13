using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAppearBoss : MonoBehaviour
{
    public GameObject boss;
    public GameObject preventobj;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grim"))
        {
            boss.SetActive(true);
            preventobj.SetActive(true);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
