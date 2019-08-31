using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

	public PickupSpawn spawnPoint;

	void OnTriggerEnter(Collider other) {
		Debug.Log ("hit");
		var engine = other.gameObject.GetComponent<Engine> ();
		if (engine != null) {
			engine.PickUp ();
			spawnPoint.photon.RPC("Deactivate", PhotonTargets.All);
			PhotonNetwork.Destroy (this.gameObject);
		}
	}
}
