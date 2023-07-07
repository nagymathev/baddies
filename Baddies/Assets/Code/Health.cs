using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health;
    public bool dead;

	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnHit()
    {
        if (dead) return;

        health -= 1.0f;
        if (health <= 0)
        {
            dead = true;
            //GameObject.Destroy(this.gameObject);

            Enemy e;
            Player p;
            if ((e = GetComponent<Enemy>()) != null)
            {
                e.OnDeath();
            } else
            if ((p = GetComponent<Player>()) != null)
            {
                p.OnDeath();
            } else
            {
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}
