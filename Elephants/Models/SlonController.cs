using Godot;
using System;

public partial class SlonController : Node3D
{
	private AnimationPlayer animPlayer;
		
	public override void _Ready()
	{
		animPlayer = this.GetChildByType<AnimationPlayer>();
	}
	
	public void PlayRunAnim()
	{
		animPlayer.Play("walkCycle");
	}
}
