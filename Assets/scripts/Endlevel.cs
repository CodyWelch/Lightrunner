using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endlevel : MonoBehaviour {

	public GameObject gc;

    private void Start()
    {
        if(gc==null)
        {
            gc = GameObject.FindGameObjectWithTag("GameController");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " has collided with " + other.gameObject.name);
        if (other.transform.tag == "Player")
        {
            Debug.Log("Player on final tile.");
            gc.GetComponent<GameController>().LevelFinished();
        }
    }

    void OnCollisionEnter(Collision collision){
		Debug.Log (gameObject.name + " has collided with " + collision.gameObject.name);
		if (collision.transform.tag == "Player") {
			Debug.Log ("Player on final tile.");
			gc.GetComponent <GameController> ().LevelFinished ();
		}
	}
}
