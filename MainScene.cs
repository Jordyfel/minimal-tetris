using Godot;
using System;

public partial class MainScene : Control
{
    [Export]
    private ObjectShape[] _objectShapes;

    [Export]
    private Field _field;

    [Export]
    private Label _lossLabel;

    [Export]
    private Button _resetButton;

    private Random _rand = new Random();

    public override void _Ready()
    {
        Game();
        _resetButton.Pressed += OnButtonPressed;
    }

    public async void Game()
    {
        while (true)
        {
            ObjectShape shape = _objectShapes[_rand.Next(_objectShapes.Length)];

            Vector2[] newPoints = new Vector2[shape.Points.Length];
            Array.Copy(shape.Points, newPoints, shape.Points.Length);

            FallingObject fallingObject = new FallingObject(newPoints, shape.Color);
            _field.AddChild(fallingObject);

            await ToSignal(fallingObject, FallingObject.SignalName.Fell);

            foreach (Vector2 gridPoint in fallingObject.GridPosition)
            {
                _field.AddBlock((int)gridPoint.X, (int)gridPoint.Y, fallingObject.Color);
                if (gridPoint.Y >= 20)
                {
                    _lossLabel.Visible = true;
                    return;
                }
            }

            fallingObject.QueueFree();
            _field.DestroyRows();
        }
    }

    public void OnButtonPressed()
    {
        foreach (FallingObject child in _field.GetChildren())
        {
            child.GridPosition[0].Y = 20;
            child.EmitSignal(FallingObject.SignalName.Fell);
            child.QueueFree();
        }
        _lossLabel.Visible = false;
        _field.Clear();

        Game();
    }
}
