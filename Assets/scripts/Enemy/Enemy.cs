using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshSourceTag))]
public class Enemy : MonoBehaviour {

	NavMeshAgent pathfinder;
	public Transform target;
	[SerializeField]
	private bool playerDetected;
	private NavMeshAgent nav;

	void Start () {

		playerDetected = false;
		pathfinder = GetComponent<NavMeshAgent> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		StartCoroutine (UpdatePath ());

		//nav = GetComponent<NavMeshAgent> ();
	}

	/*void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "Player") {
			Debug.Log ("Player detected");
			//playerDetected = true;
		}	
	}*/

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


		if (target == null) {
			target = GameObject.FindGameObjectWithTag ("Player").transform;
		}
		pathfinder.enabled = true;
		NavMeshHit hit;
		if (NavMesh.SamplePosition(target.position, out hit, 1.0f, NavMesh.AllAreas)) {
			transform.position = hit.position;
		}
		//pathfinder.SetDestination (transform.position);
		pathfinder.enabled = false;
		Vector3 targetPosition = transform.position;
		while (target != null) {




			if (playerDetected) {
				targetPosition = new Vector3 (target.position.x, 0, target.position.z);
				pathfinder.SetDestination (targetPosition);
			}else {
				if (Vector3.Distance (transform.position, target.transform.position) < 5) {
					pathfinder.enabled = true;
					target = GameObject.FindGameObjectWithTag ("Player").transform;
					playerDetected = true;
					targetPosition = new Vector3 (target.position.x, 0, target.position.z);
				} else {
					Debug.Log ("Player not detected.");
				}



			}
			yield return new WaitForSeconds (refreshRate);


		}
	}
}
