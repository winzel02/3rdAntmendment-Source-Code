using System.Collections;
using UnityEngine;

public class SpiderAI : MonoBehaviour {

	public float moveSpeed;			//Move Speed
	public float turnSpeed;			//Turn Speed
	public float waitTime = 1f;		//Time to wait every time he proceeds to go to another waypoint.
	public GameObject waypointHolder;

	bool playerDetected;

	int[] waveDifficulty = { 3, 6, 9, 12, 15, 18, 21,24,25,30,34,37,39,45,50 };
	int currentDifficulty = 0;
	void Awake(){ 
		
	}

	void Start(){
		waypointHolder = Instantiate (waypointHolder, GameObject.Find("Environment").transform);
		Vector3[] waypoints = new Vector3[waypointHolder.transform.childCount];
		for (int i = 0; i < waypoints.Length; i++) {
			waypoints [i] = waypointHolder.transform.GetChild (i).position;
			waypoints [i] = new Vector3 (waypoints [i].x, transform.position.y, waypoints [i].z);
		}
		StartCoroutine (WalkPath (waypoints));
	}

	void Update(){
		if (GameManager.Instance.currentLevel >= waveDifficulty [currentDifficulty] && GameManager.Instance.currentLevel < waveDifficulty [currentDifficulty + 1]) {
			moveSpeed += 3;
			currentDifficulty += 1;
		}
			
		Collider[] hit = Physics.OverlapSphere (transform.FindChild ("DmgArea").position, 2f, 1 << LayerMask.NameToLayer ("Player"));
		if (hit.Length > 0) {
			if (!GameManager.Instance.invulnerable) {
				if (!GameManager.Instance.isDead)
					hit[0].SendMessage ("Died", SendMessageOptions.DontRequireReceiver);
			}
		}


	}
	/*void PlayerDetected(GameObject player){
		Vector3 displacement = player.transform.position - transform.position;
		Vector3 dir = displacement.normalized;

		float distanceToTarget = displacement.magnitude;
		if (distanceToTarget > 1f) {
			transform.Translate (dir * 2 * Time.deltaTime);
		}

	}*/
	IEnumerator WalkPath(Vector3[] waypoints){
		transform.position = waypoints [0];

		int targetWaypoint = 1;
		Vector3 targetPos = waypoints [targetWaypoint];
		transform.LookAt (targetPos);

		while (true) {

			transform.position = Vector3.MoveTowards (transform.position, targetPos, moveSpeed * Time.deltaTime);
			if (transform.position == targetPos) {

				targetWaypoint = (targetWaypoint + 1) % waypoints.Length;
				targetPos = waypoints [targetWaypoint];
				yield return new WaitForSeconds (waitTime);
				yield return StartCoroutine (TurnFace (targetPos));
			}

			yield return null;
		}
	}
	IEnumerator TurnFace(Vector3 lookTarget){
		Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
		float targetAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
		while (Mathf.Abs(Mathf.DeltaAngle (transform.eulerAngles.y, targetAngle)) > 0.05f) {
			float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
			transform.eulerAngles = Vector3.up * angle;
			yield return null;
		}
	}
}
