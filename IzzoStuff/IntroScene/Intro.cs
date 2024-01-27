using Godot;
using System;

public partial class Intro : Node3D
{
	[Export]
	public SlonResource Slon;
	
	Label3D title;
	Label3D subtitle;
	Node3D character;
	
	PackedScene model;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		title = this.GetChildByType<Label3D>("Title");
		subtitle = this.GetChildByType<Label3D>("Subtitle");
		character = this.GetChildByType<Node3D>("Character");
		
		title.Text = Slon.IntroTitle;
		subtitle.Text = Slon.IntroSubtitle;
		
		var slonModel = Slon.SlonModel.Instantiate();
		character.AddChild(slonModel);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
