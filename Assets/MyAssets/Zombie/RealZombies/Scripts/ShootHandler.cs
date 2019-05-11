using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class ShootHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.Log("You selected the " + hit.transform.name); // ensure you picked right object
                //Debug.DrawRay(hit.point, 10f*Vector3.up, Color.magenta);
                //Debug.DrawLine(hit.point, Camera.main.transform.position, Color.yellow);
                //Debug.Break();
                BaseHandler bh = hit.transform.GetComponent<BaseHandler>();
                if (bh != null) bh.ImpactHandler( hit.point, ray.direction.normalized, hit.normal );
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}
