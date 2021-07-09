using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTile : MonoBehaviour {


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " has collided with " + other.gameObject.name);
        if (other.transform.tag == "Player")
        {
            Debug.Log("Player on final tile.");
            GameController.instance.LevelFinished();
        }
    }

    void OnCollisionEnter(Collision collision){
		Debug.Log (gameObject.name + " has collided with " + collision.gameObject.name);
		if (collision.transform.tag == "Player") {
			Debug.Log ("Player on final tile.");
			GameController.instance.LevelFinished();
		}
	}
}
