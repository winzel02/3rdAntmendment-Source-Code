using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedBonus : MonoBehaviour {

	public float rotateSpeed = 5f;
	float movePosY;
	bool acquired;
	Transform child;
	void Awake(){
		movePosY = transform.position.y + 1.7f;
		child = transform.GetChild (0);
	}
	void Update(){
		if (!acquired) {
			child.transform.position = new Vector3 (transform.position.x, movePosY + Mathf.PingPong (Time.time * 2, 3), transform.position.z);
			child.transform.Rotate (new Vector3 (0f , 0f,rotateSpeed * Time.deltaTime));
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" && !acquired) {
			acquired = true;
			GameManager.Instance.PlayBonusClip ();
			other.gameObject.GetComponent<Ant> ().AddSpeedBonus ();
			Destroy (transform.GetChild (0).gameObject);
			Destroy (this.gameObject, 6f);
		}
	}
}
