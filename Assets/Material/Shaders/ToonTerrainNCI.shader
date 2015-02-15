Shader "AudioTest/ToonTerrainNCI" {
	Properties {
		_GrassColor ("Grass Color", Color) = (0,0,0,0)
		_CliffColor ("Cliff Color", Color) = (0,0,0,0)
		_CliffThreshold ("Cliff Threshold", Float) = 0.9
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _GrassColor;
		fixed4 _CliffColor;
		float _CliffThreshold;

		struct Input {
			float2 uv_GrassTex;
			float3 worldNormal;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = (0,0,0,0);
			if (IN.worldNormal.y > _CliffThreshold) {
				c = _GrassColor;
			}
			else  {
				c = _CliffColor;
			}
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}