// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/ConvertToRGB"{
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
			float RGBData;
			float3 hsv2rgb2(float3 c){
				c.r = frac(c.r);
				c.gb = saturate(c.gb);
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				float3 pp = c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
				return pp;
			}

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				float4 rgbconv = float4(hsv2rgb2(OldColor.rgb),OldColor.a);
				if (RGBData==0)
					rgbconv.rgb = rgbconv.rgb;
				if (RGBData==1)
					rgbconv.rgb = rgbconv.rrr;
				if (RGBData==2)
					rgbconv.rgb = rgbconv.ggg;
				if (RGBData==3)
					rgbconv.rgb = rgbconv.bbb;
				return DoAlphaModeStuff(OldColor,rgbconv);
			}
			ENDCG
		}
	}
}
