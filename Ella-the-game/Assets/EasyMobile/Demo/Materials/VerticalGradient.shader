// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "SgLib/VerticalGradient" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_ColorUp ("Color Up", Color) = (1,1,1,1)
		_ColorHigh ("Color Top", Color) = (0,0,0,1)
		_ColorLow ("Color Bottom", Color) = (0,0,0,0)
		_yLocalPosHigh ("Local Y Top", Float) = 0
		_yLocalPosLow ("Local Y Bottom", Float) = 0
		_GradientBias ("Gradient Bias", Range(0,1)) = 0.5
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "DisableBatching"="True"}	// we'll work in object space so batching is disabled
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#define UP float3(0,1,0)

		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
			float3 worldPos;
		};

		sampler2D _MainTex;
		fixed4 _ColorUp;
		fixed4 _ColorHigh;
		fixed4 _ColorLow;
		float _yLocalPosHigh;
		float _yLocalPosLow;
		half _GradientBias;
		half _Glossiness;
		half _Metallic;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

			// Albedo also depends on normal
			half nd = saturate(dot(IN.worldNormal, UP));
			float3 localPos = mul(unity_WorldToObject, float4(IN.worldPos, 1)).xyz;	// _World2Object must be 1s parameter of mul
			half r = min(_GradientBias, 1 - _GradientBias);
			fixed4 gradient = lerp(_ColorLow, _ColorHigh, smoothstep(_GradientBias - r, _GradientBias + r, smoothstep(_yLocalPosLow, _yLocalPosHigh, localPos.y)));
			c *= nd * _ColorUp + (1 - nd) * gradient;
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
