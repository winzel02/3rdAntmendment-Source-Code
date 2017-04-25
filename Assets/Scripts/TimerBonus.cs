using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBonus : MonoBehaviour {

	public float rotateSpeed = 5f;
	float movePosY;
	Transform child;


	void Awake(){
		movePosY = transform.position.y + 1.7f;
		child = transform.GetChild (0);

	}
	void Update(){
		child.transform.position =  new Vector3 (transform.position.x, movePosY + Mathf.PingPong (Time.time * 2, 3), transform.position.z);
		child.transform.Rotate(new Vector3(0f, rotateSpeed * Time.deltaTime, 0f));
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player") {
			GameManager.Instance.PlayBonusClip ();
			GameManager.Instance.vAnimate ();
			GameManager.Instance.AddCountdownTimer ();
			Destroy (this.gameObject);
		}
	}

}
