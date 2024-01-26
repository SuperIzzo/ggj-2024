using Godot;
using System;

public partial class LocationController : Node2D
{
	protected Vector2 m_vBoundsSize;

	public virtual void Process(Minigame.Stage eStage, double dTimeLeft) 
	{
		
	}

	public void SetBounds(Vector2 vBounds) => m_vBoundsSize = vBounds;
}
