using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

	public static Manager instance;

    public GameObject endPanel; 

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

    public Image[] myFlag;
    public Image[] opponentFlags;
    Image currentFlag;
    float currentFlagRotation;

	public GameColors[] targetColors;
	public GameObject nextBallsWidget;
	NextBallsWidget nextBallsWidgetScript;

	int numGreen = 0;
	int numPurple = 0;
	int numOrange = 0;

	int numCreatures = 0;

	public int hexRadius = 3;
	public int currentTurn = 0;
    public int playerId = -1;

	public int nextShotIndex;
	public Shot.ShotParams[] nextShots;

	public GameObject winningAnimation;
	public Text winningText;
	bool isFinished = false;
	public int framesDelayOnFinish = 120;

	// Map hexagon-cube coordinates to creatures:
	Creature[, ,] coordToCreature;

	// Map creaturee to hexagon-cube coordinates:
	Dictionary<Creature, int[]> creatureToCoord = new Dictionary<Creature, int[]> ();

	// Use this for initialization
	void Awake ()
	{
        if (instance == null)
        {
            instance = this;
        }

        Vector3 positionFix = new Vector3(0, -2, 0);
        endPanel.SetActive(false);
		initWorld (hexRadius);
		if (PhotonNetwork.inRoom)
		{
            playerId = (int)PhotonNetwork.player.customProperties["playerId"];
			Vector3 pos = Quaternion.Euler(0, 120 * this.playerId, 0) * Camera.main.transform.position;
			Camera.main.transform.position = pos;
            Camera.main.transform.rotation = Quaternion.LookRotation(positionFix - Camera.main.transform.position, Vector3.up);
		}
        else
        {
            playerId = 0;
            Camera.main.transform.rotation = Quaternion.LookRotation(positionFix  - Camera.main.transform.position, Vector3.up);
        }
        this.setFlagColors();
        this.setCurrentFlag();
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
        float rot = Time.deltaTime * 50;
        currentFlagRotation += rot;
        this.currentFlag.transform.Rotate(new Vector3(0, 0, 1), rot);
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
        return (((this.playerId - currentTurn) % 3) + 3) % 3;
    }

    void setCurrentFlag()
    {
        if (this.currentFlag != null)
        {
            this.currentFlag.transform.Rotate(new Vector3(0, 0, 1), -this.currentFlagRotation);
            this.currentFlagRotation = 0;
        }
        int numPlayers = PhotonNetwork.playerList.Length;
        switch ((this.currentTurn - this.playerId + numPlayers) % numPlayers)
        {
            case 0:
                this.currentFlag = this.myFlag[0];
                break;
            case 1:
                this.currentFlag = this.opponentFlags[1];
                break;
            case 2:
                this.currentFlag = this.opponentFlags[0];
                break;
        }
    }

    void setFlagColors()
    {
        GameColors myColor = this.targetColors[this.playerId];
        this.myFlag[0].color = myColor.GetColor();
        this.myFlag[1].color = myColor.getPrimaries()[0].GetColor();
        this.myFlag[2].color = myColor.getPrimaries()[1].GetColor();

        this.opponentFlags[1].color = this.targetColors[(this.playerId + 1) % this.targetColors.Length].GetColor();
        this.opponentFlags[0].color = this.targetColors[(this.playerId + 2) % this.targetColors.Length].GetColor();
    }

    public void ReturnToLobby()
    {
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        Application.LoadLevel("Lobby");
    }

    public void updateTurn()
    {
        --numTurns;
		checkFinish ();

        turnsText.text = string.Format("{0}", numTurns);

        currentTurn = (currentTurn + 1) % PhotonNetwork.playerList.Length;
        this.setCurrentFlag();
    }

    public bool isMyTurn()
    {
        return (currentTurn == this.playerId && !isFinished);
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

	public void checkFinish(){
		print ("check finish\tturns " + numTurns + "\tpurple " + numPurple
		       + "\tgreen " + numGreen + "\torange " + numOrange);
		if (numTurns <= 0)
		{
			if(numGreen > numOrange && numGreen > numPurple) //green win
				finish(GameColors.Green);
			if(numPurple > numOrange && numPurple > numGreen) //purple win
				finish(GameColors.Purple);
			if(numOrange > numPurple && numOrange > numGreen) //orange win
				finish(GameColors.Orange);
		}
	}
	
	//called when game ends
	void finish(GameColors winningColor) {
		isFinished = true;

		//fireworks
		for (int i = 0; i < 3; i++) {
			GameObject winEffect = Instantiate (winningAnimation);
			winEffect.transform.position = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 1);
		}
		
		//text and continue button
		winningText.text = (winningColor.ToString() + " wins!");


        endPanel.SetActive(true);
		
		
	}
}
