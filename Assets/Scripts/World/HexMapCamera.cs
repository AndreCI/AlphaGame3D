using System.Collections;
using UnityEngine;

public class HexMapCamera : MonoBehaviour {



	public float stickMinZoom, stickMaxZoom;

	public float swivelMinZoom, swivelMaxZoom;

	public float moveSpeedMinZoom, moveSpeedMaxZoom;

	public float rotationSpeed;

    public float Delta = 0.5f;
    public float distancePerFrame = 0.15f;
    private float savedDistancePerFrame = 0.15f;
    Transform swivel, stick;

	public HexGrid grid;

	float zoom = 1f;

	float rotationAngle = 90;
    private bool moving;
    private float xTarget;
    private float zTarget;

	public static HexMapCamera instance;

	public static bool Locked {
		set {
			instance.enabled = !value;
		}
	}

	public static void ValidatePosition () {
		instance.AdjustPosition(0f, 0f);
	}

	void Awake () {
		swivel = transform.GetChild(0);
		stick = swivel.GetChild(0);
        savedDistancePerFrame = distancePerFrame;
	}

	void OnEnable () {
		instance = this;
		ValidatePosition();
	}

	void Update () {
		float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
		if (zoomDelta != 0f) {
			AdjustZoom(zoomDelta);
		}

		float rotationDelta = Input.GetAxis("Rotation");
		if (rotationDelta != 0f) {
			AdjustRotation(rotationDelta);
            moving = false;
            distancePerFrame = savedDistancePerFrame;
        }

		float xDelta = Input.GetAxis("Horizontal");
		float zDelta = Input.GetAxis("Vertical");

        if (xDelta != 0f || zDelta != 0f) {
			AdjustPosition(xDelta, zDelta);
            moving = false;
            distancePerFrame = savedDistancePerFrame;
		}
        if (moving)
        {
            AdjdustPositionToTarget();
        }
	}

	void AdjustZoom (float delta) {
		zoom = Mathf.Clamp01(zoom + delta);

		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);

		float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
	}

	void AdjustRotation (float delta) {
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		if (rotationAngle < 0f) {
			rotationAngle += 360f;
		}
		else if (rotationAngle >= 360f) {
			rotationAngle -= 360f;
		}
		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
	}

	void AdjustPosition (float xDelta, float zDelta) {
		Vector3 direction =
			transform.localRotation *
			new Vector3(xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance =
			Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
			damping * Time.deltaTime;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition =
			grid.wrapping ? WrapPosition(position) : ClampPosition(position);
	}
    public void SetPosition(float x, float z, float speed=-1)
    {
        if (speed != -1)
        {
            distancePerFrame = speed;
        }
        xTarget = x;
        zTarget = z;
        moving = true;
    }
    private void AdjdustPositionToTarget()
    {
        if (!(transform.localPosition.x < xTarget + Delta &&
              transform.localPosition.x > xTarget - Delta &&
              transform.localPosition.z < zTarget + Delta &&
              transform.localPosition.z > zTarget - Delta))
        {
            float xDelta = xTarget - transform.localPosition.x;// > 0 ? 1 : -1;
            float zDelta = zTarget - transform.localPosition.z;// > 0 ? 1 : -1;
            Vector3 direction = new Vector3(xDelta, 0f, zDelta).normalized;
            float damping = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.z));
            float dampingDelta = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
            float speedBasedOnDistance = Mathf.Log(dampingDelta, 2) + 1;
            float distance =
            Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
            damping * Time.deltaTime * distancePerFrame * speedBasedOnDistance ;
            Vector3 position = transform.localPosition;
            position += direction * distance;
            transform.localPosition =
                grid.wrapping ? WrapPosition(position) : ClampPosition(position);
        }
        else
        {
            moving = false;
            distancePerFrame = savedDistancePerFrame;
        }
    
        
    }

    Vector3 ClampPosition (Vector3 position) {
		float xMax = (grid.cellCountX - 0.5f) * HexMetrics.innerDiameter;
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		return position;
	}

	Vector3 WrapPosition (Vector3 position) {
		float width = grid.cellCountX * HexMetrics.innerDiameter;
		while (position.x < 0f) {
			position.x += width;
		}
		while (position.x > width) {
			position.x -= width;
		}

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		grid.CenterMap(position.x);
		return position;
	}
}