using UnityEngine;
using System.Collections;

public class WallBumper : UnityEngine.MonoBehaviour
{
    private Vector3 _revertDirection;
    public int speedReflectionVector = 50;
    Rigidbody rigid;

    void Start()
    {

        rigid = GetComponent<Rigidbody>();
    }

    /***********************************************
    * name : OnCollisionEnter
    * return type : void
    * Script to make kart bounce off walls 
    * ********************************************/
    void OnCollisionEnter(Collision e)
    {
        ContactPoint cp = e.contacts[0];

        // get the Normal of the wall
        _revertDirection = cp.normal;

        // Get the angle between wall and player normal
        var angle = Vector3.Angle(rigid.velocity, _revertDirection);
        //Debug.LogError ("angle " + angle);

        // Head on collision should have more bounce then sideways
        // angle is between 90 and 180
        float angleBounceBonus = 0.0f;
        if (angle > 90.0f)
        {
            angleBounceBonus = (angle - 90) * 5;
        }
        //Debug.LogError ("angleBounceBonus " + angleBounceBonus);

        // Higher speeds should have more bounce
        // speed is between 4 and 20
        float speed = rigid.velocity.magnitude;
        Debug.LogError("speed " + rigid.velocity.magnitude);

        // Bounce of wall amount
        Vector3 launchSpeed = _revertDirection.normalized * ((speedReflectionVector + (int)angleBounceBonus) / 10) * speed;

        // remove vertical bounce
        launchSpeed.y = 0;

        // apply force to rigidbody
        var pos = transform.position;
        rigid.AddForceAtPosition(launchSpeed, pos, ForceMode.Acceleration);

        Debug.DrawLine(transform.position, launchSpeed, Color.cyan);
        //Debug.LogError ("rigid.velocity = " + rigid.velocity + "  launcespeed = " + launchSpeed);

        //_revertDirection = Vector3.Reflect(e.rigidbody.velocity, cp.normal * -1);
        //e.rigidbody.velocity = (_revertDirection.normalized * speedReflectionVector);
    }
}