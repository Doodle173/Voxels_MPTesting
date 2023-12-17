using Godot;

public partial class Player : CharacterBody3D{

    public string name;
    public int id;

    public Player(string uname, int uid){
        this.name = uname;
        this.id = uid;
    }
    

}