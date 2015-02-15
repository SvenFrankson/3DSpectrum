using UnityEngine;
using System.Collections;

public class SoundCallback : MonoBehaviour {
	
	public float upSpeed;
	public float downSpeed;
	public float maxMult = 5f;

	public int limit;

	public Transform QuickBar;
	public Transform Step;

	static public float depth = 0f;

	public void OnSpectrumComputedLocal (float data) {
		
		this.QuickBar.transform.localPosition = new Vector3 (this.QuickBar.transform.localPosition.x, data * this.maxMult - 5.1f, this.QuickBar.transform.localPosition.z);
		
		if (this.Step.transform.localPosition.y < data * this.maxMult) {
			float newHeight = Mathf.Min (this.Step.transform.localPosition.y + this.upSpeed * Time.deltaTime, data * this.maxMult);
			this.Step.transform.localPosition = new Vector3 (this.Step.transform.localPosition.x, newHeight, this.Step.transform.localPosition.z);
		}
		else {
			float newHeight = Mathf.Max (this.Step.transform.localPosition.y - this.downSpeed * Time.deltaTime, data * this.maxMult);
			this.Step.transform.localPosition = new Vector3 (this.Step.transform.localPosition.x, newHeight, this.Step.transform.localPosition.z);
		}
	}
	
	public void OnSpectrumComputed (float data) {
		this.QuickBar.transform.position = new Vector3 (this.QuickBar.transform.position.x, data * this.maxMult - 5.1f, this.QuickBar.transform.position.z);

		if (this.Step.transform.position.y < data * this.maxMult) {
			float newHeight = Mathf.Min (this.Step.transform.position.y + this.upSpeed * Time.deltaTime, data * this.maxMult);
			this.Step.transform.position = new Vector3 (this.Step.transform.position.x, newHeight, this.Step.transform.position.z);
		}
		else {
			float newHeight = Mathf.Max (this.Step.transform.position.y - this.downSpeed * Time.deltaTime, data * this.maxMult);
			this.Step.transform.position = new Vector3 (this.Step.transform.position.x, newHeight, this.Step.transform.position.z);
		}
	}

	public void OnSpectrumComputedGenerate (float data) {
		GameObject.Instantiate (this.Step.gameObject, this.Step.position + new Vector3 (0, 0, depth), this.Step.rotation);

		if (this.Step.transform.position.y < data * this.maxMult) {
			float newHeight = Mathf.Min (this.Step.transform.position.y + this.upSpeed * Time.deltaTime, data * this.maxMult);
			this.Step.transform.position = new Vector3 (this.Step.transform.position.x, newHeight, this.Step.transform.position.z);
		}
		else {
			float newHeight = Mathf.Max (this.Step.transform.position.y - this.downSpeed * Time.deltaTime, data * this.maxMult);
			this.Step.transform.position = new Vector3 (this.Step.transform.position.x, newHeight, this.Step.transform.position.z);
		}
	}
}
