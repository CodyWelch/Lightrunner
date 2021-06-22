using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agro : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(){

		if (GetComponent<Collider>().gameObject.tag == "Player") {
			Debug.Log ("Found Player");
		}
	}
}
