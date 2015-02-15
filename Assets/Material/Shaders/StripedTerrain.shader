Shader "AudioTest/StripedTerrain" {
	Properties {
		_LineColor ("Line Color", Color) = (0,0,0,0)
		_GroundColor ("Ground Color", Color) = (0,0,0,0)
		_LineSize ("Line Size", Float) = 1
		_GroundSize ("Ground Size", Int) = 10
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _LineColor;
		fixed4 _GroundColor;
		float _LineSize;
		int _GroundSize;

		struct Input {
			float3 worldNormal;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = (0,0,0,0);
			if (IN.worldPos.y - ((int) IN.worldPos.y / _GroundSize) * _GroundSize < _LineSize) {
				c = _LineColor;
			}
			else {
				c = _GroundColor;
			}
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}