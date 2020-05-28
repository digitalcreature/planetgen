Shader "FX/FresnelGlass" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
    _Color ("Color", Color) = (1, 1, 1, 0.5)
		_FresnelPower ("Fresnel Power", Float) = 5
    _AlphaFresnel ("Alpha Fresnel", Float) = 1
    _SpecularFresnel ("Specular Fresnel", Float) = 1
    _SmoothnessFresnel ("Smoothness Fresnel", Float) = 1
		_Specular ("Specular", Color) = (1, 1, 1, 1)
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		_ShadowAlpha ("Shadow Alpha", Range(0, 1)) = 0.5
	}

	SubShader {
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
		LOD 100

		ZWrite On

		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

		#pragma surface surf StandardSpecular alpha

		#include "UnityCG.cginc"

		sampler2D _MainTex;
    fixed4 _Color;
		float _FresnelPower;
		float _AlphaFresnel;
		float _SpecularFresnel;
		float _SmoothnessFresnel;
		fixed4 _Specular;
		fixed _Smoothness;

		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
			float3 worldPos;
		};

		void surf(Input i, inout SurfaceOutputStandardSpecular o) {
      float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
			float fresnel = 1 - saturate(dot(viewDir, i.worldNormal));
      fresnel = pow(fresnel, _FresnelPower);

			fixed4 color = tex2D(_MainTex, i.uv_MainTex) * _Color;

			o.Albedo = color.rgb;
      o.Alpha = clamp(color.a + fresnel * _AlphaFresnel, 0, 1);
			o.Specular = clamp(_Specular.rgb + fresnel * _SpecularFresnel, 0, 1);
			o.Smoothness = clamp(_Smoothness + fresnel * _SmoothnessFresnel, 0, 1);

		}

		half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) {
	    half NdotL = dot (s.Normal, lightDir);
	    half diff = NdotL * 0.5 + 0.5;
	    half4 c;
	    c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
	    c.a = s.Alpha;
	    return c;
		}

		ENDCG

		Pass {

			Name "SHADOWCASTER"

			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On
			ZTest Less
			Cull Off
			Offset 1, 1

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma target 3.0

			// #pragma multi_compile_shadowcaster
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _ShadowAlpha;

			sampler3D _DitherMaskLOD;

			struct appdata {
				float4 position : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				#if defined(SHADOWS_CUBE)
					float3 lightVec : TEXCOORD1;
				#endif
			};

			v2f vert(appdata v, out float4 pos : SV_POSITION) {
				v2f i;
				#if defined(SHADOWS_CUBE)
					pos = UnityObjectToClipPos(v.position);
					i.lightVec =
						mul(unity_ObjectToWorld, v.position).xyz - _LightPositionRange.xyz;
				#else
					pos = UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
					pos = UnityApplyLinearShadowBias(pos);
				#endif

				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return i;
			}

			float4 frag(v2f i, UNITY_VPOS_TYPE vpos : VPOS) : SV_TARGET {
				float alpha = _ShadowAlpha;
				alpha *= tex2D(_MainTex, i.uv).a;
				float dither =
					tex3D(_DitherMaskLOD, float3(vpos.xy * 0.25, alpha * 0.9375)).a;
				clip(dither - 0.01);

				#if defined(SHADOWS_CUBE)
					float depth = length(i.lightVec) + unity_LightShadowBias.x;
					depth *= _LightPositionRange.w;
					return UnityEncodeCubeShadowDepth(depth);
				#else
					return 0;
				#endif
			}

			ENDCG

		}

	}
}
