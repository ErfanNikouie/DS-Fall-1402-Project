using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Vector2 zoom;
	[SerializeField] private float zoomSpeed = 5f;

	private Camera mainCamera;
	private Vector3 touchStart;
	private float groundZ = 0;

	void Start()
	{
		mainCamera = Camera.main;
	}

	void Update()
	{
		HandleZoom();
		HandlePan();
	}

	void HandleZoom()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		float newSize = mainCamera.orthographicSize - scroll * zoomSpeed;
		mainCamera.orthographicSize = Mathf.Clamp(newSize, zoom.x, zoom.y);
	}

	void HandlePan()
	{
		if (Input.GetMouseButtonDown(0))
			touchStart = GetWorldPosition(groundZ);
		
		if (Input.GetMouseButton(0))
		{
			Vector3 direction = touchStart - GetWorldPosition(groundZ);
			transform.position += direction;
		}
	}
	
	private Vector3 GetWorldPosition(float z)
	{
		Ray mousePos = mainCamera.ScreenPointToRay(Input.mousePosition);
		Plane ground = new Plane(Vector3.forward, new Vector3(0,0,z));
		ground.Raycast(mousePos, out float distance);
		return mousePos.GetPoint(distance);
	}
}