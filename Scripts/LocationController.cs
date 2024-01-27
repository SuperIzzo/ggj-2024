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

	[Export]
	protected Vector2 MaxVelocity = Vector2.Zero;

	[Export]
	protected Color AbilityRadiusColor;

	protected Vector2 m_vBoundsSize;
	protected Vector2 m_vInput;

	[Export]
	protected float AbilityRadius = 30.0f;

	public delegate Minigame DelGetMinigame();
	public DelGetMinigame GetMinigameDelegate;

	public virtual void Process(Minigame.Stage eStage, double dTimeLeft) 
	{
		ProcessAcceleration();
		ProcessFriction();
		ProcessKeepInBounds();
	}

	public void SetBounds(Vector2 vBounds) => m_vBoundsSize = vBounds;


	public virtual void AddInput(Vector2 vInput) {}

	protected void ApplyForce(Vector2 v, bool bApplyingFriction = false)
	{
		if(!bApplyingFriction && IsVelocityOutOfBounds())
		{
			return;
		}

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

		ApplyForce(vFriction, true);
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

	private bool IsVelocityOutOfBounds()
	{
		if(!MaxVelocity.IsZeroApprox())
		{
			// If the current velocity is outside of our max vel when wanting to add input, bail out
			Vector2 vClamped = m_vVelocity.Clamp(-MaxVelocity, MaxVelocity);
			if(!m_vVelocity.IsEqualApprox(vClamped))
			{
				return true;
			}
		}

		return false;
	}

	public override void _Draw()
	{
		DrawCircle(new Vector2(0.0f, 0.0f), AbilityRadius, AbilityRadiusColor);
	}
}
