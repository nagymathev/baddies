using UnityEngine;
using System.Collections;

public class MomentumFollow : MonoBehaviour
{
    //ToDo: support target velocity

    public Transform target;
    public bool linear = true;
    public bool angular = true;

    public Vector3 lastPosition;
    public Quaternion lastRotation;
    public Vector3 lastLinVel;
    public Vector3 lastAngVel;

    //public float smoothness = 1.0f;
    public float tDamping = 0.25f;
    public float tSpring = 3.0f;
    public float tX = 0.3f;

    private void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        float dT = Time.fixedDeltaTime;

        if (linear)
        {
            lastLinVel = Vector3.MoveTowards(lastLinVel, Vector3.zero, dT / tDamping);
            Vector3 posX = lastPosition + lastLinVel * tX;
            lastLinVel += (target.position - posX) / tSpring;    //temp., it's frame rate dependent
            lastPosition += lastLinVel * dT;

            transform.position = lastPosition;
        }

        if (angular)
        {
            lastAngVel = Vector3.MoveTowards(lastAngVel, Vector3.zero, dT / tDamping);
            Quaternion rotX = DUtil.RotationExtrapolated(lastRotation, lastAngVel, tX);
            lastAngVel += DUtil.AngVelFromQuaternions(rotX, target.rotation, tSpring);    //temp., it's frame rate dependent
            lastRotation = DUtil.RotationExtrapolated(lastRotation, lastAngVel, dT);

            transform.rotation = lastRotation;
        }
    }
}
