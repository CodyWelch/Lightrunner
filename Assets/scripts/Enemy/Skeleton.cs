using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshSourceTag))]
public class Skeleton : MonoBehaviour {

	NavMeshAgent pathfinder;
	public Transform target;
	[SerializeField]
	private bool playerDetected;
	private NavMeshAgent nav;
	Animator anim;
	private bool isDead = false;

	void Start () {
		anim = this.GetComponent<Animator> ();
		playerDetected = false;
		pathfinder = GetComponent<NavMeshAgent> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		StartCoroutine (UpdatePath ());
		//nav = GetComponent<NavMeshAgent> ();
		pathfinder.height = 0.5f;
		pathfinder.baseOffset = 0;
	}

	/*void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "Player") {
			Debug.Log ("Player detected");
			//playerDetected = true;
		}	
	}*/


	Vector3 targetPosition;

	IEnumerator UpdatePath(){

		/*NavMeshHit closestHit;
		if( NavMesh.SamplePosition(  this.transform.position, out closestHit, 10.0f, NavMesh.AllAreas ) ){
			//this.transform.position = closestHit.position;
			//go.AddComponent<NavMeshAgent>();
			//TODO
		}
*/
		float refreshRate = 0.25f;

		// need pause (to build navmesh?)
		yield return new WaitForSeconds (1);

		this.GetComponent<NavMeshAgent> ().enabled = true;
		if (target == null) {
			target = GameObject.FindGameObjectWithTag ("Player").transform;
		}
		pathfinder.enabled = false;
		targetPosition = transform.position;
		while (target != null) {
			

	/*		NavMeshHit hit;
			if (NavMesh.SamplePosition(target.position, out hit, 1.0f, NavMesh.AllAreas)) {
				target.position = hit.position;
				return true;
			}*/

			if (playerDetected && !isDead) {
				targetPosition = new Vector3 (target.position.x, 0, target.position.z);
				pathfinder.SetDestination (targetPosition);
				anim.SetBool ("moving", true);

			}else {
				if (Vector3.Distance (transform.position, target.transform.position) < 5) {
					/*pathfinder.enabled = true;
					target = GameObject.FindGameObjectWithTag ("Player").transform;
					playerDetected = true;
					targetPosition = new Vector3 (target.position.x, 0, target.position.z);*/
					PlayerDetected ();

				} else {
					//Debug.Log ("Player not detected.");
				}
					


			}
			yield return new WaitForSeconds (refreshRate);

				
		}
	}

	public void PlayerDetected(){
		Debug.Log("player detect");
		pathfinder.enabled = true;
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		playerDetected = true;
		targetPosition = new Vector3 (target.position.x, 0, target.position.z);

	}

	public void dead(){
		isDead = true;
		int current_exp = PlayerPrefs.GetInt ("experience");
		PlayerPrefs.SetInt ("experience", current_exp + 1);
		anim.SetTrigger ("dead");
		this.gameObject.GetComponent<tt_Modified_bsn_PainGiver> ().enabled = false;
		pathfinder.SetDestination (transform.position);
		pathfinder.enabled = false;
	}
}
