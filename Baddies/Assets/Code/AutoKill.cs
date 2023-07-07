using UnityEngine;
using System.Collections;

public class AutoKill : MonoBehaviour 
{
	public float lifeTime = 5.0f;

	void Start () 
	{
		GameObject.Destroy(this.gameObject, lifeTime);
	}
}
