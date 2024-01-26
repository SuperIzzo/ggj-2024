using Godot;
using System;
using System.Diagnostics;


public partial class Minigame : Node2D
{
	public enum Stage
	{
		Idle,
		Init,
		LineUp,
		Engage,
		Exit
	}

	[Export]
	public Node2D PlayAreaNode;

	[Export]
	public AttackController AttackLocation;

	[Export]
	public ProtectController ProtectLocation;

	private Stage m_eStage = new();
	private Vector2 m_vAreaSize = new();
	private double m_dStageTimeLeft;


	public void TriggerStart()
	{
		m_eStage = Stage.Init;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug.Assert(PlayAreaNode != null);
		Debug.Assert(AttackLocation != null);
		Debug.Assert(ProtectLocation != null);

		if(PlayAreaNode is Polygon2D poly)
		{
			Vector2 vMin = new();
			Vector2 vMax = new();

			foreach(Vector2 p in poly.Polygon)
			{
				if(p.X < vMin.X)
				{
					vMin.X = p.X;
				}
				if(p.Y < vMin.Y)
				{
					vMin.Y = p.Y;
				}

				if(p.X > vMax.X)
				{
					vMax.X = p.X;
				}
				if(p.Y > vMax.Y)
				{
					vMax.Y = p.Y;
				}
			}

			m_vAreaSize = vMax - vMin;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(m_eStage)
		{
			case Stage.Idle:
			{
				TriggerStart(); // Temp for now until a calling system can call this for us
				break;
			}
			case Stage.Init:
			{
				Stage_Init();
				break;
			}
			case Stage.LineUp:
			{
				Stage_LineUp();
				break;
			}
			case Stage.Engage:
			{
				Stage_Engage();
				break;
			}
			case Stage.Exit:
			{
				Stage_Exit();
				break;
			}
		}

		m_dStageTimeLeft -= delta;
	}

	private double GetStageTimer(Stage eStage)
	{
		switch(m_eStage)
		{
			case Stage.LineUp:		return 5.0;
			case Stage.Engage:		return 5.0;
			default: return 0.0;
		}
	}

	public override void _Input(InputEvent @event)
	{
		// Mouse in viewport coordinates.
		if (@event is InputEventMouseButton eventMouseButton)
		{
			//GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);
		}
		else if (@event is InputEventMouseMotion eventMouseMotion)
		{
			AttackLocation.UpdateMouseInput(eventMouseMotion.Relative);
			GD.Print("Mouse Motion at: ", eventMouseMotion.Relative);
		}

		// Print the size of the viewport.
		//GD.Print("Viewport Resolution is: ", GetViewport().GetVisibleRect().Size);
	}

	private void Stage_Init()
	{
		int iSpawnPaddingX = 20;
		int iSpawnPaddingY = 20;
		
		Vector2 GetRandomSpawnPos()
		{
			return new Vector2(iSpawnPaddingX + (GD.Randi() % m_vAreaSize.X), iSpawnPaddingY + (GD.Randi() % m_vAreaSize.Y));
		}

		m_eStage = Stage.LineUp;
		m_dStageTimeLeft = GetStageTimer(Stage.LineUp);

		AttackLocation.Position = GetRandomSpawnPos();
		ProtectLocation.Position = GetRandomSpawnPos();

		AttackLocation.SetBounds(m_vAreaSize);
		ProtectLocation.SetBounds(m_vAreaSize);

		Input.MouseMode = Input.MouseModeEnum.Captured;
		//Input.set_mouse_mode(Input.MOUSE_MODE_HIDDEN);
	}

	private void Stage_LineUp()
	{
		AttackLocation.Process(Stage.LineUp, m_dStageTimeLeft);
		ProtectLocation.Process(Stage.LineUp, m_dStageTimeLeft);
	}

	private void Stage_Engage()
	{
		
	}

	private void Stage_Exit()
	{
		m_eStage = Stage.Idle;
	}
}
