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
using System.Collections.Generic;

namespace SvenFrankson.AudioSpectrum {

	public class MusicTerrain : MonoBehaviour {

		private float[] datas = new float[8192];
		private int[] datasLimits;
		public int resolution;													// Count for how many parts the sound will be divided.
		private List<SoundCallback> targets = new List<SoundCallback> ();

		public int mDatasLimit = 10;											// Count for how many frames sound data will be buffered before generating mesh chunck.
		public List<float[]> tmpDatas = new List<float[]> ();
		public float step = 0.3f;												// Distance between each sound data on the depth axis.
		private float heightMult;
		private List<GameObject> chuncks = new List<GameObject> ();
		private int maxChuncks;

		public Transform mainCamera;
		public GameObject equalizerPrefab;

		public Transform box;

		public Material terrainMat;

		public float waterHeight;
		public Material waterMat;

		public Material cloudMat;
		public float cloudSpeed;

		void Start () {

			this.ComputeDatasLimits ();

			this.GenerateEqualizer ();

			this.SetCameraStartPos ();

			this.SetWallStartPos ();
		}

		/// <summary>
		/// Computes the datas limits, hence the borders (in Hz) of each part of the visualized spectrum.
		/// </summary>
		private void ComputeDatasLimits () {
			this.datasLimits = new int[this.resolution + 1];
			
			float inc = (Mathf.Log (8192) - Mathf.Log (10)) / this.resolution;
			
			for (int i = 0; i < this.resolution; i++) {
				this.datasLimits [i] = Mathf.CeilToInt(Mathf.Exp (inc * i)) + 10;
				if (i > 0) {
					if (this.datasLimits[i] <= this.datasLimits[i - 1]) {
						this.datasLimits[i] = this.datasLimits[i - 1] + 1;
					}
				}
			}
			this.datasLimits [this.resolution] = 8192;
			
			this.heightMult = this.resolution / 3f;
		}

		/// <summary>
		/// Sets the camera start position, according to the size of the computation (resolution).
		/// </summary>
		private void SetCameraStartPos () {
			float ratio = Screen.width / Screen.height;
			float back = this.resolution / (2f * ratio * Mathf.Tan (Mathf.Deg2Rad * this.camera.fieldOfView / 2));
			back += this.step * this.mDatasLimit;

			float right = this.resolution / 2f;

			float height = right / 4f;

			this.transform.position = new Vector3 (right, height, - back);

			this.maxChuncks = Mathf.CeilToInt((back / (this.step * (this.mDatasLimit - 2)))) + 20;

			CameraMove cm = this.GetComponent<CameraMove> ();
			cm.minX = 1f;
			cm.maxX = (this.resolution - 1f) - 1f;
			cm.minY = this.waterHeight + 1f + 1f;
			cm.maxY = (this.resolution - 1) / 2f - 1f;

			cm.ComputeLinearConstants ();
		}

		/// <summary>
		/// Sets the wall start position, according to the size of the computation (resolution).
		/// </summary>
		private void SetWallStartPos () {
			this.box.transform.localPosition -= this.step * (this.mDatasLimit) * Vector3.forward;

			Transform back = this.box.transform.FindChild ("BackWall");
			back.localPosition = new Vector3 ((this.resolution - 1) / 2f, 0, 0);
			back.localScale = new Vector3 ((this.resolution - 1) / 10f, 0, (this.resolution - 1) / 10f);

			Transform right = this.box.transform.FindChild ("RightWall");
			right.localPosition = new Vector3 ((this.resolution - 1), 0, -(this.resolution - 1) / 2f);
			right.localScale = new Vector3 ((this.resolution - 1) / 10f, 0, (this.resolution - 1) / 10f);
			
			Transform left = this.box.transform.FindChild ("LeftWall");
			left.localPosition = new Vector3 (0, 0, -(this.resolution - 1) / 2f);
			left.localScale = new Vector3 ((this.resolution - 1) / 10f, 0, (this.resolution - 1) / 10f);
			
			Transform water = this.box.transform.FindChild ("Water");
			water.localPosition = new Vector3 ((this.resolution - 1) / 2f, this.waterHeight + 1f, -(this.resolution - 1) / 2f);
			water.localScale = new Vector3 ((this.resolution - 1) / 10f, 0, (this.resolution - 1) / 10f);
			
			Transform ground = this.box.transform.FindChild ("Ground");
			ground.localPosition = new Vector3 ((this.resolution - 1) / 2f, 0, -(this.resolution - 1) / 2f);
			ground.localScale = new Vector3 ((this.resolution - 1) / 10f, 0, (this.resolution - 1) / 10f);
			
			Transform top = this.box.transform.FindChild ("Top");
			top.localPosition = new Vector3 ((this.resolution - 1) / 2f, (this.resolution - 1) / 2f, -(this.resolution - 1) / 2f);
			top.localScale = new Vector3 ((this.resolution - 1) / 10f, 0, (this.resolution - 1) / 10f);
		}

		/// <summary>
		/// Generates the equalizers, the items representing the frequency you may see in background in original project.
		/// </summary>
		void GenerateEqualizer () {
			for (int i = 0; i < this.resolution; i++) {
				GameObject g = GameObject.Instantiate (this.equalizerPrefab) as GameObject;
				SoundCallback scb = g.GetComponent<SoundCallback> ();
				g.transform.parent = this.box;
				g.transform.localPosition = new Vector3 (i, 0, 0);

				scb.upSpeed = 1000f;
				scb.downSpeed = this.heightMult / 5f;
				scb.maxMult = this.heightMult * 5;

				this.targets.Add (scb);
			}
		}

		/// <summary>
		/// Update loop. Sound datas are gathered in this method.
		/// </summary>
		void Update () {
			AudioListener.GetSpectrumData (this.datas, 0, FFTWindow.Blackman);
			float[] maxs = new float[this.resolution];

			for (int i = 0; i < this.resolution; i++) {
				maxs[i] = 0f;
				for (int j = this.datasLimits [i]; j < this.datasLimits [i + 1]; j++) {
					if (maxs[i] < this.datas[j]) {
						maxs[i] = this.datas[j];
					}
				}
			}

			for (int i = 0; i < this.resolution; i++) {
				this.targets[i].OnSpectrumComputedLocal (maxs[i]);
			}

			this.tmpDatas.Add (maxs);
			if (this.tmpDatas.Count > this.mDatasLimit) {
				GameObject g = GameObject.CreatePrimitive (PrimitiveType.Cube);
				MeshFilter mf = g.GetComponent<MeshFilter> ();
				MeshRenderer mr = g.GetComponent<MeshRenderer> ();

				mf.sharedMesh = this.BuildMeshFromDatasRaw (this.tmpDatas);
				mr.material = this.terrainMat;

				if (this.chuncks.Count > 0) {
					g.transform.position = this.chuncks[this.chuncks.Count - 1].transform.position + new Vector3 (0, 0, this.step * (this.mDatasLimit - 2));
				}
				else {
					g.transform.position = Vector3.zero + Vector3.up;
				}
				this.chuncks.Add(g);

				if (this.chuncks.Count > this.maxChuncks) {
					GameObject.Destroy (this.chuncks[0]);
					this.chuncks.RemoveAt (0);
				}
				
				this.tmpDatas.RemoveRange (0, this.tmpDatas.Count - 3);
			}

			this.box.transform.position += this.step * Vector3.forward;

			this.cloudMat.mainTextureOffset += new Vector2(this.cloudSpeed * Time.deltaTime / 10f, 0);
		}

		/// <summary>
		/// Builds the mesh from sound datas. Might be improved by using Array instead of List.
		/// </summary>
		/// <returns>Generated mesh.</returns>
		/// <param name="lDatas">L datas.</param>
		public Mesh BuildMeshFromDatasRaw (List<float[]> lDatas) {
			
			int n = lDatas [0].Length;
			int N = lDatas.Count;
			
			int vn = 3 * n;
			int vN = N - 2;
			
			List<Vector3> allVertices = new List<Vector3> ();
			
			for (int j = 0; j < N; j++) {
				for (int i = 0; i < n; i++) {

					int index = allVertices.Count;

					if (i == 0) {
						allVertices.Add(new Vector3(i - 1/3f, lDatas[j][i] * this.heightMult, j * this.step));
					}
					else {
						allVertices.Add((2f * new Vector3(i, lDatas[j][i] * this.heightMult, j * this.step) + new Vector3(i - 1f, lDatas[j][i - 1] * this.heightMult, j * this.step)) / 3f);
					}
					
					allVertices.Add(new Vector3(i, lDatas[j][i] * this.heightMult, j * this.step));

					if (i == n - 1) {
						allVertices.Add(new Vector3(i + 1/3f, lDatas[j][i] * this.heightMult, j * this.step));
					}
					else {
						allVertices.Add((2f * new Vector3(i, lDatas[j][i] * this.heightMult, j * this.step) + new Vector3(i + 1f, lDatas[j][i + 1] * this.heightMult, j * this.step)) / 3f);
					}
					
					allVertices[index + 1] = (allVertices[index] + allVertices[index + 1] + allVertices[index + 2]) / 3f;
				}
			}

			Vector3[] normals = new Vector3[vN * vn];

			for (int j = 0; j < vN; j++) {
				for (int i = 0; i < vn; i++) {
					if ((i == 0) || (i == vn - 1)) {
						normals[j * vn + i] = Vector3.up;
					}
					else {
						Vector3 North = allVertices[j * vn + i + 2 * vn] - allVertices[j * vn + i + vn];
						Vector3 South = allVertices[j * vn + i] - allVertices[j * vn + i + vn];
						Vector3 East = allVertices[j * vn + i + 1] - allVertices[j * vn + i + vn];
						Vector3 West = allVertices[j * vn + i - 1] - allVertices[j * vn + i + vn];

						normals[j * vn + i] = (Vector3.Cross (North, East) + Vector3.Cross (South, West)).normalized;
					}
				}
			}

			Vector2[] uv = new Vector2[vN * vn];
			
			for (int j = 0; j < vN; j++) {
				for (int i = 0; i < vn; i++) {
					uv[j * vn + i] = new Vector2 ((float) i / (vn - 1), (float) j / (vN - 1));
				}
			}

			List<int> trianglesList = new List<int> ();
			
			for (int j = 0; j < vN - 1; j++) {
				for (int i = 0; i < vn - 1; i++) {
					trianglesList.Add(j * vn + i);
					trianglesList.Add((j + 1) * vn + i);
					trianglesList.Add((j + 1) * vn + (i + 1));

					trianglesList.Add(j * vn + i);
					trianglesList.Add((j + 1) * vn + (i + 1));
					trianglesList.Add(j * vn + (i + 1));
				}
			}

			Vector3[] vertices = new Vector3[allVertices.Count - 2 * vn];

			for (int i = 0; i < vertices.Length; i++) {
				vertices[i] = allVertices[i + vn];
			}
			
			Mesh m = new Mesh ();
			
			m.vertices = vertices;
			m.triangles = trianglesList.ToArray ();
			m.uv = uv;
			m.normals = normals;
			
			return m;
		}
	}
}