// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/PoissonBlurNoWeighting"{
	Properties{}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
	static float2 Poisson3Points[3]={
		float2(0.5724562f, 0.6826031f),
		float2(0.01501155f, -0.915989f),
		float2(-0.8965766f, -0.06154215f)
	};
	static float2 Poisson4Points[4]={
		float2(0.02476075f, -0.7779286f),
		float2(0.3943906f, 0.6101655f),
		float2(-0.8852497f, 0.1672804f),
		float2(0.8505912f, -0.3281441f)
	};
	static float2 Poisson5Points[5]={
		float2(-0.7957063f, 0.3076356f),
		float2(0.09538598f, 0.8763882f),
		float2(0.229684f, -0.6201293f),
		float2(-0.7097354f, -0.4950111f),
		float2(0.3737206f, 0.1264346f)
	};
	static float2 Poisson10Points[10]={
		float2(0.3361086f, -0.7454419f),
		float2(-0.3396147f, -0.232551f),
		float2(0.1044865f, 0.1541135f),
		float2(0.7479108f, -0.1854571f),
		float2(-0.4358546f, -0.8037816f),
		float2(-0.5230139f, 0.4908966f),
		float2(-0.9737176f, 0.2253013f),
		float2(0.01432822f, 0.6359013f),
		float2(-0.8122727f, -0.4197346f),
		float2(0.656606f, 0.6030211f)
	};
	static float2 Poisson15Points[15]={
		float2(-0.3014517f, 0.4104693f),
		float2(0.122612f, 0.3694764f),
		float2(-0.5772369f, 0.7581095f),
		float2(0.1277625f, -0.2439991f),
		float2(-0.2503848f, -0.02185887f),
		float2(0.04606917f, 0.9134792f),
		float2(-0.7149873f, 0.1212395f),
		float2(-0.3735067f, -0.4221394f),
		float2(-0.7762218f, -0.4957251f),
		float2(-0.4232179f, -0.8786449f),
		float2(0.591468f, -0.6616971f),
		float2(0.7475024f, -0.1394626f),
		float2(0.1937208f, -0.8706161f),
		float2(0.6234847f, 0.304923f),
		float2(0.5161396f, 0.8536832f)
	};
	static float2 Poisson20Points[20]={
		float2(-0.5420936f, -0.06769659f),
		float2(-0.5578253f, -0.583697f),
		float2(-0.02640542f, -0.5568427f),
		float2(-0.9909379f, -0.115223f),
		float2(-0.758519f, 0.2848054f),
		float2(-0.2363301f, 0.288436f),
		float2(-0.103763f, -0.08932879f),
		float2(-0.6585842f, 0.6582814f),
		float2(0.5345013f, -0.4915428f),
		float2(0.2108035f, 0.5521463f),
		float2(0.533279f, -0.04395336f),
		float2(0.9272754f, 0.1125228f),
		float2(0.6202674f, 0.4349916f),
		float2(0.8812137f, -0.3336655f),
		float2(0.2979943f, -0.9476216f),
		float2(0.1870839f, 0.1591433f),
		float2(-0.2876338f, -0.9391012f),
		float2(-0.1694376f, 0.688916f),
		float2(0.5204541f, 0.8427336f),
		float2(0.1538422f, 0.9681873f)
	};
	static float2 Poisson25Points[25]={
		float2(0.2080095f, -0.01085258f),
		float2(0.2431835f, -0.6473321f),
		float2(-0.2630148f, -0.03314619f),
		float2(0.706383f, -0.01909894f),
		float2(0.2649457f, 0.319762f),
		float2(-0.1366887f, -0.5147343f),
		float2(0.6449754f, -0.4467505f),
		float2(-0.06845222f, 0.3660467f),
		float2(0.677255f, 0.3592614f),
		float2(-0.4845913f, -0.6542463f),
		float2(-0.5686948f, -0.1560421f),
		float2(-0.2901484f, -0.946431f),
		float2(0.06065856f, -0.9272438f),
		float2(0.6195858f, -0.7732947f),
		float2(0.9591783f, -0.2774893f),
		float2(0.9708589f, 0.205323f),
		float2(0.7125145f, 0.6999037f),
		float2(0.2239271f, 0.6519736f),
		float2(-0.2834446f, 0.6219321f),
		float2(-0.1253776f, 0.9499886f),
		float2(-0.4140132f, 0.3006884f),
		float2(-0.6702861f, 0.522322f),
		float2(-0.709277f, 0.151078f),
		float2(0.1577622f, -0.335809f),
		float2(-0.8429466f, -0.3235372f)
	};
	static float2 Poisson30Points[30]={
		float2(0.5011483f, 0.04521868f),
		float2(0.1465597f, 0.08051669f),
		float2(0.4249199f, -0.3267138f),
		float2(0.4148951f, 0.3668412f),
		float2(0.8471819f, -0.3454683f),
		float2(0.1121189f, -0.2301473f),
		float2(0.7479299f, 0.2963868f),
		float2(-0.02526122f, 0.3329114f),
		float2(-0.2069537f, 0.06547579f),
		float2(-0.2425406f, -0.2733826f),
		float2(0.057284f, 0.6228352f),
		float2(-0.3816113f, 0.3154401f),
		float2(0.7328869f, 0.6222063f),
		float2(0.4785986f, 0.7927536f),
		float2(0.904448f, 0.02076665f),
		float2(-0.4010861f, 0.8992082f),
		float2(-0.5566451f, 0.04093836f),
		float2(-0.9160525f, 0.2224667f),
		float2(-0.7013909f, 0.5131071f),
		float2(-0.2446982f, 0.6357487f),
		float2(0.1219466f, 0.9327087f),
		float2(-0.9268347f, -0.2508305f),
		float2(-0.5423443f, -0.3187264f),
		float2(-0.7674371f, -0.520185f),
		float2(0.4785696f, -0.6505225f),
		float2(0.1735249f, -0.566444f),
		float2(-0.247582f, -0.7004845f),
		float2(-0.063352f, -0.9795408f),
		float2(-0.5582952f, -0.776435f),
		float2(0.276051f, -0.8835434f)
	};
	static float2 Poisson40Points[40]={
		float2(0.7256591f, -0.1042184f),
		float2(0.4492062f, -0.4431369f),
		float2(0.7055052f, -0.3740624f),
		float2(0.5323756f, 0.3323394f),
		float2(0.9846258f, -0.1362978f),
		float2(0.7877364f, 0.3699379f),
		float2(0.2854357f, 0.05240316f),
		float2(0.9336613f, 0.1215896f),
		float2(0.2809109f, -0.2491614f),
		float2(0.3638776f, 0.6816204f),
		float2(0.08544681f, 0.2493392f),
		float2(0.6221807f, 0.7307564f),
		float2(0.287935f, 0.4152373f),
		float2(0.0205787f, -0.2755264f),
		float2(0.1613114f, -0.6803955f),
		float2(-0.1412761f, -0.4816247f),
		float2(-0.04645349f, 0.0317595f),
		float2(-0.07534885f, 0.4957601f),
		float2(-0.2535167f, 0.2566531f),
		float2(0.1584422f, 0.8409653f),
		float2(-0.1671611f, 0.7797599f),
		float2(-0.4729695f, -0.3225064f),
		float2(-0.09659555f, -0.732605f),
		float2(-0.6943507f, 0.1025151f),
		float2(-0.631149f, 0.5491853f),
		float2(-0.3866712f, 0.02117324f),
		float2(-0.3471077f, 0.4888262f),
		float2(-0.4647051f, -0.800525f),
		float2(-0.2439562f, -0.2132379f),
		float2(0.209526f, -0.9264317f),
		float2(-0.1374217f, -0.9810445f),
		float2(-0.8579679f, -0.1124868f),
		float2(-0.8993123f, 0.3334761f),
		float2(0.6261228f, -0.655448f),
		float2(0.4670109f, -0.8765142f),
		float2(-0.4122593f, 0.8613442f),
		float2(-0.6350462f, -0.5527102f),
		float2(-0.8742716f, -0.3821971f),
		float2(-0.5496528f, 0.3086437f),
		float2(0.5342344f, 0.0770176f)
	};
	static float2 Poisson50Points[50]={
		float2(0.01191647f, -0.3766487f),
		float2(0.06530184f, -0.06239028f),
		float2(-0.1811882f, -0.5388901f),
		float2(0.1055802f, -0.710745f),
		float2(0.3280199f, -0.2674491f),
		float2(-0.193547f, -0.2725269f),
		float2(0.2221897f, -0.4697671f),
		float2(-0.1753743f, 0.002348582f),
		float2(-0.151774f, -0.77955f),
		float2(-0.3858232f, -0.4307106f),
		float2(0.354765f, -0.8052159f),
		float2(0.4152598f, -0.5817062f),
		float2(-0.05505833f, -0.9784528f),
		float2(0.08306476f, 0.2260627f),
		float2(-0.5666506f, 0.05787798f),
		float2(-0.1841481f, 0.3785738f),
		float2(-0.4438635f, -0.2119619f),
		float2(-0.4176045f, 0.3660236f),
		float2(0.5945581f, -0.7927471f),
		float2(0.6253349f, -0.3254496f),
		float2(0.6937959f, -0.5451365f),
		float2(0.1498304f, 0.4665151f),
		float2(-0.4727286f, 0.6839205f),
		float2(-0.2536181f, 0.7588404f),
		float2(-0.0238042f, 0.6838953f),
		float2(0.7665393f, -0.125378f),
		float2(0.8741308f, -0.4184473f),
		float2(0.2488604f, 0.06121285f),
		float2(0.4464028f, -0.05704774f),
		float2(-0.3928106f, -0.8157101f),
		float2(0.1660396f, -0.9463302f),
		float2(-0.5493619f, -0.5980057f),
		float2(-0.7476863f, -0.4808375f),
		float2(0.4309874f, 0.3841074f),
		float2(0.6658426f, 0.186349f),
		float2(0.7253714f, 0.3993621f),
		float2(0.3989984f, 0.6901026f),
		float2(0.6660151f, 0.7267773f),
		float2(-0.67133f, -0.1480678f),
		float2(0.9698753f, 0.1371034f),
		float2(0.2569785f, 0.884242f),
		float2(-0.05084391f, 0.9498577f),
		float2(-0.6316729f, 0.3139676f),
		float2(-0.6975635f, 0.6446213f),
		float2(-0.8742073f, -0.2880245f),
		float2(0.9943928f, -0.08217984f),
		float2(-0.8804912f, 0.3471478f),
		float2(-0.919705f, -0.05283951f),
		float2(-0.7785192f, 0.1181058f),
		float2(-0.3565525f, 0.1414825f)
	};
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
			float Contrast;
			float Blur_X;
			float Blur_Y;
			float Quality;
			float Reduce_Banding;
			float Animate_Noise;
			float4 _SSTime;
			half2 PoissonBlurRot( half2 d, float r ) {
				half2 sc = half2(sin(r),cos(r));
				return half2( dot( d, half2(sc.y, -sc.x) ), dot( d, sc.xy ) );
			}
			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				
				float4 newColor = 0;
				float2 Blur = float2(Blur_X,Blur_Y);
				
		half PoissonBlurRandRotation = 6.28*frac(sin(dot(i.uv.xy, float2(12.9898, 78.233)))* 43758.5453);
		half PoissonBlurRandRotationAnimated = 6.28*frac(sin(dot(i.uv.xy, float2(12.9898, 78.233))+_SSTime.x)* 43758.5453);
		half4 PoissonBlurRotation = half4( PoissonBlurRot(half2(1,0),PoissonBlurRandRotation), PoissonBlurRot(half2(0,1),PoissonBlurRandRotation));
		half4 PoissonBlurRotationAnimated = half4( PoissonBlurRot(half2(1,0),PoissonBlurRandRotationAnimated), PoissonBlurRot(half2(0,1),PoissonBlurRandRotationAnimated));
		
		if (Reduce_Banding==1){
			if (Animate_Noise==0)
				PoissonBlurRotationAnimated = PoissonBlurRotation;
		}else{
			PoissonBlurRotationAnimated = float4(1,0,0,1);
		}
		
				if (Quality>=50){
					[unroll]
					for(float ii = 0; ii < 50; ii += 1){
						newColor+=tex2Dlod(_Previous,float4(i.uv+float2(dot(Poisson50Points[ii],PoissonBlurRotationAnimated.xz),dot(Poisson50Points[ii],PoissonBlurRotationAnimated.yw))*Blur,0,0))*(1.0/50.0);
					}
				}else if (Quality>=40){
					[unroll]
					for(float ii = 0; ii < 40; ii += 1){
						newColor+=tex2Dlod(_Previous,float4(i.uv+float2(dot(Poisson40Points[ii],PoissonBlurRotationAnimated.xz),dot(Poisson40Points[ii],PoissonBlurRotationAnimated.yw))*Blur,0,0))*(1.0/40.0);
					}
				}else if (Quality>=30){
					[unroll]
					for(float ii = 0; ii < 30; ii += 1){
						newColor+=tex2Dlod(_Previous,float4(i.uv+float2(dot(Poisson30Points[ii],PoissonBlurRotationAnimated.xz),dot(Poisson30Points[ii],PoissonBlurRotationAnimated.yw))*Blur,0,0))*(1.0/30.0);
					}
				}else if (Quality>=25){
					[unroll]
					for(float ii = 0; ii < 25; ii += 1){
						newColor+=tex2Dlod(_Previous,float4(i.uv+float2(dot(Poisson25Points[ii],PoissonBlurRotationAnimated.xz),dot(Poisson25Points[ii],PoissonBlurRotationAnimated.yw))*Blur,0,0))*(1.0/25.0);
					}
				}else if (Quality>=20){
					[unroll]
					for(float ii = 0; ii < 20; ii += 1){
						newColor+=tex2Dlod(_Previous,float4(i.uv+float2(dot(Poisson20Points[ii],PoissonBlurRotationAnimated.xz),dot(Poisson20Points[ii],PoissonBlurRotationAnimated.yw))*Blur,0,0))*(1.0/20.0);
					}
				}else if (Quality>=15){
					[unroll]
					for(float ii = 0; ii < 15; ii += 1){
						newColor+=tex2Dlod(_Previous,float4(i.uv+float2(dot(Poisson15Points[ii],PoissonBlurRotationAnimated.xz),dot(Poisson15Points[ii],PoissonBlurRotationAnimated.yw))*Blur,0,0))*(1.0/15.0);
					}
				}else if (Quality>=10){
					[unroll]
					for(float ii = 0; ii < 10; ii += 1){
						newColor+=tex2Dlod(_Previous,float4(i.uv+float2(dot(Poisson10Points[ii],PoissonBlurRotationAnimated.xz),dot(Poisson10Points[ii],PoissonBlurRotationAnimated.yw))*Blur,0,0))*(1.0/10.0);
					}
				}else if (Quality>=5){
					[unroll]
					for(float ii = 0; ii < 5; ii += 1){
						newColor+=tex2Dlod(_Previous,float4(i.uv+float2(dot(Poisson5Points[ii],PoissonBlurRotationAnimated.xz),dot(Poisson5Points[ii],PoissonBlurRotationAnimated.yw))*Blur,0,0))*(1.0/5.0);
					}
				}else if (Quality>=4){
					[unroll]
					for(float ii = 0; ii < 4; ii += 1){
						newColor+=tex2Dlod(_Previous,float4(i.uv+float2(dot(Poisson4Points[ii],PoissonBlurRotationAnimated.xz),dot(Poisson4Points[ii],PoissonBlurRotationAnimated.yw))*Blur,0,0))*(1.0/4.0);
					}
				}else if (Quality>=3){
					[unroll]
					for(float ii = 0; ii < 3; ii += 1){
						newColor+=tex2D(_Previous,i.uv+float2(dot(Poisson3Points[ii],PoissonBlurRotationAnimated.xz),dot(Poisson3Points[ii],PoissonBlurRotationAnimated.yw))*Blur)*(1.0/3.0);
					}
				}
				
				return newColor;//DoAlphaModeStuff(OldColor,newColor);
			}

			ENDCG
		}
	}
}
