using UnityEngine;
using System.Collections;

public class BodyHandler : BaseHandler
{

    [Header("--------- Skins ----------")]
    public Transform healthySkin;
    public Transform damagedSkin;
    //public Transform fatalSkin;

    // Use this for initialization
    void Start () {
        base.Init();
    }

    override public void ImpactHandler(Vector3 pos, Vector3 hitDirection, Vector3 hitNormal, bool isFatal = false)
    {
        HealthStates hState = HealthProcessing();
        switch (hState)
        {
            case HealthStates.damaged:
                damagedSkin.gameObject.SetActive(true);
                //fatalSkin.gameObject.SetActive(true);
                healthySkin.gameObject.SetActive(false);
                base.ImpactHandler(pos, hitDirection, hitNormal, true);
                break;
            case HealthStates.fatal:
                damagedSkin.gameObject.SetActive(false);
                healthySkin.gameObject.SetActive(false);
                base.ImpactHandler(pos, hitDirection, hitNormal, true);
                break;
            default:
                base.ImpactHandler(pos, hitDirection, hitNormal);
                return;
        }
    }

}
