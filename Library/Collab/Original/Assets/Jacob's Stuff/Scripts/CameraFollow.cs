using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform player;

	public float shakeAmt;
	Vector3 cameraPos;
	void Awake(){
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		cameraPos = Camera.main.transform.localPosition;
	}
	void Update()
	{
		Collider[] hitCols = Physics.OverlapSphere (player.transform.position, 15f, 1 << LayerMask.NameToLayer("Human"));
		if (hitCols.Length > 0)
			Shake ();
	}
	void Shake(){
		InvokeRepeating ("BeginShake", 0, 0.01f);
		Invoke ("StopShake", 0.2f);
	}

	void BeginShake(){
		if (shakeAmt > 0) {
			Vector3 camPos = Camera.main.transform.position;

			float offsetX = Random.value * shakeAmt * 2 - shakeAmt;
			float offsetY = Random.value * shakeAmt * 2 - shakeAmt;

			camPos.x += offsetX;
			camPos.y += offsetY;

			Camera.main.transform.position = camPos;
		}
	}

	void StopShake(){
		CancelInvoke ("BeginShake");
		Camera.main.transform.localPosition = cameraPos;
	}
}