using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
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
			// Celebratory VFX :)
			if (onPickUp != null)
                GameObject.Instantiate(onPickUp, transform.position, transform.rotation);
            GameObject.Destroy(this.gameObject);

			//Actual effect
			if (Gameplay.singleton)
				Gameplay.singleton.OnPlayerReachedGoal();
		}
	}
}
