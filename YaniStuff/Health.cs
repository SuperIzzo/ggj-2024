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
	
	public double MaxHp
	{
		get { return healthProgressBar.MaxValue; }
		set
		{
			healthProgressBar.MaxValue = value;
		}
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
		
	public override void _Ready()
	{
		healthProgressBar = GetNode<ProgressBar>("HPBar");
	}
}
