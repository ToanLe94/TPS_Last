using UnityEngine;
using System.Collections;

public class PalmHandler : BaseHandler {

    [Header("--------- PalmHandler Vars ----------")]
    public Transform palmLimb;      // point where the physical object will be created
    public Transform palmPhys;      // physical object
    public Transform[] skinsToHide;  // skinned meshes that will be hidden
    public Transform damagedForearm;
    //public Rigidbody bodyToAttachPalm;

    CharacterJoint cj;

	// Use this for initialization
	void Start () {
        base.Init();
        cj = palmPhys.GetComponent<CharacterJoint>();
	}

    override public void ImpactHandler(Vector3 pos, Vector3 hitDirection, Vector3 hitNormal, bool isFatal = false)
    {
        HealthStates hState = HealthProcessing();
        switch( hState )
        {
            case HealthStates.damaged:
                damagedForearm.gameObject.SetActive(true);
                skinsToHide[0].gameObject.SetActive(false);
                base.ImpactHandler(pos, hitDirection, hitNormal, true);
                break;
            case HealthStates.fatal:
                // hide skinned meshes
                for (int i = 0; i < skinsToHide.Length; i++)
                {
                    skinsToHide[i].gameObject.SetActive(false);
                }
                damagedForearm.gameObject.SetActive(false);
                // enable physical object
                palmPhys.transform.position = palmLimb.position;// - 0.04f * palmLimb.right + 0.035f * palmLimb.forward + 0.04f * palmLimb.up;
                palmPhys.transform.rotation = palmLimb.rotation;
                //Debug.Break();
                palmPhys.gameObject.SetActive(true);
                //palmPhys.GetComponent<Rigidbody>().AddForce(10f * hitDirection, ForceMode.Impulse);
                palmPhys.GetComponent<Rigidbody>().AddTorque(10f * Random.onUnitSphere, ForceMode.Impulse);
                // destroy this collider
                Destroy(transform.GetComponent<Joint>());
                Destroy(transform.GetComponent<Collider>());
                Destroy(transform.GetComponent<Rigidbody>());
                base.ImpactHandler(pos, hitDirection, hitNormal, true);
                break;
            default:
                base.ImpactHandler(pos, hitDirection, hitNormal);
                return;
        }
    }

    public void DestroyPalm(Vector3 pos, Vector3 hitDirection, Vector3 hitNormal, bool isFatal = false)
    {
        // hide skinned meshes
        for (int i = 0; i < skinsToHide.Length; i++)
        {
            skinsToHide[i].gameObject.SetActive(false);
        }
        damagedForearm.gameObject.SetActive(false);
        // enable physical object
        palmPhys.transform.position = palmLimb.position;// - 0.04f * palmLimb.right + 0.035f * palmLimb.forward + 0.04f * palmLimb.up;
        palmPhys.transform.rotation = palmLimb.rotation;
        //cj.connectedBody = bodyToAttachPalm;
        palmPhys.gameObject.SetActive(true);
        // destroy this collider
        Destroy(transform.GetComponent<Joint>());
        Destroy(transform.GetComponent<Collider>());
        Destroy(transform.GetComponent<Rigidbody>());
    }

}
