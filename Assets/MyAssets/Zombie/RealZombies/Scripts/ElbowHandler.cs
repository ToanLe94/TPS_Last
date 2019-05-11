using UnityEngine;
using System.Collections;

public class ElbowHandler : BaseHandler
{
    [Header("--------- ElbowHandler Vars ----------")]
    public Transform elbowLimb;      // Elbow limb. We will create physics object here
    public Transform elbowPhys;      // Physical object (not ragdoll)
    public Transform palmPhys;       // Palm physical object
    public Transform[] skinsToHide;  // Skinned meshes that will be hidden
    public Transform damagedArm;     // damaged skinned mesh
    public float palmShiftDirection = 1f;  // it is needed for palm shifting if the palm is not destroyed

    public PalmHandler palmHandler; // call destroy palm if palm is not destroyed

    // Use this for initialization
    void Start()
    {
        base.Init();
    }

    override public void ImpactHandler(Vector3 pos, Vector3 hitDirection, Vector3 hitNormal, bool isFatal = false)
    {
        HealthStates hState = HealthProcessing();
        switch (hState)
        {
            case HealthStates.damaged:
                damagedArm.gameObject.SetActive(true);
                skinsToHide[0].gameObject.SetActive(false);
                base.ImpactHandler(pos, hitDirection, hitNormal, true);
                break;
            case HealthStates.fatal:
                // enable physical object
                elbowPhys.gameObject.SetActive(true);

                elbowPhys.transform.position = elbowLimb.position;// - 0.04f * palmLimb.right + 0.035f * palmLimb.forward + 0.04f * palmLimb.up;
                elbowPhys.transform.rotation = elbowLimb.rotation;
                // if palm is active
                if (palmPhys.gameObject.activeSelf)
                {
                    //elbowPhys.GetComponent<Rigidbody>().AddForce(10f * hitDirection, ForceMode.Impulse);
                    elbowPhys.GetComponent<Rigidbody>().AddTorque(10f * Random.onUnitSphere, ForceMode.Impulse);
   }
                else
                {
                    palmHandler.DestroyPalm(pos, hitDirection, hitNormal);
                    palmPhys.transform.position = elbowPhys.transform.position - 0.25f * palmShiftDirection * elbowPhys.transform.right;
                    palmPhys.transform.rotation = Quaternion.LookRotation(-elbowPhys.transform.forward, -elbowPhys.transform.up);
                    elbowPhys.gameObject.AddComponent<FixedJoint>().connectedBody = palmPhys.GetComponent<Rigidbody>();
                    //elbowPhys.GetComponent<Rigidbody>().AddForce(15f * hitDirection, ForceMode.Impulse);
                    elbowPhys.GetComponent<Rigidbody>().AddTorque(10f * Random.onUnitSphere, ForceMode.Impulse);
                    //Debug.Break();
                }

                // hide skinned meshes
                for (int i = 0; i < skinsToHide.Length; i++)
                {
                    skinsToHide[i].gameObject.SetActive(false);
                }
                damagedArm.gameObject.SetActive(false);
                base.ImpactHandler(pos, hitDirection, hitNormal, true);
                break;
            default:
                base.ImpactHandler(pos, hitDirection, hitNormal);
                return;
        }
    }

}
