using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public GameObject camera;
    public float cameraMovementSpeed = 1;
    public float cameraTurnSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraRotation = camera.transform.rotation.eulerAngles;
        cameraRotation.x = 0;
        cameraRotation.z = 0;

        Quaternion q = Quaternion.identity;
        q.eulerAngles = cameraRotation;

        Vector3 combinedInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        combinedInput = Vector3.ClampMagnitude(combinedInput, 1);
        //Vector3 mouseInput = new Vector3(-Input.GetAxisRaw("Mouse X"), 0, -Input.GetAxisRaw("Mouse Y"));
        //mouseInput = Vector3.ClampMagnitude(combinedInput, 1);

        this.transform.position += q * combinedInput * Time.deltaTime * cameraMovementSpeed;
        //this.transform.position += q * Vector3.forward * Input.GetAxisRaw("Vertical")*Time.deltaTime * cameraMovementSpeed;
        //this.transform.position += q * Vector3.right * Input.GetAxisRaw("Horizontal")*Time.deltaTime * cameraMovementSpeed;
        //this.transform.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        /*if (Input.GetKey(KeyCode.Mouse1))
        {
            this.transform.position += q * mouseInput * cameraMovementSpeed;
        }*/
    }
}
