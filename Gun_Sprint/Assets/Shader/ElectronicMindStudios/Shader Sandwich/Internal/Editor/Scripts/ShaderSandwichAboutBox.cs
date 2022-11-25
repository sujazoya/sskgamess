using UnityEditor;
using UnityEngine;
using System.Collections;

using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public class ShaderSandwichAboutBox : EditorWindow{
	public void OnEnable(){
		titleContent = new GUIContent("About Shader Sandwich :D"); 
		position = new Rect(position.x,position.y,400,225);
		minSize = new Vector2(400,225);
		maxSize = new Vector2(400,225);
	}
    void OnGUI(){
		if (ShaderSandwich.Instance==null){
			Close();
		}
		GUI.DrawTexture(new Rect(0,0,position.width,80),ShaderSandwich.BannerLight,ScaleMode.ScaleToFit);
		GUI.Label(new Rect(70,60,position.width-10,40),"Version 1718! :D");
		GUI.skin.label.alignment = TextAnchor.UpperRight;
		GUI.Label(new Rect(0,60,position.width-10,40),"Created by Sean Boettger :)\n© 1660   ");
		EditorGUI.SelectableLabel(new Rect(20,position.height-80,position.width-20,40),"Thanks to Icon8 for the trash can icon: \nhttps://icons8.com/icon/362/Trash-Can",GUI.skin.label);
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		
		EditorGUI.SelectableLabel(new Rect(0,position.height-20,position.width-20,40),"Alright, done my due diligence :)",GUI.skin.label);
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		
		EditorGUI.SelectableLabel(new Rect(0,100,position.width-20,40),"Equal parts adorable and bizzare cat and bin icons from: \nhttps://raindropmemory.deviantart.com/art/down-to-earth-Iconset-328754834, thanks!",GUI.skin.label);
    }

    void OnInspectorUpdate(){
        Repaint();
    }
}
}