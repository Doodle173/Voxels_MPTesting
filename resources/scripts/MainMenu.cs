using Godot;
using System;

public partial class MainMenu : Control
{

	private ENetMultiplayerPeer peer;

	private int port = 8910;
	private string ip = "127.0.0.1";

	public override void _Ready()
	{
		Multiplayer.PeerConnected += _peer_connected;
		Multiplayer.PeerDisconnected += _peer_disconnected;
		Multiplayer.ConnectedToServer += _connected_to_server;
		Multiplayer.ConnectionFailed += _connection_failed;
	}

	/// <summary>
	/// 
	/// Clientside only.
	/// 
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	private void _connection_failed()
	{
		GD.Print("DBG> Connection failed");
	}

	/// <summary>
	/// 
	/// Clientside only
	/// 
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	private void _connected_to_server()
	{
		GD.Print("DBG> Connection to server was successful");

		var name = GetNode<LineEdit>("LineEdit").Text;
		var id = Multiplayer.GetUniqueId();

		RpcId(1, "_send_player_info", name, id);
	}


	/// <summary>
	/// 
	/// Player disconnect event
	/// 
	/// </summary>
	/// <param name="id"></param> Id of the player that disconnected
	/// <exception cref="NotImplementedException"></exception>
	private void _peer_disconnected(long id)
	{
		GD.Print("DBG> Player {" + id.ToString() + "} has disconnected.");
	}

	/// <summary>
	/// Player connection event
	/// </summary>
	/// <param name="id"></param> Id of the player that connected
	/// <exception cref="NotImplementedException"></exception>
	private void _peer_connected(long id)
	{
		GD.Print("DBG> Player {" + id.ToString() + "} has connected.");
		
	}

	private void _on_host_pressed()
	{
		GD.Print("Host Button Pressed");
		this.peer = new ENetMultiplayerPeer();
		var err = peer.CreateServer(port, 4);
		if (err != Error.Ok)
		{
			GD.Print("DBG> Failed to create a server: ");
			GD.Print(err);
			return;
		}

		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = this.peer;
		GD.Print("Server startup succesful. Waiting for players.");

		_send_player_info(GetNode<LineEdit>("LineEdit").Text, 1);

	}




	private void _on_join_pressed()
	{
		GD.Print("DBG> Initializing client");
		peer = new ENetMultiplayerPeer();
		peer.CreateClient(ip, port);
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = this.peer;
		GD.Print("DBG> Attempting to join a server.");
	}



	private void _on_start_pressed()
	{
		Rpc("_start_game");
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void _start_game(){

		foreach(var i in GameManager.players){
			GD.Print("DBG> Player: " + i.name + "is playing.");
		}

		var scene = ResourceLoader.Load<PackedScene>("res://game.tscn").Instantiate();
		GetTree().Root.AddChild(scene);
		this.Hide();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void _send_player_info(string username, int id){
		Player player = new Player(username, id);

		if(!GameManager.players.Contains(player)){
			GameManager.players.Add(player);
		}

		if(Multiplayer.IsServer()){
			foreach(var i in GameManager.players){
				Rpc("_send_player_info", username, id);
			}
		}
	}

}



