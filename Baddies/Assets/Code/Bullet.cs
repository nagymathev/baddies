using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50.0f; //m/s
    public float range = 50.0f; //m

    public float lifeTime;
    public bool landed;
    public bool timedOut;

    public GameObject onFire;
    public GameObject onHit;


    void Start ()
    {
        lifeTime = range / speed;
        GameObject.Instantiate(onFire, transform.position, transform.rotation);
	}
	
	void FixedUpdate ()
    {
        if (landed || timedOut) return;
        if (lifeTime <= 0)
        {
            timedOut = true;
            GameObject.Destroy(this.gameObject, 0.1f);
        }
        lifeTime -= Time.fixedDeltaTime;

        Vector3 lastPos = transform.position;
        transform.position += transform.forward * speed * Time.fixedDeltaTime;

        RaycastHit hit;
        if (Physics.Raycast(lastPos, transform.forward, out hit, speed * Time.fixedDeltaTime))
        {//hit something
         //check if it's the player/owner, and ignore if it is
            if (hit.collider.gameObject.GetComponentInParents<Player>())   //hack; bullets don't hurt player
                return;

            transform.position = hit.point;
            //ToDo: explode
            landed = true;
            GameObject.Destroy(this.gameObject, 0.1f);
            GameObject.Instantiate(onHit, hit.point, Quaternion.LookRotation(hit.normal));

            if (hit.collider.attachedRigidbody!=null)
            {
                hit.collider.attachedRigidbody.AddForceAtPosition(transform.forward * 250.0f, hit.point);
            }
            Health h = hit.collider.GetComponentInParent<Health>();
            if (h != null)
            {
                h.OnHit();
            }
        } else
        {//didn't
        }
	}
}
