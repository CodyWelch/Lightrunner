using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

	public float timeBetweenAttacks = 0.5f;
	public int attackDamage = 10;
	private vp_FPPlayerEventHandler PlayerEvents = null;

	Animator anim;
	GameObject player;
	//PlayerHealth playerHealth;
	//EnemyHealth enemyHealth;
	bool playerInRange;
	float timer;

	void Awake(){
		// refs
		player = GameObject.FindGameObjectWithTag("Player");
		//
		//
		anim = GetComponent<Animator>();
		//PlayerEvents = Player.transform.GetComponent<vp_FPPlayerEventHandler> ();

	}

	void OnTriggerEnter (Collider other){
		// if the entering collider is the player...
		if (other.gameObject == player) {
			// ... the player is in range.
			playerInRange = true;
		}
	}

	void OnTriggerExit (Collider other){
		// if the existing collider is the player...
		if (other.gameObject == player) {
			// ... the player is no longer in range.
			playerInRange = false;
		}
	}
	
	void Update () {
		// Add the time since Update was last called to the timer.
		timer += Time.deltaTime;

		// if the timer exceeds the time between attacks, the player is in range and
		if (timer >= timeBetweenAttacks && playerInRange) {
			// .. attack
			Attack();
		}

		// If the player has zero or less health...

	}

	void Attack(){
	}


}
