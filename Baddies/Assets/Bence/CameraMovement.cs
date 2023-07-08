using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{

    public GameObject camera;
    [SerializeField] public float _cameraMovementSpeed = 25;
    [SerializeField] public float _cameraTurnSpeed = 60;
    [SerializeField] private float _cameraZoomSpeed = 10;

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

        transform.position += rotation * inputDir * (Time.deltaTime * _cameraMovementSpeed);
        transform.Rotate(new Vector3(0, Input.GetAxisRaw("QE_Rotation") * _cameraTurnSpeed * Time.deltaTime, 0) , Space.World);

        // Zoom In and Out
        camera.transform.Translate(new Vector3(0, 0, Input.GetAxisRaw("Mouse ScrollWheel") * _cameraZoomSpeed), Space.Self);

		//Dragging
		/*if (Input.GetKey(KeyCode.Mouse1))
        {
            //this.transform.position += q * mouseInput * cameraMovementSpeed;
        }*/

		camera.transform.position += camera.transform.forward * Input.mouseScrollDelta.y;
    }
}
