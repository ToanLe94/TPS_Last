using UnityEngine;


public class RealisticWater : MonoBehaviour {
	
	public float scrollSpeedX = 0.005f;
	public float scrollSpeedY = 0.005f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float offsetX = Time.time * scrollSpeedX;
		float offsetY = Time.time * scrollSpeedY;
		GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", new Vector2(offsetX,offsetY));
		GetComponent<Renderer>().material.SetTextureOffset ("_BumpMap", new Vector2(offsetX,offsetY));
	}
}
