using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {
	public float rotationSpeed = 10f;
	private Rigidbody _rigid;
	private Ray _centerRay = new Ray();
	private RaycastHit _centerHit = new RaycastHit();
	// Use this for initialization
	void Start () {
		_rigid = GetComponent<Rigidbody> ();
		_rigid.AddForce (transform.forward * 30,ForceMode.Impulse);
		StartCoroutine (Die (10f));
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawRay (transform.position, transform.forward * 10);
		_centerRay = new Ray(transform.position, -(this.transform.up));
		if (Physics.Raycast (_centerRay, out _centerHit, 1f)) {
			var t = transform.position;
			t.y = _centerHit.point.y;
			transform.position = t;
		}
		_rigid.transform.Rotate (0, rotationSpeed, 0);
		//Debug.DrawRay (transform.position, transform.forward * 10);
	}
	void OnCollisionEnter(Collision collision) {
		var engine = collision.collider.gameObject.GetComponent<Engine> ();
		if (engine != null && !engine.isInvulnerable) {
			engine.photon.RPC("GotHit", PhotonTargets.All);
			PhotonNetwork.Destroy (this.gameObject);
		} else {
			//particleeffect
		}
	}
	IEnumerator Die(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		PhotonNetwork.Destroy (this.gameObject);
	}
}
