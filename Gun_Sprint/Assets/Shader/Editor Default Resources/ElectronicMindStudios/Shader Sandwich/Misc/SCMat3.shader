// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with '_Object2World'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'mul(UNITY_MATRIX_MVP,*)'

Shader "Hidden/SSCubemapSkybox2" {
Properties {
	[NoScaleOffset] _Cube ("Cubemap   (HDR)", Cube) = "grey" {}
}

SubShader {
Tags { "RenderType"="Opaque" "Queue" = "Background"}
LOD 200
	cull Front

	Pass {
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0

		#include "UnityCG.cginc"

		samplerCUBE _Cube;
		half4 _Cube_HDR;

		float4 RotateAroundYInDegrees (float4 vertex, float degrees)
		{
			float alpha = degrees * UNITY_PI / 180.0;
			float sina, cosa;
			sincos(alpha, sina, cosa);
			float2x2 m = float2x2(cosa, -sina, sina, cosa);
			return float4(mul(m, vertex.xz), vertex.yw).xzyw;
		}
		
		struct appdata_t {
			float4 vertex : POSITION;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float3 texcoord : TEXCOORD0;
		};

		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);//RotateAroundYInDegrees(v.vertex, _Rotation));
			o.texcoord = -UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex));
			return o;
		}

		fixed4 frag (v2f i) : SV_Target
		{
			half4 tex = texCUBElod (_Cube, float4(i.texcoord,0));
			#if (UNITY_VERSION>500)
				half3 c = DecodeHDR (tex, _Cube_HDR);
			#else
				half3 c = tex.rgb;
			#endif
			return half4(c, 1);
		}
		ENDCG 
	}
} 	


Fallback Off

}
