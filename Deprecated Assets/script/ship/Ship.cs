using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;


public class Ship : KittyNetworkBehaviour {

		public float thrust;
		public float thrustHover;
		public float thrustfwd;
		public float pitch;
		public float roll;
		public float strafe;
		bool inertiadamp;
		bool gyro;
		public bool hover;
		public bool focus;
		public float force = 1f;

		public MouseLook mouseLook;

		public Rigidbody rb { get; private set; }
		public GBody gbody { get; private set; }
		public Player owner { get; private set; }
		public Camera cockpit { get; private set; }
		public Camera playercam { get ; private set; }

		protected override void Awake() {
			base.Awake();
				cockpit = GetComponentInChildren<Camera>();
				rb = GetComponent<Rigidbody>();
				gbody = GetComponent<GBody>();
				CameraRig rig = CameraRig.instance;
			 	playercam = rig.cam;
				inertiadamp = true;
				gyro = true;
		}

		void Start() {
			if (isLocalPlayer) {
				//CameraRig.instance.Retarget(transform);
			}
			cockpit.enabled = false;
		}

		public void PivotTo(Vector3 position){
     Vector3 offset = transform.position - position;
     foreach (Transform child in transform)
         child.transform.position += offset;
     		 transform.position = position;
 		}


		void FixedUpdate() {
			if (isLocalPlayer || (owner != null && owner.isLocalPlayer)) {

			if (hover){
					rb.AddForce(transform.up * thrustHover);
				}

			if (focus) {
				if (Input.GetKey (KeyCode.Space))
					rb.AddForce(transform.up * thrust);
				if (Input.GetKey (KeyCode.LeftShift))
					rb.AddForce(-transform.up * thrust);
				if (Input.GetKey (KeyCode.W))
					rb.AddForce(transform.forward * thrustfwd);
				if (Input.GetKey (KeyCode.S))
					rb.AddForce(-transform.forward * thrustfwd);
				if (Input.GetKey (KeyCode.A))
					rb.AddForce(-transform.right * strafe);
				if (Input.GetKey (KeyCode.D))
					rb.AddForce(transform.right * strafe);
				if (Input.GetKey (KeyCode.Q))
					rb.AddTorque(transform.forward * roll);
				if (Input.GetKey (KeyCode.E))
					rb.AddTorque(-transform.forward * roll);


				Vector3 targetDelta = mouseLook.gimbal.forward;
				//get the angle between transform.forward and target delta
				float angleDiff = Vector3.Angle(transform.forward, targetDelta);
				// get its cross product, which is the axis of rotation to
				// get from one vector to the other
				Vector3 cross = Vector3.Cross(transform.forward, targetDelta);
				// apply torque along that axis according to the magnitude of the angle.
				rb.AddTorque(cross * angleDiff * force);

			}
		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {			//sets camera to focus on ship
		focus = !focus;
		cockpit.enabled = !cockpit.enabled;
		playercam.enabled = !playercam.enabled;
		}
		if (Input.GetKeyDown (KeyCode.I)){
			inertiadamp = !inertiadamp;
		}
		if (!inertiadamp){
			rb.drag = 0;
		}else{
			rb.drag = 2.5f;
		}
		if ((Input.GetKeyDown (KeyCode.H)) && focus){
			hover = !hover;
		}

		if (Input.GetKeyDown (KeyCode.G)){
			gyro = !gyro;
		}
		if (!gyro){
			rb.angularDrag = 0;
		}else{
			rb.angularDrag = 5f;
		}
		if (focus) {
			// call the method
			  mouseLook.UpdateGimbal(transform.up);
		}
	}


	public void SetOwner(Player owner) {
		 this.owner = owner;
		 // make sure the ship is aligned to the same gravity field as its owner
		 // (quick and dirty for now; this will be done differently when we get a better idea)
		 // about how to handle spawning objects in general)
		 this.gbody.SetGField(owner.gfield);
		 // if the server is the one changing this ship's owner,
		 // make sure the change is done on the client as well!
		 if (isServer) {
			 RpcSetOwner(owner.Id());
		 }
	 }

	 [ClientRpc]
	 void RpcSetOwner(Id id) {
		 SetOwner(id.Find<Player>());
	 }

}
