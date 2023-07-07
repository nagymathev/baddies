using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public List<Transform> transforms;  //for cycling

    //public Inventory inventory;
    //public Transform inventoryCam;
    //public bool inventoryActive;

    public GamepadInput.GamePad.Index gamePadIndex = GamepadInput.GamePad.Index.One;

    //public MeleeControl meleeControl;   //temp hack
    //public Transform meleeTransformHolding;
    //public Transform meleeTransformHeld;
    //public bool holdingActive;
    //public bool heldActive;

    public Transform prevTransform;
    public float transitionTime = 0.5f;
    public float transitionTimer;

    public Transform zoomTransform;
    public float zoomPercent;
    public Vector2 zoomFoV = new Vector2(70.0f, 35.0f);
    /*
    [System.Serializable]
    public struct TransformPair
    {
        public Transform source;
        public Transform target;
        public float lerp;
        public float slerp;
    }
    public TransformPair[] pairs;
    */
    //public LookAt lookAt;
    //public Sonic sonic;

    //public SimpleLoco2 loco;
    public Transform inventoryTransform;
    public float inventoryPercent;

    public Transform smoothTarget;

    public Vector3 lastPosition;
    public Quaternion lastRotation;
    public Vector3 lastLinVel;
    public Vector3 lastAngVel;

    //public float smoothness = 1.0f;
    public float tDamping = 0.25f;
    public float tSpring = 3.0f;
    public float tX = 0.3f;



    void Start()
    {
        //if (sonic != null)
        //    sonic.aiControlled = false;

        if (cameraTransform == null)
            cameraTransform = transforms[0];
    }

    //void OnPreCull()
    void LateUpdate()
    {
        //foreach(TransformPair pair in pairs)
        //{
        //    pair.target.position = Vector3.Lerp(pair.target.position, pair.source.position, pair.lerp);
        //    pair.target.rotation = Quaternion.Slerp(pair.target.rotation, pair.source.rotation, pair.slerp);
        //}
        //if (lookAt != null)
        //    lookAt.DoUpdate();

        /*
        if (inventory != null)
        {
            if (inventory.gameObject.activeSelf && inventoryActive == false)
            {
                prevTransform = cameraTransform;
                transitionTimer = transitionTime;
                cameraTransform = inventoryCam;
            } else
            if (!inventory.gameObject.activeSelf && inventoryActive)
            {
                cameraTransform = prevTransform;
                prevTransform = inventoryCam;
                transitionTimer = transitionTime;
            }
            inventoryActive = inventory.gameObject.activeSelf;
        }
        */

        if (smoothTarget)
        {
            lastLinVel = Vector3.MoveTowards(lastLinVel, Vector3.zero, Time.deltaTime / tDamping);
            Vector3 posX = lastPosition + lastLinVel * tX;
            lastLinVel += (smoothTarget.position - posX) / tSpring;    //temp., it's frame rate dependent
            lastPosition += lastLinVel * Time.deltaTime;

            lastAngVel = Vector3.MoveTowards(lastAngVel, Vector3.zero, Time.deltaTime / tDamping);
            Quaternion rotX = DUtil.RotationExtrapolated(lastRotation, lastAngVel, tX);
            lastAngVel += DUtil.AngVelFromQuaternions(rotX, smoothTarget.rotation, tSpring);    //temp., it's frame rate dependent
            lastRotation = DUtil.RotationExtrapolated(lastRotation, lastAngVel, Time.deltaTime);

            transform.position = lastPosition;
            transform.rotation = lastRotation;
        } else
        if (cameraTransform != null)
        {
            if (transitionTimer > 0 && prevTransform != null)
            {
                float f = Mathf.Clamp01(transitionTimer / transitionTime);
                f = 3.0f * f * f - 2.0f * f * f * f;    //S-curve
                transform.position = Vector3.Lerp(cameraTransform.position, prevTransform.position, f);
                transform.rotation = Quaternion.Slerp(cameraTransform.rotation, prevTransform.rotation, f);
            } else
            {
                transform.position = cameraTransform.position;
                transform.rotation = cameraTransform.rotation;
            }

            if (zoomTransform != null)
            {
                float f = zoomPercent;
                f = 3.0f * f * f - 2.0f * f * f * f;    //S-curve
                transform.position = Vector3.Lerp(transform.position, zoomTransform.position, f);
                transform.rotation = Quaternion.Slerp(transform.rotation, zoomTransform.rotation, f);
                Camera cam = GetComponent<Camera>();
                cam.fieldOfView = Mathf.Lerp(zoomFoV.x, zoomFoV.y, f);
            }

            if (inventoryTransform != null)
            {
                float f = inventoryPercent;
                f = 3.0f * f * f - 2.0f * f * f * f;    //S-curve
                transform.position = Vector3.Lerp(transform.position, inventoryTransform.position, f);
                transform.rotation = Quaternion.Slerp(transform.rotation, inventoryTransform.rotation, f);
            }

            lastLinVel = (transform.position - lastPosition) / Time.deltaTime;
            lastAngVel = DUtil.AngVelFromQuaternions(lastRotation, transform.rotation, Time.deltaTime);

            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }

    public void Set(Transform trans)
    {
        prevTransform = cameraTransform;
        transitionTimer = transitionTime;
        cameraTransform = trans;
    }

    void Update()
    {
        if (transitionTimer > 0)
        {
            transitionTimer -= Time.deltaTime;
            float f = Mathf.Clamp01(transitionTimer / transitionTime);
        }

        if (GamepadInput.GamePad.GetButtonDown(GamepadInput.GamePad.Button.RightStick, gamePadIndex))
        {
            int i = transforms.FindIndex(t => t == cameraTransform);
            i = (i + 1) % transforms.Count;
            Set(transforms[i]);
        }

        float zoom = GamepadInput.GamePad.GetTrigger(GamepadInput.GamePad.Trigger.LeftTrigger, gamePadIndex);
        //zoomPercent = Mathf.MoveTowards(zoomPercent, zoom, Time.deltaTime * 2.0f);
        zoomPercent = Mathf.Lerp(zoomPercent, zoom, Time.deltaTime * 3.0f);

        //if (loco!=null)
        //{
        //    inventoryPercent = Mathf.Lerp(inventoryPercent, loco.browsingInventory ? 1 : 0, Time.deltaTime * 2.0f);
        //}

        /*
        if (meleeControl != null)
        {
            bool _held = meleeControl.holders.Count > 0;
            bool _holding = meleeControl.heldOpponent != null;

            if (!(_held || _holding) && (heldActive || holdingActive))
            {
                heldActive = holdingActive = false;

                prevTransform = cameraTransform;
                transitionTimer = transitionTime;

                cameraTransform = transforms[0];
            } else
            if (_held && !heldActive)
            {
                heldActive = _held;

                prevTransform = cameraTransform;
                transitionTimer = transitionTime;

                cameraTransform = meleeTransformHeld;
            } else
            if (_holding && !holdingActive)
            {
                holdingActive = _holding;

                prevTransform = cameraTransform;
                transitionTimer = transitionTime;

                cameraTransform = meleeTransformHolding;
            }
        }
        */
    }

    public void NextCamera()
    {
        int i = transforms.FindIndex(t => t == cameraTransform);
        i = (i + 1) % transforms.Count;

        Set(transforms[i]);
    }
}
