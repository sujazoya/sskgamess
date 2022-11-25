using System.Collections.Generic;
using System.Collections;
using System;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public class ShaderFix{
	public enum FixType{Message,Optimization,Warning,Error}
	//public ShaderVar shaderVar;
	public object Parent = null;
	public string MessageWName = "I forgot to set a message for this fix...sorry!";
	public string _Message = "I forgot to set a message for this fix...sorry!";
	public string Message{
		get{return _Message;}
		set{
			_Message = value;
			MessageWName = Message;
			if (Parent as ShaderIngredient!=null)
				MessageWName = (Parent as ShaderIngredient).Name+"(Ingredient) - "+MessageWName;
			if (Parent as ShaderLayer!=null)
				MessageWName = (Parent as ShaderLayer).Name.Text+"(Layer) - "+MessageWName;
		}
	}
	public bool Ignore = false;
	public Action Fix = null;
	public FixType Type = FixType.Message;
	
	public ShaderFix(object parent, string message, FixType ftw, Action fox){
		Parent = parent;
		Message = message;
		Type = ftw;
		Fix = fox;
	}
}
}