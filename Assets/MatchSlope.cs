using UnityEngine;
using System.Collections;

public class MatchSlope : MonoBehaviour {

    RaycastHit centerHit = new RaycastHit();
    public float speed = 1000.1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        Ray centerRay = new Ray(transform.position, -(this.transform.up) * 10);

	    Debug.DrawRay(transform.position, -(this.transform.up) * 10, Color.black);


        if (Physics.Raycast(centerRay, out centerHit, 10.0f))
        {
            Debug.DrawRay(centerRay.origin, centerHit.normal * 10, Color.cyan);

            var targetRotation = Quaternion.FromToRotation(transform.up, centerHit.normal) * transform.rotation;
            transform.rotation = targetRotation; //Quaternion.Lerp(transform.rotation, targetRotation, speed);

            //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.time * speed);
        }
	}
}
