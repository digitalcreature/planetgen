Shader "UI/Sketchy" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
		_SketchMap ("SketchMap", 2D) = "gray" {}
		_SketchAmp ("Sketch Amplitude", Float) = 1
		_SketchFreq ("Sketch Frequency", Float) = 1
		_FrameTime ("Frame Time", Float) = 0.1
	}
	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
    }

		Blend SrcAlpha OneMinusSrcAlpha

		Pass {


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;

			sampler2D _SketchMap;
			float4 _SketchMap_ST;

			float _SketchAmp;
			float _SketchFreq;
			float _FrameTime;

			struct appdata {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_sketch : TEXCOORD1;
				float4 color : COLOR;
			};

			v2f vert(appdata v) {
				v2f o;

				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				o.uv_sketch = TRANSFORM_TEX(v.uv, _SketchMap);
				o.color = v.color;

				return o;
			}

			half4 frag(v2f i) : SV_Target {
				float t = ((int) (_Time.y / _FrameTime)) * _FrameTime;
				float3 sketch = tex2D(_SketchMap, i.uv_sketch + (t * _SketchFreq)).xyz;
				sketch = sketch * 2 - 1;
				return tex2D(_MainTex, i.uv + sketch.xy * _SketchAmp) * i.color * _Color;
			}
			ENDCG
		}
	}
}
