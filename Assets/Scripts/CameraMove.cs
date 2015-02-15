using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

	public float Speed;
	public float maxAzimut = 45f;
	public float maxLace = 60f;

	public float minX;
	private float aMinX;
	private float bMinX;
	public float maxX;
	private float aMaxX;
	private float bMaxX;
	public float minY;
	private float aMinY;
	private float bMinY;
	public float maxY;
	private float aMaxY;
	private float bMaxY;

	public float borderSize;

	// Use this for initialization
	public void ComputeLinearConstants () {
		this.aMinX = this.maxLace / this.borderSize;
		this.bMinX = - this.aMinX * this.minX;

		this.aMaxX = - this.maxLace / this.borderSize;
		this.bMaxX = - this.aMaxX * this.maxX;

		this.aMinY = this.maxAzimut / this.borderSize;
		this.bMinY = - this.aMinY * this.minY;
		
		this.aMaxY = - this.maxAzimut / this.borderSize;
		this.bMaxY = - this.aMaxY * this.maxY;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 movement = this.transform.forward;
		movement = movement * Speed / movement.z;
		this.transform.position += movement;

		float azimut = (Input.mousePosition.y - Screen.height / 2f) / (Screen.height / 2f);
		float lace = (Input.mousePosition.x - Screen.width / 2f) / (Screen.width / 2f);

		this.transform.rotation = Quaternion.Lerp (this.transform.rotation, Quaternion.Euler (new Vector3 (- azimut * this.maxAzimut, lace * this.maxLace, 0f)), Time.deltaTime);

		Vector3 flatForward = this.transform.forward - this.transform.forward.y * Vector3.up;

		if (this.transform.position.x < this.minX + this.borderSize) {
			if (this.transform.forward.x < 0) {
				float maxAngle = this.aMinX * this.transform.position.x + this.bMinX;
				float angle = Vector3.Angle (flatForward, Vector3.forward);

				if (angle > maxAngle) {
					this.transform.rotation = this.transform.rotation * Quaternion.AngleAxis (angle - maxAngle, Vector3.up);
				}
			}
		}
		if (this.transform.position.x > this.maxX - this.borderSize) {
			if (this.transform.forward.x > 0) {
				float maxAngle = this.aMaxX * this.transform.position.x + this.bMaxX;
				float angle = Vector3.Angle (flatForward, Vector3.forward);
				
				if (angle > maxAngle) {
					this.transform.rotation = this.transform.rotation * Quaternion.AngleAxis (angle - maxAngle, - Vector3.up);
				}
			}
		}

		Vector3 straightForward = this.transform.forward - this.transform.forward.x * Vector3.right;
		
		if (this.transform.position.y < this.minY + this.borderSize) {
			if (this.transform.forward.y < 0) {
				float maxAngle = this.aMinY * this.transform.position.y + this.bMinY;
				float angle = Vector3.Angle (straightForward, Vector3.forward);
				
				if (angle > maxAngle) {
					this.transform.rotation = this.transform.rotation * Quaternion.AngleAxis (angle - maxAngle, - Vector3.right);
				}
			}
		}
		if (this.transform.position.y > this.maxY - this.borderSize) {
			if (this.transform.forward.y > 0) {
				float maxAngle = this.aMaxY * this.transform.position.y + this.bMaxY;
				float angle = Vector3.Angle (straightForward, Vector3.forward);
				
				if (angle > maxAngle) {
					this.transform.rotation = this.transform.rotation * Quaternion.AngleAxis (angle - maxAngle, Vector3.right);
				}
			}
		}
	}
}
