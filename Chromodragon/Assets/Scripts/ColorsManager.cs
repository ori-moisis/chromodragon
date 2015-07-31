using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorsManager : MonoBehaviour
{
	public Color blue;
	public Color red;
	public Color yellow;
	public Color purple;
	public Color green;
	public Color orange;

	public static Dictionary<GameColors, Color> colorMap;

	#if UNITY_EDITOR
	protected void OnDrawGizmos()
	{
		Awake();
	}
	#endif

	protected void Awake()
	{
		if (colorMap == null) {
			colorMap = new Dictionary<GameColors, Color>();
			colorMap[GameColors.White] = Color.white;
			colorMap[GameColors.Blue] = blue;
			colorMap[GameColors.Red] = red;
			colorMap[GameColors.Yellow] = yellow;
			colorMap[GameColors.Purple] = purple;
			colorMap[GameColors.Green] = green;
			colorMap[GameColors.Orange] = orange;
			colorMap[GameColors.Black] = Color.black;
		}
	}
}
