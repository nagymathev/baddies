using UnityEngine;
using System.Collections;

public class EventListener : MonoBehaviour
{
    public delegate void TriggerFunction(Collider col);
    public delegate void CollisionFunction(Collision coll);
    public delegate void GameObjectFunction(GameObject g);

    public event TriggerFunction OnTriggerEnterDelegate = null;
    public event TriggerFunction OnTriggerExitDelegate = null;

    public event CollisionFunction OnCollisionEnterDelegate = null;
    public event CollisionFunction OnCollisionExitDelegate = null;

    public event GameObjectFunction OnDestroyDelegate = null;
    public event GameObjectFunction OnEnableDelegate = null;
    public event GameObjectFunction OnDisableDelegate = null;


    void OnTriggerEnter(Collider col)
    {
        if (OnTriggerEnterDelegate != null) OnTriggerEnterDelegate(col);
    }

    void OnTriggerExit(Collider col)
    {
        if (OnTriggerExitDelegate != null) OnTriggerExitDelegate(col);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (OnCollisionEnterDelegate != null) OnCollisionEnterDelegate(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        if (OnCollisionExitDelegate != null) OnCollisionExitDelegate(collision);
    }

    void OnDestroy()
    {
        if (OnDestroyDelegate != null) OnDestroyDelegate(this.gameObject);
    }

    void OnEnable()
    {
        if (OnEnableDelegate != null) OnEnableDelegate(this.gameObject);
    }

    void OnDisable()
    {
        if (OnDisableDelegate != null) OnDisableDelegate(this.gameObject);
    }

    public static EventListener Get(Collider c)
    {
        return Get(c.gameObject);
    }

    public static EventListener Get(GameObject go)
    {
        EventListener el = go.GetComponent<EventListener>();
        return el ?? go.AddComponent<EventListener>();
    }

}
