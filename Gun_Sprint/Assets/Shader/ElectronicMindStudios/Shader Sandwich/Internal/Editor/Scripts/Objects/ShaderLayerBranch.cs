using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public struct ShaderLayerBranch{
	public int Effect;
	public string MapXOffset;
	public string MapYOffset;
	public string MapZOffset;
	public bool ResetAlpha;
	public ShaderLayerBranch(bool blergstupidcsharp){
		Effect = 0;
		MapXOffset = "";
		MapYOffset = "";
		MapZOffset = "";
		ResetAlpha = false;
	}
}
}