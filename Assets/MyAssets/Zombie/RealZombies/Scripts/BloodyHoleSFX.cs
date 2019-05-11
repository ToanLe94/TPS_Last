using UnityEngine;
using System.Collections;

public class BloodyHoleSFX : MonoBehaviour {

    public bool isDoubleSided = true;       // if the bullet penetrates through

    public Transform fakeObject;            // the fake invisible object for bake skinned mesh collider

    BloodySpurtSFX spurtSFX;                // Blood spurt SFX handler
    BloodySoundSFX soundSFX;                // Sound SFX system

    Material zmat;

    // ====================================
    Color[][] bloodPixels = new Color[4][];
    Color[] srcPixels;

    Texture2D bloodDecal;                   // blood effects decal texture
    public Texture2D tspurt;//, nspurt;     // blood effects source 
    public Texture2D grayTex;               // gray reference texture

    Mesh m;                                 // mesh, in wich we will bake the skinned mesh
    RaycastHit rHit, lHit;                  // direct and reverse raycast result
    int lm = 0;                             // layer mash for skinned mesh (baked)

    const float decalSize = 1024f;          // the size of the base decal texture
    const int spurtSize = 32;               // spurt effect size
    const float spurtSizeDiv2 = 16f;
    const float decalShift = decalSize - spurtSize;

    int rndSpurtPrev = 0;

    // Use this for initialization
    void Start () {

        spurtSFX = GetComponent<BloodySpurtSFX>();
        soundSFX = GetComponent<BloodySoundSFX>();

        //Color32[] col32mask = new Color32[(int)(decalSize * decalSize)];
        m = new Mesh();
        lm = (1 << LayerMask.NameToLayer("noCollision"));  

        // blood decal texture. We will use the details texture of the standard shader
        bloodDecal = new Texture2D((int)decalSize, (int)decalSize, TextureFormat.RGB24, true);  
        // load gray texture (base)
        bloodDecal.LoadRawTextureData(grayTex.GetRawTextureData());
        bloodDecal.Apply();

        // load four spurt variants
        bloodPixels[0] = tspurt.GetPixels(0, 0, spurtSize, spurtSize);
        bloodPixels[1] = tspurt.GetPixels(spurtSize, 0, spurtSize, spurtSize);
        bloodPixels[2] = tspurt.GetPixels(0, spurtSize, spurtSize, spurtSize);
        bloodPixels[3] = tspurt.GetPixels(spurtSize, spurtSize, spurtSize, spurtSize);

        // get all skinned meshes and grab the external material
        SkinnedMeshRenderer[] bodySkins = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < bodySkins.Length; i++)
        {
            // grab first object with externar material
            if (bodySkins[i].GetComponent<SkinnedMeshRenderer>().material.name.Contains("external"))
            {
                // make the instance of the material
                zmat = new Material(bodySkins[i].GetComponent<SkinnedMeshRenderer>().material);
                zmat.EnableKeyword("_DETAIL_MULX2");
                zmat.SetTexture("_DetailAlbedoMap", bloodDecal);
                break;
            }
        }
                
        for (int i = 0; i < bodySkins.Length; i++)
        {
            if (bodySkins[i].GetComponent<SkinnedMeshRenderer>().material.name.Contains("external" ))
            {
                bodySkins[i].GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_DETAIL_MULX2");
                bodySkins[i].GetComponent<SkinnedMeshRenderer>().material = zmat;
            }
        }

        // make a copy of the decal texture in memory
        srcPixels = bloodDecal.GetPixels();
    }

    int getRndSpurt()
    {
        int rndSpurt = Random.Range(0, 4);
        if (rndSpurt != rndSpurtPrev)
        {
            rndSpurtPrev = rndSpurt;
            //Debug.Log("RND Spurt: " + rndSpurt);
            return rndSpurt;
        }
        else
        {
            return getRndSpurt();
        }
    }

    void bloodHoleMark(SkinnedMeshRenderer sm, Vector3 hitPoint, Vector3 hitDir, Vector3 hitNormal)
    {
        // if the skinned mesh is active, do
        if (!sm.transform.gameObject.activeSelf) return;
        // bake the mesh to fakeObject
        sm.BakeMesh(m);
        fakeObject.GetComponent<MeshFilter>().mesh = m;
        // set fakeobject to current skinned mesh pos
        fakeObject.position = sm.transform.position;
        fakeObject.rotation = sm.transform.rotation;
        // set mesh collider
        fakeObject.GetComponent<MeshCollider>().sharedMesh = m;

        // cast a ray into baked mesh and detect impact point
        if (Physics.Raycast(hitPoint - 0.5f * hitDir, hitDir, out rHit, 0.7f, lm))
        {
            float a = 0;
            // get random effect (is not equal to the previous)
            int spurtNum = getRndSpurt();

            if ( isDoubleSided )
            // detect the second point
            if (Physics.Raycast(hitPoint + 0.7f * hitDir, -hitDir, out lHit, 1f, lm) )
            {
                // get current pixels
                srcPixels = bloodDecal.GetPixels((int)Mathf.Clamp(lHit.textureCoord.x * decalSize - spurtSizeDiv2, 0, decalShift), (int)Mathf.Clamp(lHit.textureCoord.y * decalSize - spurtSizeDiv2, 0, decalShift), spurtSize, spurtSize);
                
                for (int i = 0; i < srcPixels.Length; i++)
                {
                    a = bloodPixels[spurtNum][i].a;
                    if (srcPixels[i].grayscale < a)
                    {
                        srcPixels[i].r = a;
                        srcPixels[i].g = a;
                        srcPixels[i].b = a;
                    }
                    srcPixels[i] *= Color.white * bloodPixels[spurtNum][i];
                }
                bloodDecal.SetPixels((int)Mathf.Clamp(lHit.textureCoord.x * decalSize - spurtSizeDiv2, 0, decalShift), (int)Mathf.Clamp(lHit.textureCoord.y * decalSize - spurtSizeDiv2, 0, decalShift), spurtSize, spurtSize, srcPixels);
            }
            // magic
            srcPixels = bloodDecal.GetPixels((int)Mathf.Clamp(rHit.textureCoord.x * decalSize - spurtSizeDiv2, 0, decalShift), (int)Mathf.Clamp(rHit.textureCoord.y * decalSize - spurtSizeDiv2, 0, decalShift), spurtSize, spurtSize);

            for (int i = 0; i < srcPixels.Length; i++)
            {

                a = bloodPixels[spurtNum][i].a;
                
                if (srcPixels[i].grayscale < a)
                {
                    srcPixels[i].r = a;
                    srcPixels[i].g = a;
                    srcPixels[i].b = a;
                }
                srcPixels[i] *= bloodPixels[spurtNum][i];

            }
            bloodDecal.SetPixels((int)Mathf.Clamp(rHit.textureCoord.x * decalSize - spurtSizeDiv2, 0, decalShift), (int)Mathf.Clamp(rHit.textureCoord.y * decalSize - spurtSizeDiv2, 0, decalShift), spurtSize, spurtSize, srcPixels);

            bloodDecal.Apply(true);
        }
        // free fake object
        fakeObject.GetComponent<MeshCollider>().sharedMesh = null;
    }

    public void ImpactHandler(Vector3 pos, Vector3 hitDirection, Vector3 hitNormal, SkinnedMeshRenderer[] bodies, bool isFatal )
    {
        foreach( SkinnedMeshRenderer sm in bodies )
        {
            if( sm.gameObject.activeSelf ) bloodHoleMark(sm, pos, hitDirection, hitNormal);
        }
        // bloody spurt sfx
        if (spurtSFX != null) spurtSFX.ImpactHandler(pos, hitDirection, hitNormal, isFatal);
        // sound sfx
        if (soundSFX != null) soundSFX.ImpactHandler(pos, hitDirection, hitNormal, isFatal);
    }

    public void Fatality()
    {
        // if fatal impact - make all rigids nonKinematic
        transform.GetComponentInChildren<Animator>().enabled = false;
        Rigidbody[] rgs = transform.GetComponentsInChildren<Rigidbody>();
        for( int i=0; i<rgs.Length; i++)
        {
            rgs[i].isKinematic = false;
            rgs[i].useGravity = true;
        }
        fakeObject.GetComponent<Rigidbody>().isKinematic = true;
    }
    // thats all
}
