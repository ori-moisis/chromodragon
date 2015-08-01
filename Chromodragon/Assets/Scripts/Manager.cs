using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

	public static Manager instance;

	public Creature CreaturePrefab;
	public GameObject hexTilePrefab;
	public GameObject creatures;
	public GameObject tiles;

	public Slider greenScoreSlider;
	public Slider purpleScoreSlider;
	public Slider orangeScoreSlider;

    public int numTurns = 100;
    public Text turnsText;

    public Image[] turnImages;
	public GameColors[] targetColors;
	public GameObject nextBallsWidget;
	NextBallsWidget nextBallsWidgetScript;

	int numGreen = 0;
	int numPurple = 0;
	int numOrange = 0;

	int numCreatures = 0;

	public int hexRadius = 3;
	public int currentTurn = 0;

	public int nextShotIndex;
	public Shot.ShotParams[] nextShots;

	// Map hexagon-cube coordinates to creatures:
	Creature[, ,] coordToCreature;

	// Map creaturee to hexagon-cube coordinates:
	Dictionary<Creature, int[]> creatureToCoord = new Dictionary<Creature, int[]> ();

	// Use this for initialization
	void Awake ()
	{
        Vector3 positionFix = new Vector3(0, -2, 0);

		if (instance == null) {
			instance = this;
		}
		initWorld (hexRadius);
		if (PhotonNetwork.inRoom)
		{
			Vector3 pos = Quaternion.Euler(0, 120 * (PhotonNetwork.player.ID - 1), 0) * Camera.main.transform.position;
			Camera.main.transform.position = pos;
            Camera.main.transform.rotation = Quaternion.LookRotation(positionFix - Camera.main.transform.position, Vector3.up);
			setCurrentTurnImgColor(true);
		}
        else
        {
            Camera.main.transform.rotation = Quaternion.LookRotation(positionFix  - Camera.main.transform.position, Vector3.up);
        }
		nextBallsWidgetScript = nextBallsWidget.GetComponent<NextBallsWidget> ();
        nextShots = new Shot.ShotParams[3];
        for (int i = 0; i < nextShots.Length; ++i)
        {
            nextShots[i] = new Shot.ShotParams();
        }

        turnsText.text = string.Format("{0}", numTurns);
	}

	
	// Update is called once per frame
	void Update ()
	{
	    
	}


	public List<Creature> getNeighbours (Creature creature)
	{
		var neighbours = new List<Creature> ();
		int[] coord = creatureToCoord [creature];

		if (coord [0] > 0) {
			neighbours.Add (coordToCreature [coord [0] - 1, coord [1], coord [2]]);
		}
		if (coord [0] < hexRadius) {
			neighbours.Add (coordToCreature [coord [0] + 1, coord [1], coord [2]]);
		}

		if (coord [1] > 0) {
			neighbours.Add (coordToCreature [coord [0], coord [1] - 1, coord [2]]);
		}
		if (coord [1] < hexRadius) {
			neighbours.Add (coordToCreature [coord [0], coord [1] + 1, coord [2]]);
		}

		if (coord [2] > 0) {
			neighbours.Add (coordToCreature [coord [0], coord [1], coord [2] - 1]);
		}
		if (coord [2] < hexRadius) {
			neighbours.Add (coordToCreature [coord [0], coord [1], coord [2] + 1]);
		}

		return neighbours;
	}

    int currentTurnIndex()
    {
        return (((PhotonNetwork.player.ID - currentTurn - 1) % 3) + 3) % 3;
    }

    void setCurrentTurnImgColor(bool isSet)
    {
        if (PhotonNetwork.inRoom)
        {
            GameColors[] primaries = this.targetColors[currentTurn].getPrimaries();

            Color newColor = ColorsManager.colorMap[this.targetColors[currentTurn]];
            Color primary1 = ColorsManager.colorMap[primaries[0]];
            Color primary2 = ColorsManager.colorMap[primaries[1]];

            newColor.a = isSet ? 10 : 0;
            primary1.a = isSet ? 10 : 0;
            primary2.a = isSet ? 10 : 0;

            this.turnImages[currentTurnIndex()].color = newColor;
            this.turnImages[3].color = primary1;
            this.turnImages[4].color = primary2;
        }
    }

    public void updateTurn()
    {
        --numTurns;
        if (numTurns == 0)
        {

        }

        turnsText.text = string.Format("Turns Remaining: {0}", numTurns);

        setCurrentTurnImgColor(false);
        currentTurn = (currentTurn + 1) % PhotonNetwork.playerList.Length;
        setCurrentTurnImgColor(true);
    }

    public bool isMyTurn()
    {
        return (currentTurn + 1) == PhotonNetwork.player.ID;
    }

	public void updateScore (GameColors prevColor, GameColors newColor)
	{
		if (prevColor == GameColors.Green) {
			numGreen--;
		} else if (prevColor == GameColors.Purple) {
			numPurple--;
		} else if (prevColor == GameColors.Orange) {
			numOrange--;
		}

		if (newColor == GameColors.Green) {
			numGreen++;
		} else if (newColor == GameColors.Purple) {
			numPurple++;
		} else if (newColor == GameColors.Orange) {
			numOrange++;
		}

        int numMixed = Mathf.Max(1, numGreen + numPurple + numOrange) ;

        greenScoreSlider.value = (float)(numGreen) / numMixed;
        purpleScoreSlider.value = (float)(numPurple) / numMixed;
        orangeScoreSlider.value = (float)(numOrange) / numMixed;
	}

	public Shot.ShotParams GetNextShot()
	{
		Shot.ShotParams next = this.nextShots[this.nextShotIndex];
		this.nextShots[this.nextShotIndex] = new Shot.ShotParams();
		this.nextShotIndex = (this.nextShotIndex + 1) % this.nextShots.Length;

		//update widget
		nextBallsWidgetScript.dropNextBall();

		return next;
	}

	// Initiates the creatures in the world according to the specified radius (number of creatures in each axis excluding the middle one)
	void initWorld (int gridRadius)
	{
		coordToCreature = new Creature[gridRadius + 1, gridRadius + 1, gridRadius + 1];

		// Create creatures and tiles:
		for (int x = 0; x <= gridRadius; ++x) {
			for (int y = 0; y <= gridRadius; ++y) {
				for (int z = 0; z <= gridRadius; ++z) {
					if (x == 0 || y == 0 || z == 0) {
						// Calculate world coordinates from cube-hexagon coordinates:
						float newX = (x - Mathf.Cos (Mathf.PI / 3) * (y + z));
						float newY = 0* (Random.value * 0.15f);
						float newZ = (Mathf.Sin (Mathf.PI / 3) * (y - z));

                        //if (x != gridRadius && y != gridRadius && z != gridRadius)
                        //{
                            // Create a new creature:
                            Creature newCreature = Instantiate(CreaturePrefab) as Creature;
                            newCreature.transform.parent = creatures.transform;
                            newCreature.transform.position = new Vector3(newX, newY, newZ);
                            Debug.Log(newZ);

                            // add creature to data structures
                            coordToCreature[x, y, z] = newCreature;
                            int[] coord = new int[3] { x, y, z };
                            creatureToCoord.Add(newCreature, coord);
                        //}


						// Create a new tile:
						GameObject newTile = Instantiate (hexTilePrefab);
						newTile.transform.parent = tiles.transform;
						newTile.transform.position = new Vector3 (newX, newY, newZ);


						++numCreatures;
					}
				}
			}
		}
	}
}
