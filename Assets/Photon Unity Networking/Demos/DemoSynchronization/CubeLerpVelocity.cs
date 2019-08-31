using UnityEngine;

public class CubeLerpVelocity : Photon.MonoBehaviour
{
    //private Vector3 correctPlayerPos;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            syncTime += Time.deltaTime;
            transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(GetComponent<Rigidbody>().velocity);
            stream.SendNext(transform.rotation);
        }
        else
        {
            Vector3 syncPosition = (Vector3)stream.ReceiveNext();
            Vector3 syncVelocity = (Vector3)stream.ReceiveNext();

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = transform.position;
        }


        //if (stream.isWriting)
        //{
        //    // We own this player: send the others our data
        //    stream.SendNext(transform.position);
        //    stream.SendNext(transform.rotation);

        //}
        //else
        //{
        //    // Network player, receive data
        //    this.correctPlayerPos = (Vector3)stream.ReceiveNext();
        //    this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
        //}
    }
}