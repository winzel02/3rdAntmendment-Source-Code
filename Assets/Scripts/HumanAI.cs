using System.Collections;
using UnityEngine;

public class HumanAI : MonoBehaviour {

	public float moveSpeed;			//Move Speed
	public float turnSpeed;			//Turn Speed
	public float waitTime = 1f;		//Time to wait every time he proceeds to go to another waypoint.
	public GameObject waypointHolder;

	public Transform[] legs;
	Transform leftDmg, rightDmg;
	int[] waveDifficulty = { 5, 8, 13, 17, 20, 25,27,30,34,39,40,45,49,51 };
	int currentDifficulty = 0;
	void Awake(){
		Transform legTr = transform.FindChild ("Legs");
		legs = new Transform[legTr.childCount];

		for (int i = 0; i < legTr.childCount; i++) {
			legs [i] = legTr.GetChild (i);
		}
		leftDmg = legs [0].GetChild (0);
		rightDmg = legs [1].GetChild (0); 

	}
	void Start(){
		waypointHolder = Instantiate (waypointHolder, GameObject.Find("Environment").transform);
		Vector3[] waypoints = new Vector3[waypointHolder.transform.childCount];
		for (int i = 0; i < waypoints.Length; i++) {
			waypoints [i] = waypointHolder.transform.GetChild (i).position;
			waypoints [i] = new Vector3 (waypoints [i].x, transform.position.y	, waypoints [i].z);
		}
		StartCoroutine (WalkPath (waypoints));

	}
	void Update(){
		if (GameManager.Instance.currentLevel >= waveDifficulty [currentDifficulty] && GameManager.Instance.currentLevel < waveDifficulty [currentDifficulty + 1]) {
			moveSpeed += 3;
			currentDifficulty += 1;
		}
		RaycastHit hit;
		Ray leftRay = new Ray (leftDmg.position, Vector3.down);
		Ray rightRay = new Ray (rightDmg.position, Vector3.down);
		if (Physics.SphereCast (leftRay, 2f, out hit, 1f, 1 << LayerMask.NameToLayer ("Player"))) {
			if (hit.collider != null) {
				if (!GameManager.Instance.invulnerable) {
					if (!GameManager.Instance.isDead)
						hit.collider.SendMessage ("Died", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		if (Physics.SphereCast (rightRay, 2f, out hit, 1f, 1 << LayerMask.NameToLayer ("Player"))) {
			if (hit.collider != null) {
				if (!GameManager.Instance.invulnerable) {
					if (!GameManager.Instance.isDead)
						hit.collider.SendMessage ("Died", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

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
