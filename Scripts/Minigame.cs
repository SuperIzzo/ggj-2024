using Godot;
using System;
using System.Collections.Generic;
using System.Transactions;


public partial class Minigame : Node2D
{
	public enum Stage
	{
		Idle,
		Init,
		LineUp,
		IntermissionSlowDown,
		Engage,
		ProcessResult,
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
	public Polygon2D TimeBar;

	[Export]
	public Sprite2D UpKey;
	[Export]
	public Sprite2D DownKey;
	[Export]
	public Sprite2D LeftKey;
	[Export]
	public Sprite2D RightKey;

	[Export]
	public Sprite2D UpArrow;
	[Export]
	public Sprite2D DownArrow;
	[Export]
	public Sprite2D LeftArrow;
	[Export]
	public Sprite2D RightArrow;

	[Export]
	Label PlayerAttackLabel;

	[Export]
	Label EnemyAttackLabel;

	[Export]
	int SpawnPaddingX = 50;

	[Export]
	int SpawnPaddingY = 50;

	[Export]
	float LocationRadius = 20.0f;
	
	[Export]
	bool autostart = true;

	private Stage m_eStage = new();
	private Vector2 m_vAreaSize = new();
	private double m_dStageTimeLeft;
	private EnemyController m_enemy = new();
	private float m_fTimeMaxWidth;
	private EnemyController.Difficulty m_eCurrentDifficulty = EnemyController.Difficulty.One;

	private Vector2 m_vLockedInAttackPosition;
	private Vector2 m_vEnemyAttackPos;
	private Vector2 m_vEnemyProtectPos;
	private double m_dEnemyHintTimer;

	private double m_dPlayerAttackLabelTimer;
	private double m_dEnemyAttackLabelTimer;

	private int m_iNumTimesEngaged;
	private bool m_bResultProcessed;

	Dictionary<string, string> m_randomKeyMaps = new();
	int m_iChosenAttackInput = -1;
	int m_iChosenProtectInput = -1;

	Vector2 m_vRelativeMouseInputForEngage = Vector2.Zero;

	List<Tuple<int, Vector2>> m_keyDirs;

	private class KeyPressCollection
	{
		public string ActionName;
		public Sprite2D Spr;

		// 0-Top, 1-Right, 2-Bot, 3-Left
		public int Dir;
	}

	List<KeyPressCollection> m_defendKeys = new();

	public Minigame GetMinigame() => this;
	public Stage CurrentStage => m_eStage;

	public void TriggerStart()
	{
		m_eStage = Stage.Init;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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

		m_keyDirs = new()
		{
			new(0, UpKey.Position),
			new(1, RightKey.Position),
			new(2, DownKey.Position),
			new(3, LeftKey.Position)
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(m_eStage)
		{
			case Stage.Idle:
			{
				if (autostart)
				{
					TriggerStart(); // Temp for now until a calling system can call this for us
				}
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
			case Stage.IntermissionSlowDown:
			{
				if(m_dStageTimeLeft > GetStageTimer(Stage.IntermissionSlowDown))
				{
					InitEngageStage();
				}

				m_dStageTimeLeft += delta;
				ProcessTimer();

				break;
			}
			case Stage.Engage:
			{
				Stage_Engage(delta);
				break;
			}
			case Stage.ProcessResult:
			{
				Stage_ProcessResult(delta);
				break;
			}
			case Stage.Exit:
			{
				Stage_Exit();
				break;
			}
		}

		ProcessAttackLabels(delta);
	}

	private double GetStageTimer(Stage eStage)
	{
		switch(m_eStage)
		{
			case Stage.LineUp:		return 4.0;
			case Stage.Engage:		return 3.0;
			case Stage.IntermissionSlowDown: return 1.0;
			case Stage.ProcessResult: return 1.5;
			default: return 0.0;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			if(m_eStage == Stage.LineUp)
			{
				AttackLocation.AddInput(eventMouseMotion.Relative);
			}
			else if(m_eStage == Stage.Engage)
			{
				m_vRelativeMouseInputForEngage = eventMouseMotion.Relative * 0.025f;
			}
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

		HideKeys();
		HideArrows();

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

		m_enemy.Init(m_eCurrentDifficulty, m_vAreaSize, LocationRadius);
		{
			float fDiff = (float)m_eCurrentDifficulty;

			ProtectLocation.AbilityRadius = 55.0f - (2.0f * fDiff);
			ProtectLocation.QueueRedraw();

			EnemyProtectLocation.AbilityRadius = 40.0f + (3.5f * fDiff);
			EnemyProtectLocation.QueueRedraw();
		}

		m_fTimeMaxWidth = TimeBar.Polygon[1].X;

		m_iNumTimesEngaged = 0;
	}

	private void Stage_LineUp(double delta)
	{
		ProcessLineUpInput(delta);

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

		m_dStageTimeLeft -= delta;
		ProcessTimer();
		
		if(m_dStageTimeLeft < 0.0)
		{
			GotoIntermission();
		}
	}

	private void GotoIntermission()
	{
		m_eStage = Stage.IntermissionSlowDown;
	}
	private void InitEngageStage()
	{
		m_iChosenProtectInput = -1;
		m_iChosenAttackInput = -1;

		++m_iNumTimesEngaged;

		m_dEnemyHintTimer = 0.0;

		m_eStage = Stage.Engage;
		m_dStageTimeLeft = GetStageTimer(Stage.LineUp);

		AttackLocation.m_vVelocity = Vector2.Zero;
		ProtectLocation.m_vVelocity = Vector2.Zero;
		EnemyAttackLocation.m_vVelocity = Vector2.Zero;
		EnemyProtectLocation.m_vVelocity = Vector2.Zero;

		m_vLockedInAttackPosition = AttackLocation.Position;
		m_vEnemyAttackPos = EnemyAttackLocation.Position;
		m_vEnemyProtectPos = EnemyProtectLocation.Position;

		m_enemy.ChooseEngageDirections(m_eCurrentDifficulty, AttackLocation, EnemyAttackLocation, ProtectLocation, EnemyProtectLocation);

		RandomiseAndShowKeys();
		ShowArrows();
	}

	private void Stage_Engage(double delta)
	{
		m_dStageTimeLeft -= delta;

		ProcessTimer();
		ProcessEngageInput();
		ProcessEnemyHinting(delta);

		AttackLocation.Position = m_vLockedInAttackPosition + m_vRelativeMouseInputForEngage;

		if(m_dStageTimeLeft < 0.0)
		{
			AttackLocation.Position = m_vLockedInAttackPosition;
			EnemyAttackLocation.Position = m_vEnemyAttackPos;
			EnemyProtectLocation.Position = m_vEnemyProtectPos;
			EnemyAttackLocation.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			EnemyProtectLocation.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);

			m_eStage = Stage.ProcessResult;
			m_dStageTimeLeft = GetStageTimer(Stage.ProcessResult);
			m_bResultProcessed = false;
		}
	}

	private void Stage_ProcessResult(double delta)
	{
		m_dStageTimeLeft -= delta;

		void MoveInDirection(LocationController c, int dir)
		{
			Vector2 vDir = EnemyController.GetVectorFromDirection(dir);
			c.Position += vDir * 20.0f;
		}

		// TODO
		/*
		phase 2: Instantly move in direction player and enemy want to go
		phase 2: Check overlaps of shields and sword to determine if player/enemy got hit
		phase 2: Call to show hit/block text.
		phase 2: Repeat Engage phase 3 times
		Exit the minigame, launches new minigame next time charge happens
		
		*/

		if(!m_bResultProcessed)
		{
			m_bResultProcessed = true;
			
			MoveInDirection(AttackLocation, m_iChosenAttackInput);
			MoveInDirection(ProtectLocation, m_iChosenProtectInput);
			MoveInDirection(EnemyAttackLocation, m_enemy.AttackEngageDirection);
			MoveInDirection(EnemyProtectLocation, m_enemy.ProtectEngageDirection);

			bool bPlayerHitEnemy = AttackLocation.Position.DistanceTo(EnemyProtectLocation.Position) > EnemyProtectLocation.AbilityRadius;
			bool bEnemyHitPlayer = EnemyAttackLocation.Position.DistanceTo(ProtectLocation.Position) > ProtectLocation.AbilityRadius;
			
			ShowPlayerAttackLabel(bPlayerHitEnemy);
			ShowEnemyAttackLabel(bEnemyHitPlayer);

			GameGlobals globals = GetNode<GameGlobals>("/root/GameGlobals");
			

		}

		if(m_dStageTimeLeft < 0.0)
		{
			if(m_iNumTimesEngaged < 3)
			{
				InitEngageStage();
			}
			else
			{
				m_eStage = Stage.Exit;
			}
		}
		
	}

	private void ProcessEnemyHinting(double delta)
	{
		double dMaxTime = 0.75;

		m_dEnemyHintTimer += delta;

		EnemyAttackLocation.Position = m_vEnemyAttackPos.Lerp(
			m_vEnemyAttackPos + EnemyController.GetVectorFromDirection(m_enemy.AttackEngageDirection) * 5.0f, 
			Mathf.Clamp((float)Mathf.Remap(m_dEnemyHintTimer, 0.0, dMaxTime, 0.0, 1.0), 0.0f, 1.0f));

		EnemyProtectLocation.Position = m_vEnemyProtectPos.Lerp(
			m_vEnemyProtectPos + EnemyController.GetVectorFromDirection(m_enemy.ProtectEngageDirection) * 5.0f, 
			Mathf.Clamp((float)Mathf.Remap(m_dEnemyHintTimer, 0.0, dMaxTime, 0.0, 1.0), 0.0f, 1.0f));

		float fColor = Mathf.Clamp((float)Mathf.Remap(m_dEnemyHintTimer, 0.0, dMaxTime, 0.0, 0.5), 0.0f, 0.5f);
		EnemyAttackLocation.Modulate = new Color(0.5f + fColor, 0.5f + fColor, 0.5f + fColor, 0.5f + fColor);
		EnemyProtectLocation.Modulate = new Color(0.5f + fColor, 0.5f + fColor, 0.5f + fColor, 0.5f + fColor);

		if(m_dEnemyHintTimer >= dMaxTime)
		{
			m_dEnemyHintTimer = 0.0;
		}
	}

	private void Stage_Exit()
	{
		m_eStage = Stage.Idle;
	}

	private void ProcessLineUpInput(double delta)
	{
		Vector2 vInput = new();

		// Add to the janky ps1 demo vibe, make the user keep pressing inputs
		float fDirAmount = 1.0f;
		if(Input.IsActionJustPressed("Key_Left"))
		{
			vInput.X = -fDirAmount;
		}
		else if(Input.IsActionJustPressed("Key_Right"))
		{
			vInput.X = fDirAmount;
		}

		if(Input.IsActionJustPressed("Key_Up"))
		{
			vInput.Y = -fDirAmount;
		}
		else if(Input.IsActionJustPressed("Key_Down"))
		{
			vInput.Y = fDirAmount;
		}

		ProtectLocation.AddInput(vInput);

		m_enemy.Process(AttackLocation.Position, ProtectLocation.Position, 
			EnemyAttackLocation.Position, EnemyProtectLocation.Position, delta);

		EnemyAttackLocation.AddInput(m_enemy.GetAttackInput());
		EnemyProtectLocation.AddInput(m_enemy.GetProtectInput());
	}

	private void ProcessEngageInput()
	{
		if(m_iChosenProtectInput == -1)
		{
			for(int i = 0; i < m_defendKeys.Count; ++i)
			{
				if(Input.IsActionJustPressed(m_defendKeys[i].ActionName))
				{
					m_iChosenProtectInput = m_defendKeys[i].Dir;
					FadeUnusedKeysOut();
					break;
				}
			}
		}

		if(m_iChosenAttackInput == -1)
		{
			if(m_vRelativeMouseInputForEngage.X > 0.5f)
			{
				m_iChosenAttackInput = 1;
			}
			else if(m_vRelativeMouseInputForEngage.X < -0.5f)
			{
				m_iChosenAttackInput = 3;
			}
			else if(m_vRelativeMouseInputForEngage.Y > 0.5f)
			{
				m_iChosenAttackInput = 2;
			}
			else if(m_vRelativeMouseInputForEngage.Y < -0.5f)
			{
				m_iChosenAttackInput = 0;
			}

			if(m_iChosenAttackInput != -1)
			{
				FadeUnusedArrowsOut();
			}
		}
	}

	private void ProcessTimer()
	{
		float fPercentile = (float)Mathf.Clamp(m_dStageTimeLeft / GetStageTimer(Stage.LineUp), 0.0, 1.0);

		var polygons = TimeBar.Polygon;
		polygons[1] = new Vector2(m_fTimeMaxWidth * fPercentile, polygons[1].Y);
		polygons[2] = new Vector2(m_fTimeMaxWidth * fPercentile, polygons[2].Y);
		TimeBar.Polygon = polygons;
	}

	private void HideKeys()
	{
		UpKey.Visible = false;
		DownKey.Visible = false;
		LeftKey.Visible = false;
		RightKey.Visible = false;
	}

	private void HideArrows()
	{
		UpArrow.Visible = false;
		DownArrow.Visible = false;
		LeftArrow.Visible = false;
		RightArrow.Visible = false;
	}

	private void FadeUnusedKeysOut()
	{
		for(int i = 0; i < m_defendKeys.Count; ++i)
		{
			bool bFadeOut = m_defendKeys[i].Dir != m_iChosenProtectInput;
			if(bFadeOut)
			{
				m_defendKeys[i].Spr.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			}
		}
	}

	private void ShowPlayerAttackLabel(bool bHitSuccess)
	{
		m_dPlayerAttackLabelTimer = 1.5;
		PlayerAttackLabel.Text = bHitSuccess ? "Hit!" : "Block";
		PlayerAttackLabel.Position = AttackLocation.Position;
		PlayerAttackLabel.Visible = true;

		PlayerAttackLabel.Modulate = bHitSuccess ? new Color(0.2f, 0.8f, 0.2f, 1.0f) : new Color(0.8f, 0.2f, 0.2f, 1.0f);
	}

	private void ShowEnemyAttackLabel(bool bHitSuccess)
	{
		m_dEnemyAttackLabelTimer = 1.5;
		EnemyAttackLabel.Text = bHitSuccess ? "Hit!" : "Block";
		EnemyAttackLabel.Position = EnemyAttackLocation.Position;
		EnemyAttackLabel.Visible = true;
		EnemyAttackLabel.Modulate = bHitSuccess ? new Color(0.8f, 0.2f, 0.2f, 1.0f) : new Color(0.2f, 0.8f, 0.2f, 1.0f);
	}

	private void ProcessAttackLabels(double delta)
	{
		if(m_dPlayerAttackLabelTimer > 0.0)
		{
			float fAmount = (float)Mathf.Remap(m_dPlayerAttackLabelTimer, 0.5, 0.0, 0.0, 1.0);
			PlayerAttackLabel.Modulate = new Color(PlayerAttackLabel.Modulate.R, PlayerAttackLabel.Modulate.G, PlayerAttackLabel.Modulate.B, 1.0f - fAmount);

			m_dPlayerAttackLabelTimer -= delta;

			if(m_dPlayerAttackLabelTimer <= 0.05)
			{
				m_dPlayerAttackLabelTimer = 0.0;
				PlayerAttackLabel.Visible = false;
			}
		}

		if(m_dEnemyAttackLabelTimer > 0.0)
		{
			float fAmount = (float)Mathf.Remap(m_dEnemyAttackLabelTimer, 0.55, 0.0, 0.0, 1.0);
			EnemyAttackLabel.Modulate = new Color(EnemyAttackLabel.Modulate.R, EnemyAttackLabel.Modulate.G, EnemyAttackLabel.Modulate.B, 1.0f - fAmount);

			m_dEnemyAttackLabelTimer -= delta;

			if(m_dEnemyAttackLabelTimer <= 0.05)
			{
				m_dEnemyAttackLabelTimer = 0.0;
				EnemyAttackLabel.Visible = false;
			}
		}
	}

	private void FadeUnusedArrowsOut()
	{
		if(m_iChosenAttackInput == 0)
		{
			RightArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			DownArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			LeftArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
		}
		else if(m_iChosenAttackInput == 1)
		{
			UpArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			DownArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			LeftArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
		}
		else if(m_iChosenAttackInput == 2)
		{
			UpArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			RightArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			LeftArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
		}
		else if(m_iChosenAttackInput == 3)
		{
			UpArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			RightArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			DownArrow.Modulate = new Color(0.5f, 0.5f, 0.5f, 1.0f);
		}
	}

	private void ShowArrows()
	{
		UpArrow.Visible = true;
		DownArrow.Visible = true;
		LeftArrow.Visible = true;
		RightArrow.Visible = true;

		UpArrow.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		RightArrow.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		DownArrow.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		LeftArrow.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}

	private void RandomiseAndShowKeys()
	{
		m_defendKeys = new();

		m_defendKeys.Add(new()
		{
			ActionName = "Key_Up",
			Spr = UpKey,
			Dir = 0
		});

		m_defendKeys.Add(new()
		{
			ActionName = "Key_Down",
			Spr = DownKey,
			Dir = 2
		});

		m_defendKeys.Add(new()
		{
			ActionName = "Key_Left",
			Spr = LeftKey,
			Dir = 3
		});

		m_defendKeys.Add(new()
		{
			ActionName = "Key_Right",
			Spr = RightKey,
			Dir = 1
		});

		m_keyDirs.Shuffle();

		for(int i = 0; i < m_keyDirs.Count; ++i)
		{
			m_defendKeys[i].Dir = m_keyDirs[i].Item1;
			m_defendKeys[i].Spr.Position = m_keyDirs[i].Item2;
			m_defendKeys[i].Spr.Visible = true;

			m_defendKeys[i].Spr.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		
	}
}
