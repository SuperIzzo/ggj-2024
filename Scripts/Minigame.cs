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

	[Export]
	public AttackController EnemyAttackLocation;

	[Export]
	public ProtectController EnemyProtectLocation;

	[Export]
	int SpawnPaddingX = 50;

	[Export]
	int SpawnPaddingY = 50;

	[Export]
	float LocationRadius = 20.0f;

	private Stage m_eStage = new();
	private Vector2 m_vAreaSize = new();
	private double m_dStageTimeLeft;
	private EnemyController m_enemy = new();

	public Minigame GetMinigame() => this;

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
				Stage_LineUp(delta);
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
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			AttackLocation.AddInput(eventMouseMotion.Relative);
		}
		else if (@event is InputEventMouseButton eventMouseButton)
		{
			// eventMouseButton.Position
		}
	}

	private void Stage_Init()
	{
		Vector2 GetRandomSpawnPos()
		{
			return new Vector2(
				SpawnPaddingX + (GD.Randi() % (m_vAreaSize.X - SpawnPaddingX * 2)), 
				SpawnPaddingY + (GD.Randi() % (m_vAreaSize.Y - SpawnPaddingY * 2)));
		}

		m_eStage = Stage.LineUp;
		m_dStageTimeLeft = GetStageTimer(Stage.LineUp);

		// Spawn our attack and protect positions randomly
		AttackLocation.Position = GetRandomSpawnPos();
		ProtectLocation.Position = GetRandomSpawnPos();
		EnemyAttackLocation.Position = GetRandomSpawnPos();
		EnemyProtectLocation.Position = GetRandomSpawnPos();

		
		// Capture the mouse so that it's invisible and we can get the relative movement for the frame
		Input.MouseMode = Input.MouseModeEnum.Captured;
		
		// Initialise everything that needs initing
		AttackLocation.SetBounds(m_vAreaSize);
		ProtectLocation.SetBounds(m_vAreaSize);
		AttackLocation.GetMinigameDelegate = GetMinigame;
		ProtectLocation.GetMinigameDelegate = GetMinigame;

		EnemyAttackLocation.SetBounds(m_vAreaSize);
		EnemyProtectLocation.SetBounds(m_vAreaSize);
		EnemyAttackLocation.GetMinigameDelegate = GetMinigame;
		EnemyProtectLocation.GetMinigameDelegate = GetMinigame;

		m_enemy.Init(EnemyController.Difficulty.One, m_vAreaSize, LocationRadius);
	}

	private void Stage_LineUp(double delta)
	{
		ProcessInput(delta);

		AttackLocation.Process(Stage.LineUp, m_dStageTimeLeft);
		ProtectLocation.Process(Stage.LineUp, m_dStageTimeLeft);

		EnemyAttackLocation.Process(Stage.LineUp, m_dStageTimeLeft);
		EnemyProtectLocation.Process(Stage.LineUp, m_dStageTimeLeft);

		Vector2 vAttackPosition = AttackLocation.Position;
		Vector2 vProtectLocation = ProtectLocation.Position;

		if(vAttackPosition.DistanceSquaredTo(vProtectLocation) < LocationRadius * LocationRadius)
		{
			Vector2 attackVel = AttackLocation.m_vVelocity;
			Vector2 protectVel = ProtectLocation.m_vVelocity;
			Vector2 vAttToPro = (vProtectLocation - vAttackPosition).Normalized();

			AttackLocation.m_vVelocity = protectVel;
			AttackLocation.Position += protectVel.Normalized();
			AttackLocation.m_vVelocity += -vAttToPro;

			ProtectLocation.m_vVelocity = attackVel;
			ProtectLocation.Position += attackVel.Normalized();
			ProtectLocation.m_vVelocity += vAttToPro;
		}
	}

	private void Stage_Engage()
	{
		
	}

	private void Stage_Exit()
	{
		m_eStage = Stage.Idle;
	}

	private void ProcessInput(double delta)
	{
		Vector2 vInput = new();

		// Add to the janky ps1 demo vibe, only allow one input at a time
		//	and make the player keep pressing the direction
		if(Input.IsActionJustPressed("Key_Left"))
		{
			vInput.X = -1;
		}
		else if(Input.IsActionJustPressed("Key_Right"))
		{
			vInput.X = 1;
		}
		else if(Input.IsActionJustPressed("Key_Up"))
		{
			vInput.Y = -1;
		}
		else if(Input.IsActionJustPressed("Key_Down"))
		{
			vInput.Y = 1;
		}

		ProtectLocation.AddInput(vInput);

		m_enemy.Process(AttackLocation.Position, ProtectLocation.Position, 
			EnemyAttackLocation.Position, EnemyProtectLocation.Position, delta);

		EnemyAttackLocation.AddInput(m_enemy.GetAttackInput());
		EnemyProtectLocation.AddInput(m_enemy.GetProtectInput());
	}
}
