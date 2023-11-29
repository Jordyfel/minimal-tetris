using Godot;
using System;

public partial class Field : Node2D
{
    private Color?[,] _grid = new Color?[22, 10];

    [Export]
    public Vector2 CellSize {get; set;}

    [Export]
    public StyleBoxFlat BlockStyleBox {get; set;}

    public override void _Draw()
    {
        base._Draw();

        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j< _grid.GetLength(1); j++)
            {
                BlockStyleBox.BgColor = _grid[i, j] ?? new Color(0, 0, 0, 0.1f);
                DrawStyleBox(BlockStyleBox, GridToRect(j, i));
            }
        }
    }

    public void AddBlock(int x, int y, Color color)
    {
        _grid[y, x] = color;
        QueueRedraw();
    }

    public Rect2 GridToRect(int x, int y)
    {
        return new Rect2(new Vector2(CellSize.X * x, -CellSize.Y * (y + 1)), CellSize);
    }

    public bool IsValidDestination(int x, int y)
    {
        if (x >= _grid.GetLength(1) || y >= _grid.GetLength(0) || x < 0 || y < 0)
        {
            return false;
        }

        return !_grid[y, x].HasValue;
    }

    public void DestroyRows()
    {
        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            bool full = true;
            for (int j = 0; j < _grid.GetLength(1); j++)
            {
                if (!_grid[i, j].HasValue)
                {
                    full = false;
                }
            }

            
            if (full) {
                for (int j = i; j < _grid.GetLength(0) - 1; j++)
                {
                    for (int k = 0; k < _grid.GetLength(1); k++)
                    {
                        _grid[j, k] = _grid[j + 1, k];
                    }
                }

                i -=1;
            }
        }

        QueueRedraw();
    }

    public void Clear()
    {
        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j< _grid.GetLength(1); j++)
            {
                _grid[i, j] = null;
            }
        }

        QueueRedraw();
    }

    public Field()
    {
        Clear();
    }
}
