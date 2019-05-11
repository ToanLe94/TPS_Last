using UnityEngine;
using System.Collections;

public class BaseHandler : MonoBehaviour {

    // here is your skinned mesh objects which will be checked for collision with bullet; 
    [Header("--------- Skinned meshes ----------")]
    public SkinnedMeshRenderer[] skinsToCheckImpact;
    
    
    [Header("--------- Health ----------")]
    public int health = 5;          // overall health of the body part
    public int damagedHealth = 1;   // health of the body part when damaged skin will be shown

    BloodyHoleSFX bloodyHoleSFX;    // bullet impact sfx script

    // the states of the body part 
    // - damaged - when the damaged skin shown
    // - fatal - when the body part is detached
    protected enum HealthStates
    {
        none,
        damaged,
        fatal
    };

    void Start()
    {
        Init();
    }

    public void Init()
    {
        bloodyHoleSFX = transform.root.GetComponent<BloodyHoleSFX>();
    }

    // 
    virtual public void ImpactHandler(Vector3 pos, Vector3 hitDirection, Vector3 hitNormal, bool isFatal = false)
    {
        // make the hole
        bloodyHoleSFX.ImpactHandler(pos, hitDirection, hitNormal, skinsToCheckImpact, isFatal);
    }

    protected HealthStates HealthProcessing()
    {
        health -= 1;
        if (health == damagedHealth) return HealthStates.damaged;
        if (health == 0 ) return HealthStates.fatal;
        if (health < -1) health = -1;
        return HealthStates.none;
    }

    public void Fatality()
    {
        // fatal sfx when the part was cutted
        bloodyHoleSFX.Fatality();
    }

    

}
