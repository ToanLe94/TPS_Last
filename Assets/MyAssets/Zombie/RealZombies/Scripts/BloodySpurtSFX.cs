using UnityEngine;
using System.Collections;

public class BloodySpurtSFX : MonoBehaviour {

    public Transform[] bulletImpacts;
    public float bulletSFX_lifetime = 2f;
    public Transform[] spurtImpacts;
    public float spurtSFX_lifetime = 4f;

    // Use this for initialization
    void Start () {
	
	}

    public void ImpactHandler(Vector3 pos, Vector3 hitDirection, Vector3 hitNormal, bool isFatal)
    {
        if (isFatal)
        {
            if (spurtImpacts.Length == 0) return;
            Transform t = (Transform)Instantiate(spurtImpacts[getRandom(0, spurtImpacts.Length)], pos, Quaternion.identity);
            Destroy(t.gameObject, spurtSFX_lifetime);
        }
        else
        {
            if (bulletImpacts.Length == 0) return;
            Transform t = (Transform)Instantiate( bulletImpacts[getRandom(0, bulletImpacts.Length)], pos, Quaternion.LookRotation(-hitDirection) );
            Destroy(t.gameObject, bulletSFX_lifetime);
            //Debug.Break();
        }
    }


    int randomPrev = 0;

    int getRandom( int min, int max )
    {
        int rnd = Random.Range(min, max);
        if (rnd != randomPrev)
        {
            randomPrev = rnd;
            //Debug.Log("RND Spurt: " + rndSpurt);
            return rnd;
        }
        else
        {
            return getRandom( min, max );
        }
    }

}
