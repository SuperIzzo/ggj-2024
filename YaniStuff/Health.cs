using Godot;
using System;

public partial class Health : Godot.Control
{
	private ProgressBar healthProgressBar;
	
	public double CurrentHp
{
	get { return healthProgressBar.Value; }
	set
	{
		healthProgressBar.Value = value;
	}
}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthProgressBar = GetNode<ProgressBar>("HPBar");
		CurrentHp = 100;
		CurrentHp = DamageHealth(CurrentHp, 50);
		
	}
		
		
		
	 public void UpdateHealth(double CurrentHp)
	{
		CurrentHp = Mathf.Clamp(CurrentHp, 0, 100);
	}
	
	public double DamageHealth(double CurrentHp, double damage)
	{
		double remainingHp = CurrentHp - damage;
		return remainingHp;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
