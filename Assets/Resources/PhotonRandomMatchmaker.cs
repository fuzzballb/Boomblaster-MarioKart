using UnityEngine;
using Photon;
using System;

public class PhotonRandomMatchmaker : Photon.PunBehaviour
{
	// Use this for initialization
	void Start()
	{
        // PhotonNetwork.sendRate = 200;
        //  PhotonNetwork.sendRateOnSerialize = 200;

        PhotonNetwork.ConnectUsingSettings("v4.2");
	}



    private string roomName = "roomName";
    private RoomInfo[] roomsList;
    private PhotonView myPhotonView;
    private MiniMap minimap;

 
    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        else if (PhotonNetwork.room == null)
        {
            // Create Room

            roomName = GUI.TextField(new Rect(100, 70, 250, 20), roomName, 25);


            if (GUI.Button(new Rect(100, 100, 250, 100), "Create Room"))
                PhotonNetwork.CreateRoom(roomName);
 
            // Join Room
            if (roomsList != null)
            {
                for (int i = 0; i < roomsList.Length; i++)
                {
                    if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join " + roomsList[i].name))
                        PhotonNetwork.JoinRoom(roomsList[i].name);
                }
            }
        }
    }
 
    void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }

    //public override void OnJoinedLobby()
    //{
    //    PhotonNetwork.JoinRandomRoom();
    //}

	void OnPhotonRandomJoinFailed()
	{
		Debug.Log("Can't join random room!");
		PhotonNetwork.CreateRoom(null);
	}

	void OnJoinedRoom()
	{

		// Instantiate KartVR for google cardboard VR 
		// !important, also disable the main camera when usign this
		//GameObject monster = PhotonNetwork.Instantiate("KartVR", new Vector3(0.0f,10.0f,0.0f) , Quaternion.identity, 0);

		GameObject monster = PhotonNetwork.Instantiate("Cube 1", new Vector3(0.0f,10.0f,0.0f) , Quaternion.identity, 0);

        // enable your controler
		Engine controller = monster.GetComponent<Engine>();
        Rigidbody rigidBody = monster.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        controller.enabled = true;

        // add yourself to the target of the main camera
		var camera2 = Camera.main;
		SmoothCameraAdvanced sf = camera2.GetComponent<SmoothCameraAdvanced>();
		if (sf)
			sf.target = monster.transform;

        // add your self to your minimap
        minimap = this.GetComponent<MiniMap>();
        minimap.player = monster.transform;

        // add your self as enemy to minimap of other players
        myPhotonView = monster.GetComponent<PhotonView>();
        myPhotonView.RPC("UpdateEnemiesMinimap", PhotonTargets.Others);
	}

    void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        myPhotonView.RPC("UpdateEnemiesMinimap", PhotonTargets.Others);
    }


}