using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraForCar : MonoBehaviour
{
    public GameObject car;
    private float carX;
    private float carY;
    private float carZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        carX = car.transform.eulerAngles.x;
        carY = car.transform.eulerAngles.y;
        carZ = car.transform.eulerAngles.z;
        transform.eulerAngles = new Vector3(carX - carX, carY, carZ - carZ);


    }
}
