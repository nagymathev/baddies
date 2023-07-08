using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public GameObject camera;
    public float cameraMovementSpeed = 25;
    public float cameraTurnSpeed = 60;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraRotation = camera.transform.rotation.eulerAngles;
        var rotation = Quaternion.Euler(0, cameraRotation.y, 0);

        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        inputDir = Vector3.ClampMagnitude(inputDir, 1);

        transform.position += rotation * inputDir * (Time.deltaTime * cameraMovementSpeed);
        transform.Rotate(new Vector3(0, Input.GetAxisRaw("QE_Rotation") * cameraTurnSpeed * Time.deltaTime, 0) , Space.World);

        //Dragging
        /*if (Input.GetKey(KeyCode.Mouse1))
        {
            //this.transform.position += q * mouseInput * cameraMovementSpeed;
        }*/
    }
}
