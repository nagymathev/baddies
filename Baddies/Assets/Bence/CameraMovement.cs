using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{

    [SerializeField]
	public GameObject _camera;
    [SerializeField]
	public GameObject _cameraPivot;
    [SerializeField]
	public float _cameraMovementSpeed = 25;
    [SerializeField]
	public float _cameraTurnSpeed = 60;
    [SerializeField]
	public float _cameraZoomSpeed = 10;
    [SerializeField]
	public float _cameraDragSpeed = 1;
    [SerializeField]
	public float _cameraMouseSensitivity = 3;
    public float _zoomLevel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraRotation = _camera.transform.rotation.eulerAngles;
        var rotation = Quaternion.Euler(0, cameraRotation.y, 0);

        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputDir = Vector3.ClampMagnitude(inputDir, 1);

        transform.position += rotation * inputDir * (Time.deltaTime * _cameraMovementSpeed);
        transform.Rotate(new Vector3(0, Input.GetAxis("QE_Rotation") * _cameraTurnSpeed * Time.deltaTime, 0) , Space.World);

        // Zoom In and Out
        _camera.transform.Translate(new Vector3(0, 0, Input.GetAxisRaw("Mouse ScrollWheel") * _cameraZoomSpeed), Space.Self);
		float dist = Vector3.Dot(_camera.transform.forward, transform.position - _camera.transform.position);
		if (dist < 2.0f)
			_camera.transform.position -= _camera.transform.forward * (3.0f - dist);
		if (dist > 50.0f)
			_camera.transform.position -= _camera.transform.forward * (50.0f - dist);

		_zoomLevel = dist;// (transform.position - _camera.transform.position).magnitude;

		//Dragging / Panning
		if (Input.GetMouseButton(1))
        {
            transform.position += rotation * new Vector3(-Input.GetAxisRaw("Mouse X"), 0 , -Input.GetAxisRaw("Mouse Y")) * ((_zoomLevel + _cameraDragSpeed) / 10);
        }

        if (Input.GetMouseButton(2))
        {
            transform.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X") * _cameraMouseSensitivity, 0), Space.World);
            _cameraPivot.transform.Rotate(new Vector3(-Input.GetAxisRaw("Mouse Y") * _cameraMouseSensitivity, 0, 0), Space.Self);
            var angles = transform.rotation.eulerAngles;
            _cameraPivot.transform.rotation = Quaternion.Euler(Mathf.Clamp(_cameraPivot.transform.rotation.eulerAngles.x, 290, 350), angles.y, 0);
        }

		// _camera.transform.position += _camera.transform.forward * Input.mouseScrollDelta.y;
    }
}
