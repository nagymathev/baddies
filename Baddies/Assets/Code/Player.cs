﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player : MonoBehaviour
{
	public bool AI;
	public bool real_input;
    public GamepadInput.GamePad.Index gamePadIndex;

	public float eyeHeight = 1.5f;
	public float timer;
	public Vector2 aimOffset;
	public Vector2 randomMove;

	public Rigidbody body;
    public Transform leftShoulder;
    public Transform rightShoulder;
    public Quaternion inputRotation = Quaternion.identity;

    [System.Serializable]
    public struct Gun
    {
        public Transform nozzle;
        public GameObject bullet;
        public float reloadTime;
        public float reloadTimer;
        public float spread;
        public int bulletsAtOnce;
    }

    public Gun[] guns;

    public Transform cameraRig;

    public float sloMoScale = 0.25f;

    public float maxSpeed = 10.0f;
    public float inputRotationSlerp = 0.02f;

    public GameObject onDeath;

    [System.Serializable]
    public struct Inputs
    {
        public Vector2 ls, rs;
        public bool LT, RT;
        public bool A, B, X, Y, LB, RB, L3, R3;
        public bool Start, Back;
    }
    public Inputs inputs, inputsLast;

    Vector3 targetVelocity;
    Vector3 cameraVel;
    Vector3 idealFwd;

    float stepTimer;
    float dashTimer;

	//AI
	[System.Serializable]
	public class KnownObject
	{
		public GameObject go;
		public float priority;
		public float distance;
	}
	public List<KnownObject> knownObjects = new List<KnownObject>();
	public int maxKnownObjects = 10;




	void Start ()
    {
		Gameplay.AddPlayer(this.gameObject);
		body = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate ()
    {
		UpdateInputs();

		//ToDo: replace with AI.
		// first of all, need to see monsters and powerups (and goals; each level needs some goal to reach)
		// priority #1: survive
		//   detect getting attacked and react by doing something (run away or attack back)
		//   prioritise visible(shootable) enemies, powerups, goal(s)
		//   interact with the highest priority item (enemy:shoot. powerup/goal: go to)
		// explore if can't see anything?

		UpdateAI();

		if (inputs.Start && !inputsLast.Start)
            Time.timeScale = (Time.timeScale == 1.0f) ? sloMoScale : 1.0f;

        if (inputs.Back)  //pressing (Back) for any other player is unspawn
            Debug.Break();  //pause

        if (inputs.LT)
        {
            if (inputs.ls.magnitude > 0.1f)
                leftShoulder.rotation = Quaternion.LookRotation(inputRotation * new Vector3(inputs.ls.x, 0, inputs.ls.y));
        } else
        {
            leftShoulder.rotation = body.rotation * Quaternion.AngleAxis(90.0f + 40.0f * Mathf.Cos(stepTimer * Mathf.PI * 2.0f), Vector3.right);
        }

        if (inputs.rs.magnitude > 0.1f)
        {
            rightShoulder.rotation = Quaternion.LookRotation(inputRotation * new Vector3(inputs.rs.x, 0, inputs.rs.y));
        } else
        {
            rightShoulder.rotation = body.rotation * Quaternion.AngleAxis(90.0f - 40.0f * Mathf.Cos(stepTimer * Mathf.PI * 2.0f), Vector3.right);
        }

        {
            //targetVelocity = inputs.LT ? Vector3.zero : inputRotation * new Vector3(inputs.ls.x, 0, inputs.ls.y) * maxSpeed;
            if (!inputs.LT)
                targetVelocity = inputRotation * new Vector3(inputs.ls.x, 0, inputs.ls.y) * maxSpeed;
            float moveEffort = inputs.LT || dashTimer > 0 ? 0.0f : inputs.ls.magnitude;
            Vector3 v = targetVelocity - body.velocity;
            v.y = 0;
            body.AddForce(Vector3.ClampMagnitude(v / 0.05f, Mathf.Lerp(10.0f, 100.0f, moveEffort)));
        }

        /*
        if (inputs.A && !inputsLast.A
            && inputs.ls.magnitude > 0.1f)
        {//dash/jump
            Vector3 dashVel = inputRotation * new Vector3(inputs.ls.x, 0, inputs.ls.y) * 20.0f;
            Vector3 v = dashVel - body.velocity;
            v.y = 0;
            body.AddForce(Vector3.ClampMagnitude(v / 0.02f, 2000.0f));
            dashTimer = 0.6f;
        }*/
        if (inputs.A && !inputsLast.A) UpdateDash(inputs.ls);
        //alternative dash experiments
        if (inputs.LB && !inputsLast.LB) UpdateDash(inputs.ls);
        //if (inputs.RB && !inputsLast.RB) UpdateDash(inputs.rs);
        if (inputs.X && !inputsLast.X) UpdateDash(Vector2.left, 0.3f);
        if (inputs.B && !inputsLast.B) UpdateDash(Vector2.right, 0.3f);

        if (dashTimer > 0)
            dashTimer -= Time.fixedDeltaTime;

        //ToDo: calculate ideal heading from arms (and velocity)
        UpdateHeading();

        UpdateCameraRig();

		if (guns.Length > 0)
		{
			UpdateGun(ref guns[0], inputs.RT);// && inputs.ls.magnitude > 0.1f);
		} else
		if (guns.Length > 1)
		{
			UpdateGun(ref guns[1], inputs.RT);// && inputs.rs.magnitude > 0.1f);

			//slight hack to make it sound better ;)
			if (inputs.LT && inputs.RT
				&& Mathf.Abs(guns[0].reloadTimer - guns[1].reloadTimer) < guns[0].reloadTime * 0.4f)
			{
				guns[1].reloadTimer += guns[1].reloadTime * 0.1f;
			}
		}

        if (dashTimer > 0)
        {
            stepTimer = 0;
        } else
        {
            stepTimer += body.velocity.magnitude * Time.fixedDeltaTime * 0.5f;
            stepTimer = stepTimer % 1.0f;
        }

        inputsLast = inputs;
    }

	void UpdateAI()
	{
		if (!AI) return;

		//vision to potentially spot new objects
		{
			GameObject go = null;
			//for now only monsters
			int nMonsters = Gameplay.singleton.monsters.Count;
			if (nMonsters > 0)
				go = Gameplay.singleton.monsters[Random.Range(0, 1000) % nMonsters];
			if (go)
			{
				if (knownObjects.FindIndex(ko => ko.go == go) < 0) //don't add already known one
				{
					KnownObject ko = new KnownObject();
					ko.go = go;
					knownObjects.Add(ko);
				}
			}
		}

		Vector3 here = transform.position + Vector3.up * eyeHeight;

		//update priorities and visibility of known objects
		for (int i= knownObjects.Count-1; i>=0; i--)
		{
			KnownObject ko = knownObjects[i];
			GameObject go = ko.go;
			if (!go) { knownObjects.RemoveAt(i); continue; }
			Vector3 there = go.transform.position + Vector3.up * eyeHeight;
			if (Physics.Raycast(here, there - here, (there - here).magnitude - 1.0f))
			{//not visible
				knownObjects.RemoveAt(i);
				continue;
			}
			ko.distance = (there - here).magnitude;
			ko.priority = 1.0f / ko.distance;

			Debug.DrawLine(here, there, Color.Lerp(Color.red, Color.green, Mathf.Clamp01(ko.priority)));
		}

		knownObjects.Sort((p1, p2) => p2.priority.CompareTo(p1.priority));
		while (knownObjects.Count > maxKnownObjects)
			knownObjects.RemoveAt(knownObjects.Count - 1);

		//at this point the highest priority one should be at the front
		if (knownObjects.Count > 0)
		{
			KnownObject ko = knownObjects[0];
			GameObject go = ko.go;
			Vector3 there = go.transform.position + Vector3.up;

			Vector3 toItLocal = Quaternion.Inverse(transform.rotation) * (there - here);
			toItLocal = Vector3.ClampMagnitude(toItLocal, 1.0f);
			//inputs.ls = new Vector2(toItLocal.x, toItLocal.z) + Random.insideUnitCircle * 0.1f;
			inputs.rs = new Vector2(toItLocal.x, toItLocal.z) + aimOffset; 
			/*inputs.LT = */inputs.RT = true;
		} else
		{
			//inputs.ls = Vector2.zero;
			inputs.rs = Vector2.zero;
			inputs.LT = inputs.RT = false;
		}

		//stay away from enemies
		Vector3 desiredMove = randomMove;
		float minDistance = 10.0f;
		for (int i = knownObjects.Count - 1; i >= 0; i--)
		{
			KnownObject ko = knownObjects[0];
			GameObject go = ko.go;
			if (ko.distance > minDistance) continue;
			float f = (minDistance - ko.distance);
			Vector3 there = go.transform.position + Vector3.up;
			desiredMove -= (there - here).normalized * f;
		}
		Vector3 desMoveLocal = Quaternion.Inverse(transform.rotation) * Vector3.ClampMagnitude(desiredMove, 1.0f);
		inputs.ls = new Vector2(desMoveLocal.x, desMoveLocal.z);

		// the rest only at lower frequency (every 0.5 sec, see below)
		if (timer > 0)
		{
			timer -= Time.fixedDeltaTime;
			return;
		}
		timer += 0.5f;

		aimOffset = Random.insideUnitCircle * 0.2f;
		randomMove = Random.insideUnitCircle * 0.2f;
	}

	void UpdateInputs()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPaused)
            return;
#endif

		if (real_input)
		{
			inputs.ls = GamepadInput.GamePad.GetAxis(GamepadInput.GamePad.Axis.LeftStick, gamePadIndex);
			inputs.rs = GamepadInput.GamePad.GetAxis(GamepadInput.GamePad.Axis.RightStick, gamePadIndex);
			inputs.LT = GamepadInput.GamePad.GetTrigger(GamepadInput.GamePad.Trigger.LeftTrigger, gamePadIndex) > 0.2f;
			inputs.RT = GamepadInput.GamePad.GetTrigger(GamepadInput.GamePad.Trigger.RightTrigger, gamePadIndex) > 0.2f;

			inputs.A = GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.A, gamePadIndex);
			inputs.B = GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.B, gamePadIndex);
			inputs.X = GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.X, gamePadIndex);
			inputs.Y = GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.Y, gamePadIndex);
			inputs.LB = GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.LeftShoulder, gamePadIndex);
			inputs.RB = GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.RightShoulder, gamePadIndex);
			inputs.L3 = GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.LeftStick, gamePadIndex);
			inputs.R3 = GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.RightStick, gamePadIndex);

			inputs.Start = GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.Start, gamePadIndex);
			inputs.Back = GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.Back, gamePadIndex);
		}
    }


    void UpdateCameraRig()
    {
		if (!cameraRig) return;

        Vector3 targetPos = body.position;
        targetPos += body.velocity * 0.6f;
        targetPos += inputRotation * new Vector3(inputs.ls.x, 0, inputs.ls.y) * 2.0f;
        targetPos += inputRotation * new Vector3(inputs.rs.x, 0, inputs.rs.y) * 2.0f;

        Vector3 targetVel = Vector3.zero;
        Vector3 cameraPos = cameraRig.position;

        cameraVel += (targetVel - cameraVel) / 0.4f;
        cameraVel += (targetPos - (cameraPos + cameraVel * 0.2f)) / 0.4f;
        cameraVel.y = 0;

        cameraPos += cameraVel * Time.fixedDeltaTime;

        cameraRig.position = cameraPos;
        cameraRig.rotation = inputRotation;
    }

    void UpdateHeading()
    {
        Vector3 fwd = inputRotation * Vector3.forward * 0.1f;
        fwd += body.velocity * 0.005f;
        float la = Mathf.Atan2(inputs.ls.x, inputs.ls.y) * Mathf.Rad2Deg;
        float ra = Mathf.Atan2(inputs.rs.x, inputs.rs.y) * Mathf.Rad2Deg;
        //Vector3 lw = inputRotation * new Vector3(inputs.ls.x, 0, inputs.ls.y);
        //Vector3 rw = inputRotation * new Vector3(inputs.rs.x, 0, inputs.rs.y);
        //if (inputs.LT)
        //    fwd += lw * 2.0f;
        //fwd += rw * 2.0f;
        idealFwd = fwd;
        idealFwd.y = 0;
        idealFwd.Normalize();
        float angle = 0.0f;
        if (inputs.ls.magnitude > 0.1f && inputs.LT)
            angle += la;// idealFwd = Quaternion.AngleAxis(la * 0.5f, Vector3.up) * idealFwd;
        if (inputs.rs.magnitude > 0.1f)
            angle += ra;// idealFwd = Quaternion.AngleAxis(ra * 0.5f, Vector3.up) * idealFwd;

        idealFwd = Quaternion.AngleAxis(Mathf.Clamp(angle, -90.0f, 90.0f), Vector3.up) * idealFwd;

        Debug.DrawRay(body.position, fwd, Color.yellow);
        Debug.DrawRay(body.position, idealFwd, Color.green);

        float delta = (inputRotation * Vector3.forward - idealFwd).magnitude;
        float f = Mathf.Clamp01(-0.5f + delta * 4.0f);
        inputRotation = Quaternion.Slerp(inputRotation, Quaternion.LookRotation(idealFwd), inputRotationSlerp * f);

        body.AddTorque(body.angularVelocity * -5.0f);
        body.AddTorque(Vector3.Cross(body.rotation * Vector3.forward, idealFwd) * 50.0f);

        Vector3 t = Vector3.Cross(body.rotation * Vector3.up, Vector3.up);
        body.AddTorque(t * 500.0f);//upright
        Debug.DrawRay(body.worldCenterOfMass, t, Color.magenta);
    }

    void UpdateGun(ref Gun gun, bool trigger)
    {
        if (gun.reloadTimer > 0)
        {
            gun.reloadTimer -= Time.fixedDeltaTime;
            return;
        }
        if (trigger)
        {
            gun.reloadTimer = gun.reloadTime;
            for (int i = 0; i < gun.bulletsAtOnce; i++)
            {
                Quaternion dir = gun.spread <= 0 ? gun.nozzle.rotation
                    : Quaternion.LookRotation(gun.nozzle.rotation * (Vector3.forward + Random.insideUnitSphere * gun.spread));
                GameObject.Instantiate(gun.bullet, gun.nozzle.position, dir);
            }
        }
    }

    void UpdateDash(Vector2 s, float t = 0.6f)
    {
        if (/*inputs.A && !inputsLast.A
            &&*/ s.magnitude > 0.1f)
        {//dash/jump
            Vector3 dashVel = inputRotation * new Vector3(s.x, 0, s.y) * 20.0f;
            Vector3 v = dashVel - body.velocity;
            v.y = 0;
            body.AddForce(Vector3.ClampMagnitude(v / 0.02f, 2000.0f));
            dashTimer = t;// 0.6f;
        }
    }

    public void OnDeath()
    {
        this.enabled = false;
        //agent.enabled = false;
        body.constraints = RigidbodyConstraints.None;
        body.AddTorque(Random.onUnitSphere * 20.0f);
        body.AddForce(Random.insideUnitSphere * 100.0f);
        //GameObject.Destroy(this.gameObject, 0.5f);

        if (onDeath != null)
            GameObject.Instantiate(onDeath, transform.position, transform.rotation);
    }

}
