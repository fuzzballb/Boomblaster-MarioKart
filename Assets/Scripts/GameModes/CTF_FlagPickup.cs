using UnityEngine;
using System.Collections;

public class CTF_FlagPickup : MonoBehaviour {

	public float spawnTime;
	public Transform spawnPosition;
	//private CTF_Flag _pickup;
	public PhotonView photon;

	// Use this for initialization
	void OnJoinedRoom(){
		photon = GetComponent<PhotonView> ();
		SpawnPickup ();
	}
	[PunRPC]
	public void SpawnPickup(){
		var t = PhotonNetwork.Instantiate ("CTF_Flag",spawnPosition.position,spawnPosition.rotation,0).GetComponent<CTF_Flag> ();
		t.spawnPoint = this;
	}
	IEnumerator Activate(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		SpawnPickup ();
	}
}
