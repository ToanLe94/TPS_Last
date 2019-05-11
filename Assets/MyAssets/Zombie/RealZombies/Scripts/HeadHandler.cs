using UnityEngine;
using System.Collections;

// Here is we will handle the head behaviour on bullet impact

public class HeadHandler : BaseHandler
{

    [Header("--------- Head skins ----------")]
    public Transform healthySkin;
    public Transform damagedSkin;
    public Transform headPartialSkin;
    [Header("--------- Head internal parts ----------")]
    public Transform headParts;

    [Header("--------- Brain explosion center ----------")]
    public Transform explosionCenter;

    [Header("--------- Head chunks ----------")]
    Rigidbody[] headRigs;

    public Transform bloodSFX;

    // Use this for initialization
    void Start () {
        base.Init();
        
    }

    override public void ImpactHandler(Vector3 pos, Vector3 hitDirection, Vector3 hitNormal, bool isFatal = false)
    {
        HealthStates hState = HealthProcessing();
        switch (hState)
        {
            // When head was damaged we will hide external skin and show head internals
            case HealthStates.damaged:
                damagedSkin.gameObject.SetActive(true);
                healthySkin.gameObject.SetActive(false);
                headPartialSkin.gameObject.SetActive(true);
                base.ImpactHandler(pos, hitDirection, hitNormal, true);
                break;
            // head kaaBoooM!
            case HealthStates.fatal:
                damagedSkin.gameObject.SetActive(false);
                healthySkin.gameObject.SetActive(false);
                headPartialSkin.gameObject.SetActive(true);
                // head explosion
                StartCoroutine(headExplosion(hitDirection));
                base.ImpactHandler(pos, hitDirection, hitNormal, true);
                break;
            default:
                base.ImpactHandler(pos, hitDirection, hitNormal);
                return;
        }
    }

    IEnumerator headExplosion( Vector3 hitDirection )
    {
        yield return new WaitForFixedUpdate();
        headParts.position = this.transform.position;
        headParts.rotation = this.transform.rotation;
        headParts.gameObject.SetActive(true);
        //Debug.Break();
        headRigs = headParts.GetComponentsInChildren<Rigidbody>();
        for( int i=0; i<headRigs.Length; i++ )
        {
            headRigs[i].AddExplosionForce(2f, explosionCenter.position + 0.2f * Random.insideUnitSphere, 2f, 0.1f, ForceMode.Impulse);
            if (bloodSFX != null)
            {
                Transform t = (Transform)Instantiate(bloodSFX, headRigs[i].transform, false);
                Destroy(t.gameObject, Random.Range(4f, 6f));
            }
        }
        //Debug.Break();
        base.Fatality();
        yield return new WaitForFixedUpdate();
        this.transform.GetComponent<Rigidbody>().AddForce(100f * (hitDirection + Vector3.up), ForceMode.Impulse);
        while (Time.timeScale > 0.012f)
        {
            yield return new WaitForSecondsRealtime(0.02f);
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.01f, 0.2f);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        yield return new WaitForSeconds(0.03f);
        while (Time.timeScale < 1f)
        {
            yield return new WaitForSecondsRealtime(0.02f);
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1.02f, 0.1f);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        //Debug.Break();
    }

}
