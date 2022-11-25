float _AlphaMode;

float4 DoAlphaModeStuff(float4 dest, float4 src){
	if (_AlphaMode==0)
		return float4(src.rgb, dest.a);
	else if (_AlphaMode==1)
		return src;
	else if (_AlphaMode==2)
		return float4(dest.rgb, src.a);
	
	return dest;
}