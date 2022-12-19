// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SgLib/Skybox/VerticalGradient" {
	Properties {
		_ColorHigh ("Color Top", Color) = (1,1,1,1)
		_ColorLow ("Color Bottom", Color) = (1,1,1,1)
		_GradientBias ("Gradient Bias", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "Queue"="Background" }

		Pass { 
		
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma vertex vert
			#pragma fragment frag

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			#include "UnityCG.cginc"

			fixed4 _ColorHigh;
			fixed4 _ColorLow;
			fixed _GradientBias;

			float4 vert(appdata_base v) : POSITION {
                return UnityObjectToClipPos (v.vertex);
            }

            fixed4 frag(float4 sp:VPOS) : SV_Target {
            	fixed r = min(_GradientBias, 1 - _GradientBias);

            	#if !SHADER_API_METAL	// non-iOS
            	return lerp(_ColorLow, _ColorHigh, smoothstep(_GradientBias - r, _GradientBias + r, sp.y/_ScreenParams.y));
            	#else	// iOS
            	// A dirty fix for the upside-down color issue on iOS
            	return lerp(_ColorHigh, _ColorLow, smoothstep(_GradientBias - r, _GradientBias + r, sp.y/_ScreenParams.y));
            	#endif
            }
			ENDCG
		}
	}
	Fallback "Unlit/Color"
}
