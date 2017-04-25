using UnityEngine;

public class Ant : MonoBehaviour
{
    [Header("Attributes")]
    public float speed = 5f;

    [Space]
    public GameObject crumbPrefab;
    public Transform[] crumbSockets;
	public float sensitivity;

    private bool frontOccupied;
    private bool backOccupied;

    private Animator animator;
	Vector3 movement;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
	void Update()
	{
		if (!GameManager.Instance.isDead) {
			//Rotation
			float mouseMoveH = Input.GetAxis ("Mouse X");
			transform.Rotate (new Vector3 (0f, mouseMoveH * Time.fixedDeltaTime * sensitivity, 0f));
		}	
	}
	void FixedUpdate ()
    {
		if (!GameManager.Instance.isDead) {
			//Position
			float h = Input.GetAxisRaw ("Horizontal");
			float v = Input.GetAxisRaw ("Vertical");

			movement = new Vector3 (h, 0, v);

			transform.Translate (movement * Time.fixedDeltaTime * speed);
		}

		if(movement.magnitude > 0)
			animator.SetBool("Moving", true);
		else
			animator.SetBool("Moving", false);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Crumb"))
        {
            if (!frontOccupied && !backOccupied)
            {
                frontOccupied = true;
                other.transform.position = crumbSockets[0].position;
                other.transform.SetParent(crumbSockets[0]);
            }
            else if(frontOccupied && !backOccupied)
            {
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
				GameManager.Instance.crumbs += 1;
				Destroy( crumbSockets [0].GetChild (0).gameObject);
				frontOccupied = false;
			}
			if (frontOccupied && backOccupied) {
				GameManager.Instance.crumbs += 2;
				Destroy( crumbSockets [0].GetChild (0).gameObject);
				Destroy( crumbSockets [1].GetChild (0).gameObject);
				frontOccupied = false;
				backOccupied = false;
			}
		}
    }
}