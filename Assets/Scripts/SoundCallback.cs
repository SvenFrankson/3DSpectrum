//	The MIT License (MIT)
//	Copyright (c) 2015 SvenFrankson (sven.frankson@gmail.com)
//		Permission is hereby granted, free of charge, to any person obtaining a copy
//		of this software and associated documentation files (the "Software"), to deal
//		in the Software without restriction, including without limitation the rights
//		to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//		copies of the Software, and to permit persons to whom the Software is
//		furnished to do so, subject to the following conditions:
//		The above copyright notice and this permission notice shall be included in all
//		copies or substantial portions of the Software.
//		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//		IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//		FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//		AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//		LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//		OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//		SOFTWARE.

using UnityEngine;
using System.Collections;

namespace SvenFrankson.AudioSpectrum {

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
}
