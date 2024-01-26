using Godot;
using System;

public partial class LocationController : Node2D
{
	public Vector2 m_vVelocity = new();
	public Vector2 m_vAcceleration = new();
	
	[Export]
	protected float MovementStrength = 1.0f;

	[Export]
	protected float Mass = 5.0f;

	[Export]
	protected float Friction = 0.02f;

	protected Vector2 m_vBoundsSize;

	public delegate Minigame DelGetMinigame();
	public DelGetMinigame GetMinigameDelegate;


	


	public virtual void Process(Minigame.Stage eStage, double dTimeLeft) 
	{
		ProcessAcceleration();
		ProcessFriction();
		ProcessKeepInBounds();
	}

	public void SetBounds(Vector2 vBounds) => m_vBoundsSize = vBounds;

	protected void ApplyForce(Vector2 v)
	{
		Vector2 vForce = v / Mass;
		m_vAcceleration += vForce;
	}

	protected void ProcessAcceleration()
	{
		if(m_vVelocity.Length() < 0.1f)
		{
			m_vVelocity = Vector2.Zero;
		}

		m_vVelocity += m_vAcceleration;
		Position += m_vVelocity;

		m_vAcceleration = Vector2.Zero;
	}

	protected void ProcessFriction()
	{
		Vector2 vFriction = m_vVelocity;

		vFriction *= -1.0f;
		vFriction = vFriction.Normalized();
		vFriction *= Friction;

		ApplyForce(vFriction);
	}

	protected void ProcessKeepInBounds()
	{
		if(Position.X < 0)
		{
			Position = new Vector2(0, Position.Y);
			m_vVelocity.X = -m_vVelocity.X;
		}
		else if(Position.X > m_vBoundsSize.X)
		{
			Position = new Vector2(m_vBoundsSize.X, Position.Y);
			m_vVelocity.X = -m_vVelocity.X;
		}

		if(Position.Y < 0)
		{
			Position = new Vector2(Position.X, 0);
			m_vVelocity.Y = -m_vVelocity.Y;
		}
		else if(Position.Y > m_vBoundsSize.Y)
		{
			Position = new Vector2(Position.X, m_vBoundsSize.Y);
			m_vVelocity.Y = -m_vVelocity.Y;
		}
	}
}
