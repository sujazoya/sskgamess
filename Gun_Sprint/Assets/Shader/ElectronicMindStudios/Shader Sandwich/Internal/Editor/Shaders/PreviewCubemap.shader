// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/ReadCubemap"{
	Properties{}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v){
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _UVs;
			samplerCUBE Cubemap;
			half4 Cubemap_HDR;
			float HDR_Support;
			float Face;

			float4 frag (v2f i) : SV_Target{
				float4 UV = float4(i.uv,0,0);
				UV.xy = UV.xy*2-1;
				UV.y *= -1;
				if (Face==0)
					UV.xyz = normalize(float3(1,UV.y,-UV.x));//+0.5;//PosX
				if (Face==4)
					UV.xyz = normalize(float3(UV.x,UV.y,1));//+0.5;//PosZ
				if (Face==2)
					UV.xyz = normalize(float3(UV.x,1,-UV.y));//+0.5;//PosY
				if (Face==1)
					UV.xyz = normalize(float3(-1,UV.y,UV.x));//+0.5;//NegX
				if (Face==5)
					UV.xyz = normalize(float3(-UV.x,UV.y,-1));//+0.5;//NegZ
				if (Face==3)
					UV.xyz = normalize(float3(UV.x,-1,UV.y));//+0.5;//NegY
				
				float4 PixCol = texCUBEbias(Cubemap,float4(UV.xyz,-1));
				
				//if (HDR_Support==1)
					PixCol = float4(DecodeHDR (PixCol, Cubemap_HDR),PixCol.a);
				if (IsGammaSpace())
					return pow(PixCol,2.2);//float4(1,0,0,1);
				else
					return PixCol*2;//float4(1,0,0,1);
			}
			ENDCG
		}
	}
}
