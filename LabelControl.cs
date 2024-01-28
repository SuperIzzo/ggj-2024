using Godot;
using System;
using System.Collections.Generic;

public partial class LabelControl : Control
{
	private Label Screentext;
	private List<String> randomEvents;
	private double timer = 0;
	[Export]
	double time = 10;
	private bool isTextClear = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Screentext = GetNode<Label>("RandomEventGenerator");
		randomEvents = new List<string>();
//region Events list.
		randomEvents.Add("You no longer have your horse.");
		randomEvents.Add("A mischievous monkey steals your fencing gear.");
		randomEvents.Add("A specific portal opens.");
		randomEvents.Add("Perception check failed.");
		randomEvents.Add("Larry will remember this.");
		randomEvents.Add("The blood moon has risen.");
		randomEvents.Add("Congratulations, it's a boy!");
		randomEvents.Add("2x multiplier!");
		randomEvents.Add("The referee has declared a penalty shot.");
		randomEvents.Add("Touché.");
		randomEvents.Add("Genie disappears; two wishes remaining.");
		randomEvents.Add("You're out of ammo");
		randomEvents.Add("Kasahune is nearby");
		randomEvents.Add("There is a war going on for your mind.");
		randomEvents.Add("I sent you my trunk please respond.");
		randomEvents.Add("Годо е много глупав.");
		randomEvents.Add("Освободи ме от този плътски затвор.");
		randomEvents.Add("No me importa un pepino.");
		randomEvents.Add("Слон е починал.");
//endregion
		UpdateRandomEvent();
		var delay = GetNode<Timer>("eventDelay");
		
		delay.WaitTime = 6.0f;
		delay.Timeout += () => UpdateRandomEvent();
	}
	
	public void UpdateRandomEvent()
	{
		if(randomEvents.Count > 0)
		{
			int randomIndex = new Random().Next(0, randomEvents.Count);
			Screentext.Text = randomEvents[randomIndex];
			isTextClear = false;
		}
	}
	
	public void ClearEventText()
	{
		if(isTextClear == false)
		{
			Screentext.Text = "";
			isTextClear = true;
			
		}
	}
	
	public void delayEvent(){
		var delay = GetNode<Timer>("eventDelay");
		//delay.Timeout += () => UpdateRandomEvent()
		delay.Start();
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		timer += delta;
		if (timer >= time)
		{
			timer = 0;
			ClearEventText();
			delayEvent();
		}
	}
}
