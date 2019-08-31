using UnityEngine;
using System.Collections;

public class CTF_FlagDropOff : MonoBehaviour {
	public PhotonView photon;

	// Use this for initialization
	void OnJoinedRoom(){
		photon = GetComponent<PhotonView> ();
	}
	void OnTriggerEnter(Collider other) {
		//check if correct team
		var engine = other.gameObject.GetComponent<Engine> ();
		if (engine != null && engine.CTF_GotFlag) {
			Debug.Log ("Dropoff");
			engine.DeliverFlag ();
		}
	}
}
