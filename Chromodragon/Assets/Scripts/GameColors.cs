using System;
using UnityEngine;

public enum GameColors
{
	White,
	Black,
	Blue,
	Red,
	Yellow,
	Green,
	Purple,
	Orange
}

public static class GameColorsExtensions
{
	public static GameColors Add (this GameColors color, GameColors colorToAdd)
	{
		switch (colorToAdd) {
		case GameColors.Blue:
			return color.AddBlue ();
		case GameColors.Red:
			return color.AddRed ();
		case GameColors.Yellow:
			return color.AddYellow ();
		default:
			return color;
		}
	}

	public static GameColors AddBlue (this GameColors color)
	{
		switch (color) {
		case GameColors.White:
			return GameColors.Blue;
		case GameColors.Red:
			return GameColors.Purple;
		case GameColors.Yellow:
			return GameColors.Green;
		default:
			return color;
		}
	}

	public static GameColors AddRed (this GameColors color)
	{
		switch (color) {
		case GameColors.White:
			return GameColors.Red;
		case GameColors.Blue:
			return GameColors.Purple;
		case GameColors.Yellow:
			return GameColors.Orange;
		default:
			return color;
		}
	}

	public static GameColors AddYellow (this GameColors color)
	{
		switch (color) {
		case GameColors.White:
			return GameColors.Yellow;
		case GameColors.Blue:
			return GameColors.Green;
		case GameColors.Red:
			return GameColors.Orange;
		default:
			return color;
		}
	}

	public static bool HasBlue (this GameColors color)
	{
		return color == GameColors.Blue || color == GameColors.Purple || color == GameColors.Green;
	}

	public static bool HasRed (this GameColors color)
	{
		return color == GameColors.Red || color == GameColors.Purple || color == GameColors.Orange;
	}

	public static bool HasYellow (this GameColors color)
	{
		return color == GameColors.Yellow || color == GameColors.Orange || color == GameColors.Green;
	}

	public static Color GetColor (this GameColors color)
	{
		return ColorsManager.colorMap [color];
	}

	public static bool IsRivalColor (this GameColors color, GameColors otherColor)
	{
		return (color == GameColors.Green && otherColor == GameColors.Red) || 
			(color == GameColors.Orange && otherColor == GameColors.Blue) ||
			(color == GameColors.Purple && otherColor == GameColors.Yellow);
	}

    public static GameColors[] getPrimaries(this GameColors color){
        GameColors[] primaries;

        if (color == GameColors.Green)
        {
            primaries = new GameColors[2] { GameColors.Blue, GameColors.Yellow };
        }
        else if (color == GameColors.Purple)
        {
            primaries = new GameColors[2] { GameColors.Blue, GameColors.Red };
        }
        else if (color == GameColors.Orange)
        {
            primaries = new GameColors[2] { GameColors.Yellow, GameColors.Red };
        }
        else
        {
            primaries = new GameColors[1] { color };
        }

        return primaries;
    }
}