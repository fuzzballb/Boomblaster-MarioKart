using UnityEngine;
using System.Collections;

public class PickupSpawn : MonoBehaviour {

	public float spawnTime;
	private Vector3 _spawnPosition;
	private Pickup _pickup;
	public PhotonView photon;

	// Use this for initialization
	void OnJoinedRoom(){
		photon = GetComponent<PhotonView> ();
		_spawnPosition = transform.position;
		SpawnPickup ();
	}
	private void SpawnPickup(){
		_pickup = PhotonNetwork.Instantiate ("Pickup",transform.position,transform.rotation,0).GetComponent<Pickup> ();
		_pickup.spawnPoint = this;
		//_pickup.GetComponent<Pickup> ();
	}
	[PunRPC]
	public  void Deactivate(){
		PhotonNetwork.Destroy(_pickup.gameObject);
		StartCoroutine (Activate (spawnTime));
	}
	IEnumerator Activate(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		SpawnPickup ();
	}
}
