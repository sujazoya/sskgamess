// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Add"{
	Properties{}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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
			
			sampler2D _Previous;
			sampler2D _New;
			float _UseMask;
			sampler2D _Mask;
			float _Amount;
			
			float _AlphaMode;
			

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				float4 NewColor = tex2D(_New,i.uv);
				float4 Mask = tex2D(_Mask,i.uv);
				if (_UseMask==0)
					Mask = 1;
					
				if (_AlphaMode==0)
					return float4(OldColor.rgb+NewColor.rgb*_Amount*Mask,OldColor.a);
				if (_AlphaMode==1)
					return float4(OldColor.rgb+NewColor.rgb*NewColor.a*_Amount*Mask,OldColor.a);
				if (_AlphaMode==2)
					return float4(OldColor.rgb+NewColor.rgb*NewColor.a*_Amount*Mask,OldColor.a);
				if (_AlphaMode==3)
					return lerp(OldColor,float4(OldColor.rgb,0),NewColor.a*_Amount*Mask);
					
				return lerp(OldColor,NewColor+OldColor,_Amount*Mask);
			}
			ENDCG
		}
	}
}
