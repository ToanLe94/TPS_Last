using UnityEngine;
using System.Collections;

public class BloodySoundSFX : MonoBehaviour {

    public AudioClip[] fleshImpacts;
    public AudioClip[] fatalImpacts;

	// Use this for initialization
	void Start () {
	
	}

    public void ImpactHandler(Vector3 pos, Vector3 hitDirection, Vector3 hitNormal, bool isFatal)
    {
        if( isFatal)
        {
            if (fatalImpacts.Length == 0) return;
            AudioSource.PlayClipAtPoint(fatalImpacts[getRandom(0, fatalImpacts.Length)], pos);
        }
        else
        {
            if (fleshImpacts.Length == 0) return;
            AudioSource.PlayClipAtPoint(fleshImpacts[getRandom(0,fleshImpacts.Length)], pos);
        }
    }

    int randomPrev = 0;

    int getRandom(int min, int max)
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
            return getRandom(min, max);
        }
    }
}
