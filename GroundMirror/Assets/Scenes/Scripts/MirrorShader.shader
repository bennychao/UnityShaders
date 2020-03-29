Shader "Unlit/MirrorShader"
{
	Properties
	{
		_LeftEyeTexture("Left Eye Texture", 2D) = "white" {}

		_matrix01("matrix01", Vector) = (0,0,0,0)
		_matrix02("matrix02", Vector) = (0,0,0,0)
		_matrix03("matrix03", Vector) = (0,0,0,0)
		_matrix04("matrix04", Vector) = (0,0,0,0)

		_depth("depth", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _LeftEyeTexture;
			sampler2D _RightEyeTexture;

			uniform float4 _matrix01;
			uniform float4 _matrix02;
			uniform float4 _matrix03;
			uniform float4 _matrix04;
			uniform float _depth;
			
			inline float4 ComputeScreenPos1(float4 pos, float d) {
				float4 o = pos * 0.5f;
				o.xy = float2(o.x / d, o.y / d * -1) + 0.5f;
				//o.xy = float2(o.x / o.w, o.y / o.w * 1) + 0.5f;
				//o.zw = pos.zw;
				return o;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float4x4 mirrorVPMat = {
					_matrix01,
					_matrix02,
					_matrix03,
					_matrix04
				};

				float4 objPos = mul(unity_ObjectToWorld, v.vertex);
				//objPos = mul(UNITY_MATRIX_V, objPos);		//UNITY_MATRIX_V
				float4 projectPos = mul(mirrorVPMat, objPos);		//UNITY_MATRIX_P
																	//projectPos = mul(mirrorVPMat, projectPos);
																	//o.vertex = projectPos;
																	//o.uv = float2(projectPos.x / projectPos.w, projectPos.y / projectPos .w* 1);
				o.uv = ComputeScreenPos1(projectPos, projectPos.w);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;

				col = tex2D(_LeftEyeTexture, i.uv);

				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
