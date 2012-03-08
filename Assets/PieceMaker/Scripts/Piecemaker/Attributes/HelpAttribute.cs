using System;

public class HelpAttribute : Attribute
{
	public string Text { get; set; }
	
	public HelpAttribute(string text)
	{
		Text = text;
	}
}