using Godot;
using System;
using System.Drawing;
using System.Linq;

public partial class FallingObject : Node2D
{
    [Signal]
    public delegate void FellEventHandler();

    private Field _field;

    public const float FallTime = 1.0f;

    public Vector2[] GridPosition { get; private set; }

    public Godot.Color Color { get; private set; }

    public override void _Ready()
    {
        _field = GetParent<Field>();
        for (int i = 0; i < GridPosition.Length; i++)
        {
            GridPosition[i] = new Vector2(GridPosition[i].X + 4, GridPosition[i].Y + 20);
        }

        FallCycle();
    }

    public async void FallCycle()
    {
        while (true)
        {
            await ToSignal(GetTree().CreateTimer(FallTime), SceneTreeTimer.SignalName.Timeout);

            Fall();
        }
    }

    private void Fall()
    {
        foreach (Vector2 gridPoint in GridPosition)
        {
            if (!_field.IsValidDestination((int)gridPoint.X, (int)gridPoint.Y - 1))
            {
                EmitSignal(SignalName.Fell);
                return;
            }
        }

        for (int i = 0; i < GridPosition.Length; i++)
        {
            GridPosition[i] = GridPosition[i] with {Y = GridPosition[i].Y - 1};
        }

        QueueRedraw();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("left"))
        {
            foreach (Vector2 gridPoint in GridPosition)
            {
                if (!_field.IsValidDestination((int)gridPoint.X - 1, (int)gridPoint.Y))
                {
                    return;
                }
            }

            for (int i = 0; i < GridPosition.Length; i++)
            {
                GridPosition[i] = GridPosition[i] with {X = GridPosition[i].X - 1};
            }

            QueueRedraw();
        } else if (@event.IsActionPressed("right"))
        {
            foreach (Vector2 gridPoint in GridPosition)
            {
                if (!_field.IsValidDestination((int)gridPoint.X + 1, (int)gridPoint.Y))
                {
                    return;
                }
            }

            for (int i = 0; i < GridPosition.Length; i++)
            {
                GridPosition[i] = GridPosition[i] with {X = GridPosition[i].X + 1};
            }

            QueueRedraw();
        } else if (@event.IsActionPressed("drop"))
        {
            Vector2 last_point = new Vector2();
            while (last_point != GridPosition[0])
            {
                last_point = GridPosition[0];
                Fall();
            }

        } else if (@event.IsActionPressed("rotate"))
        {
            // Arbitrarily chose to rotate around the second block.
            Vector2 centralBlock = GridPosition[1];
            Vector2[] localCoords = new Vector2[GridPosition.Length];
            for (int i = 0; i < GridPosition.Length; i++)
            {
                localCoords[i] = GridPosition[i] - centralBlock;
            }

            // Do the rotation and test.
            for (int i = 0; i < localCoords.Length; i++)
            {
                localCoords[i] = localCoords[i].Rotated((float)Math.PI / 2.0f).Round();
                Vector2 result = centralBlock + localCoords[i];
                if (!_field.IsValidDestination((int)result.X, (int)result.Y))
                {
                    return;
                }
            }

            // Apply the result.
            for (int i = 0; i < GridPosition.Length; i++)
            {
                GridPosition[i] = centralBlock + localCoords[i];
            }

            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        foreach (Vector2 point in GridPosition)
        {
            _field.BlockStyleBox.BgColor = Color;
            DrawStyleBox(_field.BlockStyleBox, _field.GridToRect((int)point.X, (int)point.Y));
        }
    }

    public FallingObject(in Vector2[] p_points, Godot.Color p_color)
    {
        GridPosition = p_points;
        Color = p_color;
    }
}
