// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/UselessBlur"{
	Properties{}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
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
			float Blur_X;
			float Blur_Y;
			float Quality;
			float Fast;

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				float4 NewColor = 0;
				float AddCount = 0;
				float mul = 1.0/Quality;
				float ii = 0;
				for(ii = -Quality;ii<0;ii++){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(ii*Blur_X*mul,0),0,0));AddCount+=1;}
				for(ii = 1;ii<=Quality;ii++){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(ii*Blur_X*mul,0),0,0));AddCount+=1;}
				for(ii = -Quality;ii<0;ii++){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(0,ii*Blur_Y*mul),0,0));AddCount+=1;}
				for(ii = 1;ii<=Quality;ii++){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(0,ii*Blur_Y*mul),0,0));AddCount+=1;}
				
				if (Fast==0){
					for(ii = -Quality;ii<0;ii+=2){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(ii*Blur_X*mul,ii*Blur_Y*mul),0,0));AddCount+=1;}
					for(ii = 1;ii<=Quality;ii+=2){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(ii*Blur_X*mul,ii*Blur_Y*mul),0,0));AddCount+=1;}
					for(ii = -Quality;ii<0;ii+=2){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(-ii*Blur_X*mul,-ii*Blur_Y*mul),0,0));AddCount+=1;}
					for(ii = 1;ii<=Quality;ii+=2){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(-ii*Blur_X*mul,-ii*Blur_Y*mul),0,0));AddCount+=1;}
					
					for(ii = -Quality;ii<0;ii+=2){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(-ii*Blur_X*mul,ii*Blur_Y*mul),0,0));AddCount+=1;}
					for(ii = 1;ii<=Quality;ii+=2){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(-ii*Blur_X*mul,ii*Blur_Y*mul),0,0));AddCount+=1;}
					for(ii = -Quality;ii<0;ii+=2){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(ii*Blur_X*mul,-ii*Blur_Y*mul),0,0));AddCount+=1;}
					for(ii = 1;ii<=Quality;ii+=2){NewColor+=tex2Dlod(_Previous,float4(i.uv+float2(ii*Blur_X*mul,-ii*Blur_Y*mul),0,0));AddCount+=1;}
				}
				NewColor/=AddCount;
				return DoAlphaModeStuff(OldColor, NewColor);
			}
			ENDCG
		}
	}
}
