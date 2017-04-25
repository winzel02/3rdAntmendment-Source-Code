using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

	void Awake(){
		transform.position = new Vector3 (0f, -2.7f, 0f);
		foreach (Transform waypoints in transform) {
			waypoints.position = new Vector3 (Random.Range (-65, 64), 3.83f, Random.Range (-57, 45));
		}
	}
	void Start(){
		transform.SetParent (GameObject.FindGameObjectWithTag ("GameManager").transform);
	}
	void OnDrawGizmos(){
		Vector3 startWaypoint = transform.GetChild (0).position;
		Vector3 prevWaypoint = startWaypoint;

		foreach (Transform waypoints in transform) {
			Gizmos.DrawSphere (waypoints.position, 1f);
			Gizmos.DrawLine (prevWaypoint, waypoints.position);
			prevWaypoint = waypoints.position;
		}
		Gizmos.DrawLine (prevWaypoint, startWaypoint);
	}
}
