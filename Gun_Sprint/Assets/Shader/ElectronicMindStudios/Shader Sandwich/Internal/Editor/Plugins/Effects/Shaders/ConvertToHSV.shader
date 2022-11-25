// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/ConvertToHSV"{
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
			float HSVData;
			float3 rgb2hsv2(float3 c){
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
				float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				float4 hsvconv = float4(rgb2hsv2(OldColor.rgb),OldColor.a);
				if (HSVData==0)
					hsvconv.rgb = hsvconv.rgb;
				if (HSVData==1)
					hsvconv.rgb = hsvconv.rrr;
				if (HSVData==2)
					hsvconv.rgb = hsvconv.ggg;
				if (HSVData==3)
					hsvconv.rgb = hsvconv.bbb;
				return DoAlphaModeStuff(OldColor,hsvconv);
			}
			ENDCG
		}
	}
}
