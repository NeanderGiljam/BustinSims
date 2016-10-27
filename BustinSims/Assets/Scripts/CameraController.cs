using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private float followDistance = 7;
	private float followHeight = 3.5f;
	private float heightOffset = 1;

	private float rotationDamping = 2;
	private float heightDamping = 2;
	//private float zoomRatio = 1;
	//private float defaultFOV = 60;

	private Vector3 rotationVector;
	private Camera cam;

	private float cursorSpeed = 4f;
	private float cursorRotationSpeed = 40f;
	private Transform cursorObject;

	private bool isInit;

	private void Awake() {
		cam = Camera.main;

		CreateCursorObject();

		if (cursorObject != null) {
			isInit = true;
		}
	}

	private void Update() {
		if (!isInit)
			return;

		CursorMovement();
	}

	private void LateUpdate() {
		if (!isInit)
			return;

		float scrollValue = -Input.GetAxis("Mouse ScrollWheel");
		followDistance += scrollValue;
		followHeight += scrollValue;
		followDistance = Mathf.Clamp(followDistance, 0.5f, 20);
		followHeight = Mathf.Clamp(followHeight, 0.5f, 20);

		float wantedAngle = cursorObject.eulerAngles.y;
		float wantedHeight = cursorObject.position.y + followHeight;
		float cameraAngle = cam.transform.eulerAngles.y;
		float cameraHeight = cam.transform.position.y;

		cameraAngle = Mathf.LerpAngle(cameraAngle, wantedAngle, rotationDamping * Time.smoothDeltaTime);
		cameraHeight = Mathf.Lerp(cameraHeight, wantedHeight, heightDamping * Time.smoothDeltaTime);

		Quaternion currentRotation = Quaternion.Euler(new Vector3(0, cameraAngle, 0));
		cam.transform.position = cursorObject.position;
		cam.transform.position -= currentRotation * Vector3.forward * followDistance;
		cam.transform.position = new Vector3(cam.transform.position.x, cameraHeight, cam.transform.position.z);
		cam.transform.LookAt(cursorObject.position + new Vector3(0, heightOffset, 0));
	}

	private void CursorMovement() {
		Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * cursorSpeed;
		if (Input.GetKey(KeyCode.E)) {
			cursorObject.transform.Rotate(Vector3.up, -Time.deltaTime * cursorRotationSpeed);
		} else if (Input.GetKey(KeyCode.Q)) {
			cursorObject.transform.Rotate(Vector3.up, Time.deltaTime * cursorRotationSpeed);
		}

		cursorObject.transform.Translate(movement);
	}

	private void CreateCursorObject() {
		Object orgObj = Resources.Load("Prefabs/Cursor");
		cursorObject = (Instantiate(orgObj) as GameObject).transform;
	}
}