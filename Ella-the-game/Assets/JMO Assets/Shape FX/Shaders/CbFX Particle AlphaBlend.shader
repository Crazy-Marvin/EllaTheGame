//========================================
//
// SHAPE FX
// (c) 2016 - Jean Moreno
// http://www.jeanmoreno.com/unity
//
//========================================

Shader "ShapeFX/Particles/Alpha Blended"
{
	Properties
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
		[Toggle(SHFX_NOTEXTURE)] _NoTexture ("No Texture", Float) = 0
		[Toggle(SHFX_DISABLE_SOFT_PARTICLES)] _NoSoftParticles ("Disable Soft Particles", Float) = 0
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	}

	Category
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		Cull Off
		Lighting Off
		ZWrite Off
		
		SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#pragma shader_feature SHFX_DISABLE_SOFT_PARTICLES
				#pragma shader_feature SHFX_NOTEXTURE
				
				#include "UnityCG.cginc"
				
				//Base
				sampler2D _MainTex;
				float4 _MainTex_ST;
				
				//Soft Particles
				sampler2D_float _CameraDepthTexture;
				float _InvFade;
				
				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};
				
				struct v2f
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
			#if SOFTPARTICLES_ON && !SHFX_DISABLE_SOFT_PARTICLES
					float4 projPos : TEXCOORD2;
			#endif
				};
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
			#if SOFTPARTICLES_ON && !SHFX_DISABLE_SOFT_PARTICLES
					o.projPos = ComputeScreenPos (o.vertex);
					COMPUTE_EYEDEPTH(o.projPos.z);
			#endif
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
			#if SOFTPARTICLES_ON && !SHFX_DISABLE_SOFT_PARTICLES
					float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
					float partZ = i.projPos.z;
					float fade = saturate (_InvFade * (sceneZ-partZ));
					i.color.a *= fade;
			#endif
					
					fixed4 col = i.color;
			#if !SHFX_NOTEXTURE
					col.a *= tex2D(_MainTex, i.texcoord).a;
			#endif
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
}
