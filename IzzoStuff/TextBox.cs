using Godot;
using System;

public partial class TextBox : MarginContainer
{
	Label text;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		text = this.GetChildByType<Label>();
		
		SetText("Indsaet tekst her");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetText(string message)
	{
		text.Text = message;
	}
}
