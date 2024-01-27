using Godot;
using System;

public partial class Intro : Node3D
{
	[Export]
	public SlonResource Slon;
	
	Label3D title;
	Label3D subtitle;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		title = this.GetChildByType<Label3D>("Title");
		subtitle = this.GetChildByType<Label3D>("Subtitle");
		
		title.Text = Slon.IntroTitle;
		subtitle.Text = Slon.IntroSubtitle;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
