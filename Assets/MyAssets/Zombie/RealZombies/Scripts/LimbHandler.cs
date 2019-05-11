using UnityEngine;
using System.Collections;

public class LimbHandler : BaseHandler
{

    [Header("--------- TemplateHandler Vars ----------")]
    public Transform healthySkin;
    public Transform damagedSkin;

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
