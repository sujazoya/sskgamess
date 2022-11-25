// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Mix"{
	Properties{}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#pragma target 3.0

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
			float _Transparent;
			float _AlphaMode;
			float _IsPremul;
			float _CreatesPremul;
			float _EndTag;//0=rgba,1=rgb,2=r,3=g,4=b,a=5
			float _BlendMode;//Mix,Add,Subtract,Multiply,Divide,Lighten,Darken,Normals Mix,Dot
			
			
			
	float4 GenerateMixingLerp(float4 Start,float4 End,float4 PremulEnd, float Alpha, float OneMinusAlpha, float Premul){
		if (Premul==1){
			return (Start * OneMinusAlpha + PremulEnd);
		}else{
			return lerp(Start,End,Alpha);
		}
	}
	float4 GenerateMixingLerp(float4 Start,float4 End, float Alpha, float OneMinusAlpha, float Premul){
		return GenerateMixingLerp(Start,End,End,Alpha,OneMinusAlpha,Premul);
	}
			
	float3 GenerateMixingLerpRGB(float3 Start,float3 End,float3 PremulEnd, float Alpha, float OneMinusAlpha, float Premul){
		if (Premul==1){
			return (Start * OneMinusAlpha + PremulEnd);
		}else{
			return lerp(Start,End,Alpha);
		}
	}
	float3 GenerateMixingLerpRGB(float3 Start,float3 End, float Alpha, float OneMinusAlpha, float Premul){
		return GenerateMixingLerpRGB(Start,End,End,Alpha,OneMinusAlpha,Premul);
	}
			
	float GenerateMixingLerpSingle(float Start,float End,float PremulEnd, float Alpha, float OneMinusAlpha, float Premul){
		if (Premul==1){
			return (Start * OneMinusAlpha + PremulEnd);
		}else{
			return lerp(Start,End,Alpha);
		}
	}
	float GenerateMixingLerpSingle(float Start,float End, float Alpha, float OneMinusAlpha, float Premul){
		return GenerateMixingLerpSingle(Start,End,End,Alpha,OneMinusAlpha,Premul);
	}
	
	float4 GenerateMixingCodeRGBA(float4 Dest, float4 Src, float4 BlendedSrcRGBA, float3 BlendedSrcRGB, float AlphaMode, float Alpha, float AlphaNoA, float OneMinusAlpha, float IsPremul, float CreatesPremul){
		//string AlphaNoA = Alpha.Replace( Src + ".a * ", "").Replace( Src + ".a", "");
		//string AlphaA = (Alpha.Contains(Src))?" * "+Src+".a":"";
		
		float OneMinusAlphaNoA = 1-AlphaNoA;//OneMinusAlpha.Replace( Src + ".a * ", "").Replace( Src + ".a", "");
		
		if (AlphaMode == 0){
			//if (Alpha == 1)
			//	return Dest = float4(BlendedSrcRGB,1);
			//else{
				if (IsPremul)
					return GenerateMixingLerp(Dest, float4(BlendedSrcRGB, Alpha), Alpha, OneMinusAlpha, IsPremul);
				else
					return GenerateMixingLerp(Dest, float4(BlendedSrcRGB, 1), Alpha, OneMinusAlpha, IsPremul);
			//}
		}
		if (AlphaMode == 1){
			/*if (Alpha == 1){
				if (CreatesPremul)
					return Dest.rgb = BlendedSrcRGB * Dest.a;
				else
					return Dest.rgb = BlendedSrcRGB;
			}
			else{*/
				if (CreatesPremul)
					return float4(GenerateMixingLerpRGB(Dest.rgb, BlendedSrcRGB * Dest.a, Alpha,(1 - (Alpha)), IsPremul),Dest.a);
				else
					return float4(GenerateMixingLerpRGB(Dest.rgb, BlendedSrcRGB, Alpha, OneMinusAlpha, IsPremul),Dest.a);
			//}
		}
		if (AlphaMode == 3){
			/*if (Alpha == 1){
				return Dest = float4(BlendedSrcRGB,1);
			}
			else */
			/*if (AlphaA == ""){
				if (CreatesPremul)
					return Dest = GenerateMixingLerp(Dest, float4(BlendedSrcRGB, 1), AlphaNoA, OneMinusAlphaNoA, IsPremul);
				else
					return Dest = GenerateMixingLerp(Dest, float4(BlendedSrcRGB, 1), AlphaNoA, OneMinusAlphaNoA, IsPremul);
			}
			else if (AlphaNoA == ""){
				if (IsPremul||!CreatesPremul)
					return Dest = BlendedSrcRGBA;
				else
					return Dest = float4(BlendedSrcRGB * Src.a, Src.a);
			}
			else{*/
				if (IsPremul||!CreatesPremul)
					return GenerateMixingLerp(Dest, BlendedSrcRGBA, AlphaNoA, OneMinusAlphaNoA, IsPremul);
				else
					return GenerateMixingLerp(Dest, float4(BlendedSrcRGB * Src.a, Src.a), AlphaNoA, OneMinusAlphaNoA, IsPremul);
			//}
		}
		if (AlphaMode == 2){
			/*if (Alpha == 1){
				if (CreatesPremul)
					return Dest = 0;
				else
					return Dest.a = 0;
			}
			else{*/
				if (CreatesPremul)
					return Dest *= OneMinusAlpha;
				else
					return Dest.a *= OneMinusAlpha;
			//}
		}
		return 0;
	}
			
	float4 GenerateMixingCodeRGB(float3 Dest, float4 Src, float3 BlendedSrc, float AlphaMode, float Alpha, float AlphaNoA, float OneMinusAlpha, float IsPremul, float CreatesPremul){
		if (AlphaMode == 0)
			return float4(GenerateMixingLerpRGB(Dest, BlendedSrc, Alpha, OneMinusAlpha, IsPremul),1);
		if (AlphaMode == 3)
			return float4(GenerateMixingLerpRGB(Dest, BlendedSrc, AlphaNoA, 1-AlphaNoA, IsPremul),1);
		return 0;
	}	
	float GenerateMixingCodeSingle(float Dest, float4 Src, float BlendedSrc, float AlphaMode, float Alpha, float AlphaNoA, float OneMinusAlpha, float IsPremul, float CreatesPremul){
		if (AlphaMode == 0)
			return GenerateMixingLerpSingle(Dest, BlendedSrc, Alpha, OneMinusAlpha, IsPremul);
		if (AlphaMode == 3)
			return GenerateMixingLerpSingle(Dest, BlendedSrc, AlphaNoA, 1-AlphaNoA, IsPremul);
		return 0;
	}

			float4 frag (v2f i) : SV_Target{
				float4 Dest = tex2D(_Previous,i.uv);
				float4 Src = tex2D(_New,i.uv);
				float4 Mask = tex2D(_Mask,i.uv);
				
				float Alpha = _Amount;
				float AlphaNoA = _Amount;
				
				if (_UseMask==1){
					Alpha *= Mask;
					AlphaNoA *= Mask;
				}
				if (_Transparent==1)
					Alpha *= Src.a;
			
				if (_EndTag==0){
					if(_BlendMode==0)			
						return GenerateMixingCodeRGBA(Dest,Src,Src,Src.rgb,_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
					else if(_BlendMode==1){
						if (_AlphaMode==0){
							_AlphaMode = 1;
							return GenerateMixingCodeRGBA(Dest,Src,
							float4(Dest.rgb + Src.rgb,Src.a),
							(Dest.rgb + Src.rgb),
							_AlphaMode,Alpha,AlphaNoA,1-Alpha,0,0);
						}
						else
							return GenerateMixingCodeRGBA(Dest,Src,
							float4(Dest.rgb + Src.rgb,Src.a),
							(Dest.rgb + Src.rgb),
							_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
					}
					else if(_BlendMode==2)
						return GenerateMixingCodeRGBA(Dest,Src,
						float4(Dest.rgb - Src.rgb,Src.a),
						(Dest.rgb - Src.rgb),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
					else if(_BlendMode==3)
						return GenerateMixingCodeRGBA(Dest,Src,
						float4(Dest.rgb * Src.rgb,Src.a),
						(Dest.rgb * Src.rgb),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,0);
					else if(_BlendMode==4)
						return GenerateMixingCodeRGBA(Dest,Src,
						float4(Dest.rgb / Src.rgb,Src.a),
						(Dest.rgb / Src.rgb),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,0);
					else if(_BlendMode==5)
						return GenerateMixingCodeRGBA(Dest,Src,
						float4(max(Dest.rgb,Src.rgb),Src.a),
						max(Dest.rgb,Src.rgb),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
					else if(_BlendMode==6)
						return GenerateMixingCodeRGBA(Dest,Src,
						float4(min(Dest.rgb,Src.rgb),Src.a),
						min(Dest.rgb,Src.rgb),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
					else if(_BlendMode==7)
						return GenerateMixingCodeRGBA(Dest,Src,
						float4(normalize(float3(Dest.xy+Src.xy,Src.z)),Src.a),
						normalize(float3(Dest.xy+Src.xy,Src.z)),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
					else if(_BlendMode==8)
						return GenerateMixingCodeRGBA(Dest,Src,
						float4(dot(Dest.rgb,Src.rgb).rrr,Src.a),
						dot(Dest.rgb,Src.rgb),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
				}
				else if (_EndTag==1){//rgb
					float3 taggedSrc = Src.rgb;
					if(_BlendMode==0)
						return GenerateMixingCodeRGB(Dest,Src,
						taggedSrc,
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
							
					else if(_BlendMode==1)
						return GenerateMixingCodeRGB(Dest,Src,
						(Dest + taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
						
					else if(_BlendMode==2)
						return GenerateMixingCodeRGB(Dest,Src,
						(Dest - taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
						
					else if(_BlendMode==3)
						return GenerateMixingCodeRGB(Dest,Src,
						(Dest * taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,0);
						
					else if(_BlendMode==4)
						return GenerateMixingCodeRGB(Dest,Src,
						(Dest / taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,0);
						
					else if(_BlendMode==5)
						return GenerateMixingCodeRGB(Dest,Src,
						max(Dest, taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
						
					else if(_BlendMode==6)
						return GenerateMixingCodeRGB(Dest,Src,
						min(Dest, taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
						
					else if(_BlendMode==7){
						float4 taggedSrc = normalize(float4(Dest.xx+Src.xy,Src.z,Src.a));
						
						if (_EndTag==2)
							taggedSrc = Src.r;
						else if (_EndTag==3)
							taggedSrc = Src.g;
						else if (_EndTag==4)
							taggedSrc = Src.b;
						else if (_EndTag==5)
							taggedSrc = Src.a;
						
						return GenerateMixingCodeRGB(Dest,Src,
						taggedSrc,
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
					}
						
					else if(_BlendMode==8)
						return GenerateMixingCodeRGB(Dest,Src,
						dot(Dest.xxx,Src.rgb),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
				}else if (_EndTag>=2){//r,g,b,a
					float4 taggedSrc = Src;
					
					if (_EndTag==2)
						taggedSrc = Src.r;
					else if (_EndTag==3)
						taggedSrc = Src.g;
					else if (_EndTag==4)
						taggedSrc = Src.b;
					else if (_EndTag==5)
						taggedSrc = Src.a;
					if(_BlendMode==0){
						if (_EndTag==5&&_AlphaMode==0){
							//Alpha *= Src.a;
							//AlphaNoA *= Src.a;
							Src = 1;
							taggedSrc = 1;
						}
						return GenerateMixingCodeSingle(Dest,Src,
						taggedSrc,
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
					}
							
					else if(_BlendMode==1)
						return GenerateMixingCodeSingle(Dest,Src,
						(Dest + taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
						
					else if(_BlendMode==2)
						return GenerateMixingCodeSingle(Dest,Src,
						(Dest - taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
						
					else if(_BlendMode==3)
						return GenerateMixingCodeSingle(Dest,Src,
						(Dest * taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,0);
						
					else if(_BlendMode==4)
						return GenerateMixingCodeSingle(Dest,Src,
						(Dest / taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,0);
						
					else if(_BlendMode==5)
						return GenerateMixingCodeSingle(Dest,Src,
						max(Dest, taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
						
					else if(_BlendMode==6)
						return GenerateMixingCodeSingle(Dest,Src,
						min(Dest, taggedSrc),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
						
					else if(_BlendMode==7){
						float4 taggedSrc = normalize(float4(Dest.xx+Src.xy,Src.z,Src.a));
						
						if (_EndTag==2)
							taggedSrc = Src.r;
						else if (_EndTag==3)
							taggedSrc = Src.g;
						else if (_EndTag==4)
							taggedSrc = Src.b;
						else if (_EndTag==5)
							taggedSrc = Src.a;
						
						return GenerateMixingCodeSingle(Dest,Src,
						taggedSrc,
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
					}
						
					else if(_BlendMode==8)
						return GenerateMixingCodeSingle(Dest,Src,
						dot(Dest.xxx,Src.rgb),
						_AlphaMode,Alpha,AlphaNoA,1-Alpha,_IsPremul,_CreatesPremul);
				}
				return Dest;
			}
			ENDCG
		}
	}
}
