Shader "Vehicle/ThrusterPlume" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Glow ("Glow", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}

		// ZTest Always
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
      float4 _MainTex_ST;

			float _Glow;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			half4 frag(v2f i) : SV_Target {
				half4 color = tex2D(_MainTex, i.uv);
				return float4(color.xyz * _Glow, color.w);
			}
			ENDCG
		}
	}
}
