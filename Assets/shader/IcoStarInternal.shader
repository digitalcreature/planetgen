Shader "FX/IcoStarInternal" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Glow ("Glow", Float) = 1
		_StarSpeed ("Star Speed", Float) = 1
		_SparkleMap ("Sparkle Map", 2D) = "white" {}
		_SparkleSpeed ("Sparkle Speed", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent+10"}

		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
      float4 _MainTex_ST;

			sampler2D _SparkleMap;
      float4 _SparkleMap_ST;

			float _Glow;

			float _StarSpeed;
			float _SparkleSpeed;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 sparkle_uv : TEXCOORD1;
			};

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex) + float2(0, _Time.y * _StarSpeed);
				o.sparkle_uv = TRANSFORM_TEX(v.uv, _SparkleMap) + float2(_Time.y * _SparkleSpeed, 0);
				return o;
			}

			half4 frag(v2f i) : SV_Target {
				half4 color = tex2D(_MainTex, i.uv);
				half4 sparkle = tex2D(_SparkleMap, i.sparkle_uv);
				return float4(color.xyz * _Glow, color.w * sparkle.x);
			}
			ENDCG
		}
	}
}
