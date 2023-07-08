using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public GameObject onPickUp;

	void Start ()
    {
		Gameplay.AddGoal(this.gameObject);
	}
	
	void FixedUpdate ()
    {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Player p = collision.collider.gameObject.GetComponentInParents<Player>();
        if (p != null)
        {
            Health h = p.GetComponent<Health>();
            if (h != null)
                h.health += 1;

            if (onPickUp != null)
                GameObject.Instantiate(onPickUp, transform.position, transform.rotation);
            GameObject.Destroy(this.gameObject);
        }
    }
}
