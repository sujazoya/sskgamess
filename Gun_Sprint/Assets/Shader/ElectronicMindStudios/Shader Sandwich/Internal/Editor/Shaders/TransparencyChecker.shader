// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/TransparencyChecker"{
	Properties{}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#pragma multi_compile __ UNITY_COLORSPACE_GAMMA

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
			float _EndTag;
			
			float _AlphaMode;
			

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				#ifndef UNITY_COLORSPACE_GAMMA
				if (_EndTag<2)
					OldColor.rgb = pow(max(OldColor.rgb,0),1/2.2);
				#endif
				if (_EndTag>=2)
					return float4(OldColor.rrr,1);
				
				float4 checker1 = float4(1,1,1,1);
				float4 checker2 = float4(0.87,0.87,0.87,1);
				i.uv *= 70;
				float ArrayPosX = i.uv.x;//i%rect.x;
				float ArrayPosY = i.uv.y;//Mathf.Floor(i/rect.x);
				//float total = Mathf.Floor(ArrayPosX*8f) +Mathf.Floor(ArrayPosY*8f);
				//bool isEven = ((ArrayPosX/8f)%2.0f)<1f||(((ArrayPosY+8f)/8f)%2.0f)<1f;
				bool isEven = (((ArrayPosX+floor(ArrayPosY/8)*8)/8)%2.0)<1;
				
				float4 checker = (isEven)? checker1:checker2;
				return float4(checker.rgb*(1-OldColor.a)+OldColor.rgb,1);
			}
			ENDCG
		}
	}
}
