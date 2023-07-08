﻿using System.Collections;
using System.Collections.Generic;
using Code.GameState;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody body;
    public UnityEngine.AI.NavMeshAgent agent;

    public bool checkVisibility;
    public float maxSpeed = 5.0f;

	public float eyeHeight = 1.5f;

	public Transform target;
    public Vector3 lastTargetPos;
    public Vector3 targetVel;

    public float timer;

    public GameObject onDeath;
    public GameObject onAttack;

	void Start ()
    {
		Gameplay.AddMonster(this.gameObject);

		if (body == null)
            body = GetComponent<Rigidbody>();

        timer = Random.Range(0, 0.5f);

        if (target == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
                target = go.transform;
        }

		if (agent)
		{
			agent.updatePosition = false;
			agent.updateRotation = false;
			agent.speed = maxSpeed;
		}
    }

    void FixedUpdate ()
    {
		// every frame...
		UpdateAgent();

        //Debug.DrawLine(transform.position, lastTargetPos, Color.white);

		// the rest only at lower frequency (every 0.5 sec, see below)
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            return;
        }
        timer += 0.5f;

        if (target != null)
        {
            Vector3 here = transform.position + Vector3.up * eyeHeight;
            Vector3 there = target.position + Vector3.up * eyeHeight;

            if (checkVisibility && Physics.Raycast(here, there - here, (there - here).magnitude - 1.0f))
            {//behind cover
                targetVel = Vector3.zero; //stop
            } else
            {//can see
                Vector3 targetPos = target.position;
                targetVel = (targetPos - lastTargetPos) / 0.5f;
                lastTargetPos = targetPos;
            }

            agent.SetDestination(lastTargetPos + targetVel * 0.5f);

            if ((there - here).magnitude <= 2.0f)
            {//attack
                Health h = target.GetComponentInParent<Health>();
                if (h != null && h.health > 0)
                {
                    //if (h.gameObject.tag != "Player")   //hack; bullets don't hurt player
                    h.OnHit();
                    Rigidbody b = h.GetComponent<Rigidbody>();
                    if (b != null)
                    {
                        b.AddTorque(Random.onUnitSphere * 100.0f);
                        b.AddForce((there-here).normalized * 300.0f);
                    }

                    if (onAttack != null)
                        GameObject.Instantiate(onAttack, transform.position, Quaternion.LookRotation(there - here));
                }
            }

        }
    }

	void UpdateAgent()
	{
		if (!agent) return;

		// every frame...
		agent.velocity = body.velocity + (body.position - agent.nextPosition) * 0.1f;
		if (agent.hasPath)
		{
			int n = 1;
			Vector3 targetPos = agent.path.corners[n];
			//while (n < agent.path.corners.Length-1 && (targetPos - body.position).magnitude < 2.0f)
			//{
			//    targetPos = agent.path.corners[n];
			//    n++;
			//}
			//Vector3 moveDir = Vector3.ClampMagnitude(targetPos - body.position, 3.0f);
			//Debug.DrawRay(body.position, moveDir, Color.red);
			Vector3 targetVel = (targetPos - body.position);
			targetVel.y = 0.0f;
			if (targetVel.magnitude < 1.0f && targetVel.sqrMagnitude > 0.0001f)
				targetVel = targetVel.normalized * maxSpeed;
			else
				targetVel = Vector3.ClampMagnitude(targetVel, maxSpeed);    //max speed
			Vector3 v = targetVel - body.velocity;
			v.y = 0;
			body.AddForce(Vector3.ClampMagnitude(v / 0.05f, 10.0f));
			
			//simple turn towards movement
			body.AddTorque(Vector3.Cross(body.transform.forward, targetVel) * 5.0f );
		}
	}

	public void OnDeath()
    {
        this.enabled = false;
        agent.enabled = false;

        body.constraints = RigidbodyConstraints.None;
        body.AddTorque(Random.onUnitSphere * 20.0f);
        body.AddForce(Random.insideUnitSphere * 100.0f);

        GameObject.Destroy(this.gameObject, 0.5f);
        EventManager.Instance.MinionKilled(this, this, null);

        if (onDeath != null)
            GameObject.Instantiate(onDeath, transform.position, transform.rotation);
    }
}
