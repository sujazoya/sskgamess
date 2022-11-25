// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/HeightToNormalMap"{
	Properties{}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "SSEffectShaderHelperStuff.cginc"
			

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
			
			sampler2D _Previous;
			float Size;
			float Height;
			float Channel;
			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				float4 OldColor2 = tex2D(_Previous,i.uv+float2(Size,0));
				float4 OldColor3 = tex2D(_Previous,i.uv+float2(0,Size));
				
				float4 normals = 1;
				if (Channel==0)
					normals = float4(normalize(float3(OldColor.x-OldColor2.x,OldColor.x-OldColor3.x,1))*Height,OldColor.a);
				if (Channel==1)
					normals = float4(normalize(float3(OldColor.y-OldColor2.y,OldColor.y-OldColor3.y,1))*Height,OldColor.a);
				if (Channel==2)
					normals = float4(normalize(float3(OldColor.z-OldColor2.z,OldColor.z-OldColor3.z,1))*Height,OldColor.a);
				if (Channel==3)
					normals = float4(normalize(float3(OldColor.w-OldColor2.w,OldColor.w-OldColor3.w,1))*Height,OldColor.a);
				
				return DoAlphaModeStuff(OldColor,normals); 
			}
			ENDCG
		}
	}
}
