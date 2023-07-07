using UnityEngine;
using System.Collections;

public static class DUtil
{
	public static T GetComponentInParents<T>(this GameObject thiz) where T:Component
	{
		Transform tr = thiz.transform;
		while(tr!=null)
		{
			T comp = tr.gameObject.GetComponent<T>();
			if (comp!=null)
				return comp;
			tr = tr.parent;
		}
		return null;
	}

	public static GameObject FindInChildren(this GameObject thiz, string name)
	{
		foreach(Transform t in thiz.transform)
		{
			if (t.name.Equals(name))
				return t.gameObject;
		}

		foreach(Transform t in thiz.transform)
		{
			GameObject go = t.gameObject.FindInChildren(name);
			if (go != null)
				return go;
		}
		return null;
	}

    public static Quaternion TargetRotation(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation)
    {
        // Calculate the rotation expressed by the joint's axis and secondary axis
        var right = joint.axis;
        var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
        var up = Vector3.Cross(forward, right).normalized;
        Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

        // Transform into world space
        Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

        // Counter-rotate and apply the new local rotation.
        // Joint space is the inverse of world space, so we need to invert our value
        //if (space == Space.World)
        //{
        //resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
        //}
        //else
        //{
        resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;
        //}

        // Transform back into joint space
        resultRotation *= worldToJointSpace;

        // Set target rotation to our newly calculated rotation
        //joint.targetRotation = resultRotation;
        return resultRotation;
    }

    public static Quaternion RotationExtrapolated(Quaternion currentRotation, Vector3 angVel, float time)
    {
        if (angVel.sqrMagnitude > 0.0001f)
        {
            Quaternion qAngVel = Quaternion.AngleAxis(angVel.magnitude * Mathf.Rad2Deg * time, angVel.normalized);
            currentRotation = qAngVel * currentRotation;
        }
        return currentRotation;
    }

    public static Vector3 AngVelFromQuaternions(Quaternion firstRotation, Quaternion secondRotation, float time)
    {
        //returns ang.vel in WORLD space (left handed)
        Quaternion delta = secondRotation * Quaternion.Inverse(firstRotation);
        Vector3 axis;
        float angle;
        delta.ToAngleAxis(out angle, out axis);
        Vector3 angVel = Vector3.zero;
        if (angle > 180.0f)
        {
            angle = 360.0f - angle;
            axis = -axis;
        }
        if (Mathf.Abs(angle) > 0.001f)
        {
            angVel = axis * angle * Mathf.Deg2Rad / time;
        }
        return angVel;
    }

    public static Vector3 Position(this Matrix4x4 m)
    {
        return m.GetColumn(3);
    }

    public static Quaternion Rotation(this Matrix4x4 m)
    {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }

    public static Vector3 LocalScale(this Matrix4x4 m)
    {
        return new Vector3(m.GetColumn(0).magnitude,
                           m.GetColumn(1).magnitude,
                           m.GetColumn(2).magnitude);
    }
}
