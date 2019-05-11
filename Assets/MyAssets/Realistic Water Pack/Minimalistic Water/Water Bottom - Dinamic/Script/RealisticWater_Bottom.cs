using UnityEngine;


public class RealisticWater_Bottom : MonoBehaviour {
	
	public float scrollSpeedX = 0.007f;
	public float scrollSpeedY = 0.007f;
	public float scale = 2.0f;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().material.SetTextureScale ("_MainTex", new Vector2(scale,scale));
		GetComponent<Renderer>().material.SetTextureScale ("_BumpMap", new Vector2(scale,scale));
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float offsetX = Time.time * scrollSpeedX;
		float offsetY = Time.time * scrollSpeedY;
		GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", new Vector2(offsetX,offsetY));
		GetComponent<Renderer>().material.SetTextureOffset ("_BumpMap", new Vector2(offsetX,offsetY));
	}
}
