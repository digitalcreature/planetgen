Shader "Space/Terrain Mask Object" {
	Properties {
		_StencilReference ("Mask Reference Value", Int) = 16
	}

	SubShader {

		Tags { "Queue"="Geometry-500" }

		Blend Zero One

		Pass {

			Stencil {
				Ref [_StencilReference]
				Pass Replace
			}

			// CGPROGRAM
			//
			// #pragma vertex vert
			// #pragma fragment frag
			//
			// #include "UnityCG.cginc"
			//
			// float4 vert(float4 pos : POSITION) : SV_POSITION {
			// 	return UnityObjectToClipPos(pos);
			// }
			//
			// fixed4 frag() : SV_TARGET {
			// 	return fixed4(1, 0, 0, 1);
			// }
			// ENDCG

		}

	}
}
