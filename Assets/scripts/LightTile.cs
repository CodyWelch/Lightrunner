using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTile: MonoBehaviour {

	[SerializeField]
	private Material newMaterial;

	void OnCollisionEnter(Collision collision){
		Debug.Log (collision.transform.tag);
		if (collision.transform.tag == "Player") {
			Debug.Log ("light tile");
			this.gameObject.GetComponent<Renderer> ().material = newMaterial;
		}
	}
}
