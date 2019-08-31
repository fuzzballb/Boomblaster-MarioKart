using UnityEngine;
using System.Collections;
using Photon;
using InControl;

public class Engine : UnityEngine.MonoBehaviour {
	public GameObject[]  frontWheels = new GameObject[2];

	public GameObject[]  P_frontWheels = new GameObject[2];

	public GameObject[]  rearWheels = new GameObject[2];

   // public GameObject mario = new GameObject();

    Ray[] rays = new Ray[4];

    // AB: wall bounce
    Ray[] forwardRays = new Ray[2];

    RaycastHit[] hits = new RaycastHit[4];

    // AB: wall bounce
    RaycastHit[] forwardHits = new RaycastHit[2];

    RaycastHit centerHit = new RaycastHit();
    Ray centerRay = new Ray();
    Rigidbody rigid;

	private Renderer[] _renderer;
    float gravity;
    float weight;


    [Header("Speed")]
    public float accelaration = 10;
    public float maxSpeed;
    public float maxTurnSpeed = 25.0f;
    public float speedFallOffFactor = 0.8f;

    [Header("Traction")]
    public float traction = 1.5f;
    public float tractionNegativeMax = -1.5f;
    public float _TurnSpeed;

    private float _speed;
    private float _torque;
    private GameObject[] suspensionToWheels = new GameObject[4];

    [Header("Drift")]
    public float maxTurnSpeedDrift = 200.0f;
    public float initialNegativeTraction = -1.5f;

    [Header("Suspension")]
    public float maxSuspensionForce;
    public float forwardForceOffset = 1.1f;
    public float suspensionHeight = 0.5f;

    [Header("Particle System")]
    public ParticleSystem gasParticleSystem;
	public ParticleSystem backRightParticleSystem;

    [Header("concussion")]
    private float _hitTime;
    public float concussionTime;
	public bool isInvulnerable = false;

    [Header("Shell")]
    public Shell shell;
	public Transform shellSpawn;

    private Vector3[] storedLocalPositions = new Vector3[4];

	private float _wheelHeight;

    private Vector3 launchVector;
    private Vector3 launchSpeed;
    Vector3 size;

    //TODO: Should be type of weapon
    
    private int _shots;

    [Header("Multiplayer")]
    public PhotonView photon;
    public bool CTF_GotFlag;
	public GameObject CTF_Flag;
	public int CTF_Score;
	public CTF_FlagPickup CTF_lastPickupPoint;

    [Header("Virtual Raelity")]
    public bool useVRcontrols = false;
	private float gas = 0;
	private float right = 0;
    private bool drift = false;
     

    void Start()
	{
		photon = GetComponent<PhotonView>();
		rigid = GetComponent<Rigidbody>();
		_renderer = GetComponentsInChildren<Renderer> ();//ieuwl
			
        weight = rigid.mass;
        gravity = Physics.gravity.y;
        UpdateRays();
        var t = rigid.centerOfMass;
        t.y -= 1f;
        rigid.centerOfMass = t;
        size = GetComponent<Collider>().bounds.size;

        for (int i = 0; i < rays.Length; i++)
        {
            hits[i] = new RaycastHit();
		}

        // AB: wall bounce
        for (int i = 0; i < forwardRays.Length; i++)
        {
            forwardHits[i] = new RaycastHit();
        }


        suspensionToWheels [0] = rearWheels [0];
		suspensionToWheels [1] = frontWheels [0];
		suspensionToWheels [2] = frontWheels [1];
		suspensionToWheels [3] = rearWheels [1];

		_hitTime = -concussionTime;
      
        storedLocalPositions[0]= rearWheels[0].transform.localPosition;
        storedLocalPositions[1]= frontWheels[0].transform.localPosition;
        storedLocalPositions[2]= frontWheels[1].transform.localPosition;
        storedLocalPositions[3]= rearWheels[1].transform.localPosition;

		_wheelHeight = rearWheels [1].GetComponent<Renderer> ().bounds.size.y;
    }
	void Update(){
		//TODO: networking
		if (Input.GetKeyDown (KeyCode.Space) && _shots > 0) {
			var sPos = shellSpawn.position;
			PhotonNetwork.Instantiate ("Shell",sPos,shellSpawn.rotation,0);
			_shots--;
		}
		if (Input.GetKeyDown (KeyCode.LeftControl) && CTF_GotFlag) {
			photon.RPC("DropFlag", PhotonTargets.All);
		}

		// Input Keys will get reset if in the fixed update 

		//var InputDevice = InputManager.ActiveDevice;
		//InputDevice.DPadUp.IsPressed
		/*
		Debug.Log ("InputDevice.DPadUp " + InputDevice.DPadUp);
		Debug.Log ("InputDevice.DPadY" + InputDevice.DPadY);
		Debug.Log ("InputDevice.LeftStickY " + InputDevice.LeftStickY);
		Debug.Log ("InputDevice.AnyButton) " + InputDevice.AnyButton);
		Debug.Log ("InputDevice.RightStickY " + InputDevice.RightStickY);
		Debug.Log ("InputDevice.RightStick " + InputDevice.RightStick);
		*/
		if (useVRcontrols) {	
			if (Input.GetKeyDown (KeyCode.JoystickButton4)) {
				gas = 0.5f;
	//			Debug.Log ("Input.GetKeyDown KeyCode.JoystickButton4");
			}

			if (Input.GetKeyDown (KeyCode.JoystickButton6)) {
				gas = -0.5f;
	//			Debug.Log ("Input.GetKeyDown KeyCode.JoystickButton6");
			}

			if (Input.GetKeyDown (KeyCode.JoystickButton12)) {
				gas = 0;
	//			Debug.Log ("Input.GetKeyUp KeyCode.JoystickButton12 gas 0");
			}

			if (Input.GetKeyDown (KeyCode.JoystickButton13)) {
	//			Debug.Log ("Input.GetKeyUp KeyCode.JoystickButton13");
			}


			if (Input.GetKey (KeyCode.JoystickButton5)) {
				right = 0.6f;
			}

			if (Input.GetKey (KeyCode.JoystickButton7)) {
				right = -0.6f;
			}

			if (Input.GetKeyDown (KeyCode.JoystickButton15)) {
				right = 0;
	//			Debug.Log ("Input.GetKeyUp KeyCode.JoystickButton15 right 0");
			}

			if (Input.GetKeyDown (KeyCode.JoystickButton14)) {
	//			Debug.Log ("Input.GetKeyUp KeyCode.JoystickButton14");
			}
		} else {
			gas = Input.GetAxis ("Vertical");
			right = Input.GetAxis ("Horizontal");
		}

        if(Input.GetKey(KeyCode.LeftShift))
        {
            drift = true;
            Debug.Log("drift");
        }
        else
        {
            drift = false;
            Debug.Log("NO drift");
        }

    }
    void FixedUpdate()
    {
		UpdateRays();

		var gEm = gasParticleSystem.emission;
		AnimationCurve curve = new AnimationCurve();
		curve.AddKey(0.0f, 0.1f);
		curve.AddKey(0.75f, 1.0f);
		gEm.rate = new ParticleSystem.MinMaxCurve(40.0f * gas, curve);

		#region Physics
        //suspension
        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], out hits[i], suspensionHeight))
            {
				var p = suspensionToWheels[i].transform.localPosition;
                p.y = _wheelHeight / 2 - Mathf.Clamp(hits[i].distance, 0, 0.2f); 
				suspensionToWheels[i].transform.localPosition = p;


			//	Debug.DrawRay(rays[i].origin, rays[i].direction);
                Vector3 forceToSuspension = ((suspensionHeight / hits[i].distance) * hits[i].normal);
                rigid.AddForceAtPosition(Vector3.ClampMagnitude(forceToSuspension, maxSuspensionForce), rays[i].origin);
            }
        }

        // bumper
        // AB: wall bounce
        // TODO: cleanup code "maxSuspensionForwardForce" should always be 0 to make this work
        for (int i = 0; i < forwardRays.Length; i++)
        {
            Debug.DrawRay(forwardRays[i].origin, forwardRays[i].direction, Color.cyan);

            if (Physics.Raycast(forwardRays[i], out forwardHits[i], 2.0f))
            {                
                Vector3 forceToSuspension = ((2.0f / forwardHits[i].distance) * forwardHits[i].normal);
                rigid.AddForceAtPosition(Vector3.ClampMagnitude(forceToSuspension, 0), forwardRays[i].origin);
            }
        }

        var wheelRotationSpeed = transform.InverseTransformDirection(rigid.velocity).z * 2.2f;

		frontWheels [0].transform.Rotate (new Vector3 (wheelRotationSpeed, 0, 0));
		frontWheels [1].transform.Rotate (new Vector3 (wheelRotationSpeed, 0, 0));

		rearWheels [0].transform.Rotate (new Vector3 (0, 0,wheelRotationSpeed));
		rearWheels [1].transform.Rotate (new Vector3 (0, 0,wheelRotationSpeed));



        //forward
		if (Physics.Raycast(centerRay,out centerHit, 1f) && Time.time > _hitTime + concussionTime ) {
			isInvulnerable = false;
			_speed = gas * maxSpeed;
            var pos = transform.position;
            //TODO: maybe move to center if speed = maxSpeed, since we're not accelarating at that point
            pos.y -= forwardForceOffset;
            var t = (transform.forward - (Vector3.Dot(transform.forward, centerHit.normal)) * centerHit.normal);
            //Debug.DrawLine(transform.position, t * 10, Color.black);
            Debug.DrawRay(transform.position, t * _speed, Color.red);
            rigid.AddForceAtPosition(t * _speed, pos, ForceMode.Force);

            launchVector = pos;
            launchSpeed = t * _speed;











            // AB: add drift 
            var tractionAmmount = traction;

            //         Debug.Log("Drift " + drift);
            if (drift)
            {
                tractionAmmount = initialNegativeTraction;

               
                if (initialNegativeTraction < traction)
                {
                    initialNegativeTraction += 2.0f * Time.deltaTime;
                }
               

                if (_TurnSpeed < maxTurnSpeedDrift)
                {
                    _TurnSpeed += 10.0f * Time.deltaTime;
                }

                var driftWheelRotationSpeed = 30.0f;

                rearWheels[0].transform.Rotate(new Vector3(0, 0, driftWheelRotationSpeed));
                rearWheels[1].transform.Rotate(new Vector3(0, 0, driftWheelRotationSpeed));
            }
            else
            {
                initialNegativeTraction = tractionNegativeMax;
                tractionAmmount = traction;
                _TurnSpeed = maxTurnSpeed;
            }

            //torque, only steer when landed
            //TODO: get type of ground to influence turnspeed, feels like ice atm
            _torque = right * _TurnSpeed * (transform.InverseTransformDirection(rigid.velocity).normalized.z);
            rigid.AddRelativeTorque(0, _torque, 0);



            // compensation for default sliding
            //       Debug.Log("tractionAmmount " + tractionAmmount);
            var negativeXVelocity = -transform.InverseTransformDirection(rigid.velocity).x * tractionAmmount;
            var p = transform.position;
            p.y -= forwardForceOffset;
            rigid.AddForceAtPosition(negativeXVelocity * transform.right, p, ForceMode.Force);
          




            Debug.DrawRay(transform.position, rigid.velocity.normalized * 10, Color.black);

			var stearAngle = _torque;
            
            P_frontWheels [0].transform.localEulerAngles = new Vector3(0,30f * right ,0);  
			P_frontWheels [1].transform.localEulerAngles = new Vector3(0,30f * right ,0);
        }
        else
        {
            var pos = transform.position;
            //pos.y -= 0.05f;
            rigid.AddForceAtPosition(launchSpeed, pos, ForceMode.Acceleration);
            //TODO: fix arbitrary numbers
            if (launchSpeed.y > -15.81f) { 
            launchSpeed.y -= 3.5f;
            }
            launchSpeed *= speedFallOffFactor;
        }
		#endregion
        
    }
    void UpdateRays()
    {
		//rearwheels 0
        //leftback
        rays[0] = new Ray(transform.position - (transform.right * size.x / 2) - (transform.up * size.y / 2) - (transform.forward * size.z / 2), -(transform.up));
		//frontwheels 0
        //leftfront
        rays[1] = new Ray(transform.position - (transform.right * size.x / 2) - (transform.up * size.y / 2) + (transform.forward * size.z / 2), -(transform.up));
		//frontWheels 1
        //rightfront
        rays[2] = new Ray(transform.position + (transform.right * size.x / 2) - (transform.up * size.y / 2) + (transform.forward * size.z / 2), -(transform.up));
		//rearWheels 1
        //rightback
        rays[3] = new Ray(transform.position + (transform.right * size.x / 2) - (transform.up * size.y / 2) - (transform.forward * size.z / 2), -(transform.up));

        //center
        centerRay = new Ray(transform.position - (transform.up * size.y / 2), -(this.transform.up));



        // AB: wall bounce
        //leftfront
        forwardRays[0] = new Ray(transform.position - (transform.right * size.x / 2) - (transform.up * size.y / 4) + (transform.forward * size.z / 2), (transform.forward));
        //rightfront
        forwardRays[1] = new Ray(transform.position + (transform.right * size.x / 2) - (transform.up * size.y / 4) + (transform.forward * size.z / 2), (transform.forward));


    }

	[PunRPC]
	public void GotHit(){
		rigid.velocity = Vector3.zero;
		rigid.angularVelocity = Vector3.zero; 
		_hitTime = Time.time;
		isInvulnerable = true;
		DropFlag ();
		StartCoroutine(Blink(0.1F));
	}
	public void PickUp(){
		_shots++;
	}

	[PunRPC]
	public void PickUpFlag(CTF_FlagPickup pickup){
		Debug.Log ("Pickup");
		CTF_lastPickupPoint = pickup;
		CTF_GotFlag = true;
		CTF_Flag.SetActive (true);
	}
	[PunRPC]
	public void DropFlag(){
		CTF_GotFlag = false;
		CTF_Flag.SetActive (false);
		var p = transform.position;
		p += -transform.forward * 2;
		var flag = PhotonNetwork.Instantiate ("CTF_Flag", p, transform.rotation, 0).GetComponent<CTF_Flag>();
		flag.spawnPoint = CTF_lastPickupPoint;
	}
	[PunRPC]
	public void DeliverFlag(){
		CTF_Score++;
		Debug.Log ("SCORE");
		CTF_GotFlag = false;
		CTF_Flag.SetActive (false);
		CTF_lastPickupPoint.SpawnPickup ();
	}
	IEnumerator Blink(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		if (isInvulnerable) {
			foreach (Renderer r in _renderer) {
				r.enabled = !r.enabled;
			}
			StartCoroutine(Blink(0.1F));
		} else {
			foreach (Renderer r in _renderer) {
				r.enabled = true;
			}
		}
		//_renderer.enabled = !_renderer.enabled;
	}


    [PunRPC]
    void UpdateEnemiesMinimap()
    {
        MiniMap minimap = GameObject.FindObjectOfType<MiniMap>();
        minimap.enemies.Clear();
        foreach(var kartObject in GameObject.FindGameObjectsWithTag("kart"))
        {
            if (!kartObject.GetComponent<PhotonView>().isMine)
            {
                minimap.enemies.Add(kartObject.transform);
            }
        }
    }

}
