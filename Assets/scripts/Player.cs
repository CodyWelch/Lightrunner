using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public GameObject gc;
	private  vp_FPPlayerEventHandler m_Player = null;
	int saved_health;

	void Awake(){
		
		gc = GameObject.FindGameObjectWithTag ("GameController");
		saved_health = PlayerPrefs.GetInt("health");
		//Debug.Log("Health in player : " + PlayerPrefs.GetInt("health"));

		m_Player = GetComponent<vp_FPPlayerEventHandler> ();
		//Debug.Log ("Event handler is " + m_Player);
		//m_Player = player_object.GetComponentInChildren<vp_FPPlayerEventHandler> ();
		if (saved_health == 0) {
		} else {
			//m_Player.Health.Set (10f);
			Debug.Log (m_Player.Health);
//			m_Player.Health.Set(saved_health);
		}
		//vp_LocalPlayer = 7;
	}

	void OnCollisionEnter(Collision collision){
		Debug.Log ("Player collided with " + collision.gameObject.tag);
		if (collision.transform.tag == "finalTile") {
			Debug.Log ("Player on final tile.");
			gc.GetComponent <GameController> ().LevelFinished ();
		}
	}

	void OnTriggerEnter(Collider collision){
		Debug.Log (gameObject.name + " has collided with " + collision.gameObject.name);
		if (collision.transform.tag == "finalTile") {
			Debug.Log ("Player on final tile.");
			gc.GetComponent <GameController> ().LevelFinished ();
		}
	}
		
}
