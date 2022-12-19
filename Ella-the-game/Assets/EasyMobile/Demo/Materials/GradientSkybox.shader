// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SgLib/Gradient Skybox"
{
	Properties
	{
		_Color("Sky color", Color) = (1,1,1,1)
		_Color2("Ground color", Color) = (1,1,1,1)
		[PowerSlider(1.0)]
		_SkyPos("Sky position", Range(-1.0, 1.0)) = 0.5
		[PowerSlider(1.0)]
		_GroundPos("Ground position", Range(-1.0, 1.0)) = -0.5
	}
	SubShader
	{
		Tags {"Queue"="Background"}
		Pass
		{
			Cull Back
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform float4 _Color;
			uniform float4 _Color2;
			uniform float _SkyPos;
			uniform float _GroundPos;

			struct vInput
			{
				float4 localPos: POSITION;
				float3 texCoord: TEXCOORD0;
			};

			struct vOutput
			{
				float4 clipPos: SV_POSITION;
				float3 texCoord: TEXCOORD0;
			};

			vOutput vert(vInput i)
			{
				vOutput o;
				o.clipPos = UnityObjectToClipPos(i.localPos);
				o.texCoord = i.texCoord;
				return o;
			}

			float4 frag(vOutput i): COLOR
			{
				float f = clamp((i.texCoord.y-_GroundPos)/(_SkyPos-_GroundPos),0,1);
				return lerp(_Color2, _Color, f);
			}

			ENDCG
		}
	}
}