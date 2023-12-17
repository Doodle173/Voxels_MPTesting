using Godot;
using System;

public partial class SceneManager : Node
{

	[Export]
	private PackedScene _player_template;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var i in GameManager.players)
		{
			Node3D player = _player_template.Instantiate<Node3D>();
			AddChild(player);

			Random rng = new Random();

			

			int rx = rng.Next(10);
			int rz = rng.Next(10);

			player.GlobalPosition = new Vector3(rx, 5, rz);
			var test = player.Get("Material");
			GD.Print(test);
		}


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
