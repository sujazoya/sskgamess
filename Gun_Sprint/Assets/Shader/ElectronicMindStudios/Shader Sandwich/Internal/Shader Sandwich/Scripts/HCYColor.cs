using UnityEngine;
//Thanks to andeeeee for the following! :)
public struct HCYColor {
	public float h;
	public float c;
	public float y;
	public float a;
	
	static Vector3 HCYwts = new Vector3(0.299f, 0.587f, 0.114f);
	public HCYColor(float h, float c, float y, float a) {
		this.h = h;
		this.c = c;
		this.y = y;
		this.a = a;
	}
	
	public HCYColor(float h, float c, float y) {
		this.h = h;
		this.c = c;
		this.y = y;
		this.a = 1f;
	}
	
	public HCYColor(Color col) {
		HCYColor temp = FromRGBA(col);
		h = temp.h;
		c = temp.c;
		y = temp.y;
		a = temp.a;
	}
	public static Vector3 HUEtoRGB(float H){
		float R = Mathf.Abs(H * 6f - 3f) - 1f;
		float G = 2f - Mathf.Abs(H * 6f - 2f);
		float B = 2f - Mathf.Abs(H * 6f - 4f);
		return new Vector3(Mathf.Clamp(R,0,1),Mathf.Clamp(G,0,1),Mathf.Clamp(B,0,1));
	}

	public static float RGBCVtoHUE(Vector3 RGB, float C, float V){
		Vector3 Delta = (new Vector3(V,V,V) - RGB) / C;
		Delta -= new Vector3(Delta.z,Delta.x,Delta.y);
		Delta += new Vector3(2f,4f,6f);
		// NOTE 1
		//Delta.brg = step(V, RGB) * Delta.brg;
		Vector3 DeltaTemp = Delta+new Vector3(0,0,0);//Can't remember if it's a struct or not
		Delta.z = (V>=RGB.x)?1:0 * DeltaTemp.z;
		Delta.x = (V>=RGB.y)?1:0 * DeltaTemp.x;
		Delta.y = (V>=RGB.z)?1:0 * DeltaTemp.y;
		float H;
		H = Mathf.Max(Delta.x, Mathf.Max(Delta.y, Delta.z));
		return (H / 6f)-Mathf.Floor(H / 6f);
	}

	static public float Epsilon = 1e-10f;
 
	public static Vector3 RGBtoHCV(Color RGB){
		// Based on work by Sam Hocevar and Emil Persson
		Vector4 P = (RGB.g < RGB.b) ? new Vector4(RGB.b,RGB.g, -1.0f, 2.0f/3.0f) : new Vector4(RGB.g,RGB.b, 0.0f, -1.0f/3.0f);
		Vector4 Q = (RGB.r < P.x) ? new Vector4(P.x,P.y,P.w, RGB.r) : new Vector4(RGB.r, P.y,P.z,P.x);
		float C = Q.x - Mathf.Min(Q.w, Q.y);
		float H = Mathf.Abs((Q.w - Q.y) / (6f * C + Epsilon) + Q.z);
		return new Vector3(H, C, Q.x);
	}
	public static HCYColor FromRGBA(Color col) {
	// Corrected by David Schaeffer
	Vector3 RGB = new Vector3(col.r,col.g,col.b);
	Vector3 HCV = RGBtoHCV(col);
	float Y = Vector3.Dot(RGB, HCYwts);
	float Z = Vector3.Dot(HUEtoRGB(HCV.x), HCYwts);
	if (Y < Z){
		HCV.y *= Z / (Epsilon + Y);
	}
	else{
		HCV.y *= (1f - Z) / (Epsilon + 1f - Y);
	}
	return new HCYColor(HCV.x, HCV.y, Y,col.a);		
		/*float h = 0f, c, y, a;
		a = col.a;
		
		float U,V;
		U = -Mathf.Min(col.r,Mathf.Min(col.b,col.g));
		V = Mathf.Max(col.r,Mathf.Max(col.b,col.g));
		c = V+U;
		y = Vector3.Dot(new Vector3(col.r,col.g,col.b),HCYwts);
		if (y!=0){
			h = RGBCVtoHUE(new Vector3(col.r,col.g,col.b), c, V);
			float Z = Vector3.Dot(HUEtoRGB(y),HCYwts);
			if (y>Z){
				y = 1-y;
				Z = 1-Z;
			}
			c *= Z/y;
		}*/
		//float h = 0f, c, y, a;
		//a = col.a;
		
		
		//return new HCYColor(h, c, y, a);
	}
	
	public Color ToRGBA() {
		h = h-Mathf.Floor(h);
		Vector3 RGB = HUEtoRGB(h);
		float Z = Vector3.Dot(RGB, HCYwts);
		if (y < Z){
		  c *= y / Z;
		}
		else if (Z < 1){
		  c *= (1f - y) / (1f - Z);
		}
		RGB = ((RGB - new Vector3(Z,Z,Z)) * c + new Vector3(y,y,y));
		return new Color(RGB.x,RGB.y,RGB.z,a);
	}
	
	static float Value(float n1, float n2, float hue) {
		hue = Mathf.Repeat(hue, 360f);
		
		if (hue < 60f) {
			return n1 + (n2 - n1) * hue / 60f;
		} else if (hue < 180f) {
			return n2;
		} else if (hue < 240f) {
			return n1 + (n2 - n1) * (240f - hue) / 60f;
		} else {
			return n1;
		}
	}
	
	public static implicit operator HCYColor(Color src) {
		return FromRGBA(src);
	}
	
	public static implicit operator Color(HCYColor src) {
		return src.ToRGBA();
	}

}
