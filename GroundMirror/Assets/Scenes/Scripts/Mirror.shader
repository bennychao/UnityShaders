// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "FX/MirrorReflection"

{

	Properties

	{

		_MainTex("Base (RGB)", 2D) = "white" {}

		[HideInInspector] _ReflectionTex("", 2D) = "white" {}

	}

		SubShader

	{

		Tags { "RenderType" = "Opaque" }

		LOD 100



		Pass {

			CGPROGRAM

			#pragma vertex vert

			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f

			{

				float2 uv : TEXCOORD0;

				float4 refl : TEXCOORD1;

				float4 pos : SV_POSITION;

			};

			inline float4 ComputeNonStereoScreenPos1(float4 pos) {
				float4 o = pos * 0.5f;
				o.xy = float2(o.x, o.y * -1) + o.w;
				o.zw = pos.zw;
				return o;
			}

			inline float4 ComputeScreenPos1(float4 pos, float d) {
				float4 o = pos *0.5f;
				//o.xy = float2(o.x / d, o.y / d * _ProjectionParams.x) + 0.5f;
				o.xy = float2(o.x / o.w, o.y / o.w * -1) + 0.5f;
				o.zw = pos.zw;
				return o;
			}
			float4 _MainTex_ST;

			v2f vert(float4 pos : POSITION, float2 uv : TEXCOORD0)

			{

				v2f o;

				o.pos = UnityObjectToClipPos(pos);

				o.uv = TRANSFORM_TEX(uv, _MainTex);

				//o.refl = ComputeScreenPos(o.pos);
				o.refl = ComputeNonStereoScreenPos1(o.pos);
				//o.refl = ComputeScreenPos1(o.pos, o.pos.w);

				return o;

			}

			sampler2D _MainTex;

			sampler2D _ReflectionTex;

			fixed4 frag(v2f i) : SV_Target

			{

				fixed4 tex = tex2D(_MainTex, i.uv);

				//fixed4 refl = tex2D(_ReflectionTex, i.refl.xy);
				fixed4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(i.refl));

				return tex * refl;
				//return i.refl;
			}

			ENDCG

		}

	}

}
