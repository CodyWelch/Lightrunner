using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node : MonoBehaviour {



	void Update(){
		Renderer rend = this.GetComponent<Renderer> ();
		//Material mat = renderer.material;

		float emission = Mathf.PingPong (Time.time, 1.0f);
		//Color baseColor = rend.material.color;
		Color baseColor = Color.yellow;

		Color finalColor = baseColor * Mathf.LinearToGammaSpace (emission);
		rend.material.SetColor ("_EmissionColor", finalColor);
	}
}
