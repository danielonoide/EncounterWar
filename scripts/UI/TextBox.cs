using Godot;
using System;

public class TextBox : MarginContainer
{
    Vector2 offset=new(30,10);
    private string labelText;

    public string Text 
    {
        get => labelText;
        set
        {
            labelText = value;
            if (label != null)
            {
                label.Text = labelText;
            }
        }
    }
    bool rightSide=true;
    Label label;

    public override void _Ready()
    {
        label=GetNode<Label>("Text/Label");
        label.Text=Text;

        if(!rightSide)
        {
            offset=new Vector2(-317, 10);
        }
    }
    public override void _Process(float delta)
    {
        SetGlobalPosition(GetGlobalMousePosition()+offset);
    }

    public static TextBox GetTextBox(string text, bool rightSide=true)
    {
        PackedScene packedScene=GD.Load<PackedScene>("res://scenes/UI/TextBox.tscn");
        TextBox textBox=packedScene.Instance<TextBox>();
        textBox.Text=text;
        textBox.rightSide=rightSide;

        return textBox;
    }


}
