Shader "AudioTest/ToonTerrain" {
	Properties {
		_GrassTex ("Grass Texture", 2D) = "white" {}
		_CliffTex ("Cliff Texture", 2D) = "white" {}
		_CliffThreshold ("Cliff Threshold", Float) = 0.9
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _GrassTex;
		sampler2D _CliffTex;
		float _CliffThreshold;

		struct Input {
			float2 uv_GrassTex;
			float3 worldNormal;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = (0,0,0,0);
			if (IN.worldNormal.y < _CliffThreshold) {
				c = tex2D(_CliffTex, IN.uv_GrassTex);
			}
			else  {
				c = tex2D(_GrassTex, IN.uv_GrassTex);
			}
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}