using UnityEngine;
using System.Collections;

public class Ant : MonoBehaviour
{
    [Header("Attributes")]
    public float speed = 5f;

    [Space]
    public GameObject crumbPrefab;
    public Transform[] crumbSockets;
	public float sensitivity;

	Transform respawnPos;
    private bool frontOccupied;
    private bool backOccupied;

    private Animator animator;
	Vector3 movement;
	public float intSpeed;
	TrailRenderer trail;

	AudioSource source;
	public AudioClip pickup,dead,pickdown;
	void Awake(){
		respawnPos = GameObject.FindGameObjectWithTag ("GameManager").transform.GetChild (0);
		trail = gameObject.GetComponent<TrailRenderer> ();
		source = GetComponent<AudioSource> ();
	}
    void Start()
    {
		intSpeed = speed;
        animator = GetComponent<Animator>();
    }
	void FixedUpdate ()
    {
		if (!GameManager.Instance.isDead && !GameManager.Instance.timeOver) {
			//Position
			float h = Input.GetAxisRaw ("Horizontal");
			float v = Input.GetAxisRaw ("Vertical");

			movement = new Vector3 (h, 0, v);

			transform.Translate (movement * Time.fixedDeltaTime * speed);
		}
		if (!GameManager.Instance.isDead && !GameManager.Instance.timeOver) {
			//Rotation
			float mouseMoveH = Input.GetAxis ("Mouse X");

			transform.Rotate (new Vector3 ( 0f, mouseMoveH * Time.fixedDeltaTime * sensitivity, 0f));

		}	

		if(movement.magnitude > 0)
			animator.SetBool("Moving", true);
		else
			animator.SetBool("Moving", false);

		if (GameManager.Instance.timeOver)
			StopAllCoroutines ();
		
	}

	public void AddSpeedBonus(){
		speed = intSpeed;
		StopCoroutine (AddSpeed ());
		StartCoroutine (AddSpeed ());
	}
	IEnumerator AddSpeed(){
		speed += GameManager.Instance.moveSpeedBonus;
		trail.enabled = true;
		yield return new WaitForSeconds (5f);
		speed = intSpeed;
		trail.enabled = false;
	}
	void Died(){
		GameManager.Instance.isDead = true;
		source.PlayOneShot (dead);
		Physics.IgnoreLayerCollision (8, 10, true);
		StartCoroutine (Respawn ());
	}
	IEnumerator Respawn(){
		StartCoroutine (UserInterface.Instance.DecrementRespawnTimer ());
		yield return new WaitForSeconds (GameManager.Instance.respawnTime);
		Physics.IgnoreLayerCollision (8, 10, false);
		if (frontOccupied && !backOccupied) {
			Destroy( crumbSockets [0].GetChild (0).gameObject);
		}
		else if(frontOccupied && backOccupied){
			Destroy( crumbSockets [0].GetChild (0).gameObject);
			Destroy( crumbSockets [1].GetChild (0).gameObject);
		}
		frontOccupied = false;
		backOccupied = false;
		transform.position = respawnPos.position; //REspawn position
		GameManager.Instance.isDead = false;
	}
	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "Spider") {
			if (!GameManager.Instance.invulnerable) {
				if (!GameManager.Instance.isDead)
					Died ();
			}
		}
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Crumb"))
        {
			

            if (!frontOccupied && !backOccupied)
            {
				source.PlayOneShot (pickup);
                frontOccupied = true;
				other.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
                other.transform.position = crumbSockets[0].position;
                other.transform.SetParent(crumbSockets[0]);
            }
            else if(frontOccupied && !backOccupied)
            {
				source.PlayOneShot (pickup);
				other.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
                backOccupied = true;
                other.transform.position = crumbSockets[1].position;
                other.transform.SetParent(crumbSockets[1]);
            }
            else if (frontOccupied && backOccupied)
            {
                Debug.Log("Cannot pick up more Crumbs!");
            }
        }

		if (other.gameObject.tag == "CrumbHolder") {
			if (frontOccupied && !backOccupied) {
				source.PlayOneShot (pickdown);
				CrumbScript crumbScript = crumbSockets[0].GetChild(0).gameObject.GetComponent<CrumbScript> ();
				GameManager.Instance.AddScore(crumbScript.scoreToAdd);
				GameManager.Instance.spawns.Remove (crumbSockets[0].GetChild(0).gameObject);
				Destroy( crumbSockets [0].GetChild (0).gameObject);
				frontOccupied = false;
			}
			if (frontOccupied && backOccupied) {
				source.PlayOneShot (pickdown);
				CrumbScript[] crumbScript = new CrumbScript[2];
				crumbScript[0] = crumbSockets[0].GetChild(0).gameObject.GetComponent<CrumbScript> ();
				crumbScript[1] = crumbSockets[1].GetChild(0).gameObject.GetComponent<CrumbScript> ();
				float scoreAdd = crumbScript [0].scoreToAdd + crumbScript [1].scoreToAdd;
				GameManager.Instance.AddScore(scoreAdd);
				GameManager.Instance.spawns.Remove (crumbSockets[0].GetChild(0).gameObject);
				GameManager.Instance.spawns.Remove (crumbSockets[1].GetChild(0).gameObject);
				Destroy( crumbSockets [0].GetChild (0).gameObject);
				Destroy( crumbSockets [1].GetChild (0).gameObject);
				frontOccupied = false;
				backOccupied = false;
			}
		}
    }
}