using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 randomPosScale = Vector3.one * 0.5f;
    public Vector3 randomRotationScale = Vector3.zero;

    public float spawnTime = 1.0f;
    public bool onlyIfEmpty = true;
    public LayerMask layerMask = -1;


    public float timer;

    public delegate void GameObjectFunction(GameObject g);
    public event GameObjectFunction OnSpawnDelegate = null;


    void Start ()
    {
	}
	
	void FixedUpdate ()
    {
        if (spawnTime <= 0) return;

        timer += Time.fixedDeltaTime;

        while (timer > spawnTime)
        {
            timer -= spawnTime;

            if (onlyIfEmpty)
            {
                if (Physics.CheckBox(transform.position, transform.lossyScale * 0.5f, transform.rotation, layerMask))
                    continue;   //not empty
            }

            Vector3 pos = transform.TransformPoint(Vector3.Scale(Random.insideUnitSphere, randomPosScale));
            Quaternion rot = transform.rotation * Quaternion.Euler(Vector3.Scale(Random.insideUnitSphere, randomRotationScale));

            GameObject go = GameObject.Instantiate(prefab, pos, rot);
            OnSpawnDelegate(go);
        }
	}
}
