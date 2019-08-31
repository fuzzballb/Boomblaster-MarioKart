using UnityEngine;
using System.Collections;

public class CTF_Flag : MonoBehaviour {

	public CTF_FlagPickup spawnPoint;

	void OnTriggerEnter(Collider other) {
		Debug.Log ("hit");
		var engine = other.gameObject.GetComponent<Engine> ();
		if (engine != null && !engine.isInvulnerable) {
			engine.PickUpFlag (spawnPoint);
			PhotonNetwork.Destroy (this.gameObject);
		}
	}
}
