Shader "Space/Terrain" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 0.5)

		// [Toggle(CULLSPHERE_ON)] _CullSphereOn ("Cull Sphere On", Float) = 0
		// _CullSphereCenter ("Cull Sphere Center", Vector) = (0, 1000, 0, 1)
		// _CullSphereRadius ("Cull Sphere Radius", Float) = 100
	}


	SubShader {

		Tags { "RenderType"="Opaque" }

		CGPROGRAM

		#pragma multi_compile _ CULLSPHERE_ON
		#pragma surface surf Standard addshadow fullforwardshadows

		#include "UnityCG.cginc"

		sampler2D _MainTex;
    fixed4 _Color;

		float _CullSphereEnabled;

		float4 _CullSphereCenter;
		float _CullSphereRadius;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf(Input i, inout SurfaceOutputStandard o) {
			// if (_CullSphereEnabled > 0 && length(i.worldPos - _CullSphereCenter.xyz) < _CullSphereRadius) {
			// 	o.Alpha = 0;
			// }
			// else {
			// 	o.Alpha = 1;
			// }
			o.Albedo = _Color.rgb * tex2D(_MainTex, i.uv_MainTex).rgb;
		}

		ENDCG

	}
}
