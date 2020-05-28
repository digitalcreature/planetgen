Shader "Space/Terrain Mask Area" {
	Properties {
		_StencilReference ("Mask Reference Value", Int) = 16
	}

	SubShader {

		Tags { "Queue"="Geometry-400" }

		Cull Off

		Blend Zero One

		Pass {

			ZTest Greater
			ZWrite Off

			Stencil {
				Ref [_StencilReference]
				Pass IncrWrap
			}

		}

	}
}
