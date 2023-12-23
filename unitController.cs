using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitController : MonoBehaviour
{
    public bool enemy = false;

    private int stepCounter = 0;
    private int maxSteps = 0;

    public int health;
    public int attack;

    private int x;
    private int y;

    private int myID;

    private int currentTurn = 0;
    private int totalTurns = 0;
    void Update()
    {
    }
    public void setStats(int setHealth, int setAttack)
    {
        health = setHealth;
        attack = setAttack;
    }

    public void setCoord(int SetX, int SetY)
    {
        x = SetX;
        y = SetY;
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }

    public int getCurrentTurn()
    {
        return currentTurn;
    }

    public int getTotalTurns()
    {
        return totalTurns;
    }

    public void setTotalTurns(int turns)
    {
        totalTurns = turns;
    }

    public void setCurrentTurns(int turns)
    {
        currentTurn = turns;
    }

    public void addTurn()
    {
        Debug.Log("Adding Turn.");
        currentTurn++;
        Debug.Log("Turn is now: " + currentTurn);
    }
    public void resetTurns()
    {
        currentTurn = 0;
    }

    public void setMyID(int ID)
    {
        myID = ID;
    }

    public void resetStats()
    {
        setStats(((myID % 1000) / 10000), ((myID % 100) / 100));
        setTotalTurns(myID % 10);
        setCurrentTurns((myID % 100) / 10);
    }

    public int getHealth()
    {
        return health;
    }

    public int getAttack()
    {
        return attack;
    }

    public void setMaxSteps(int i)
    {
        maxSteps = i;
    }

    public int getStep()
    {
        return stepCounter;
    }

    public void setStep(int i)
    {
        stepCounter = i;
    }

    public void addStep()
    {
        Debug.Log(stepCounter);
        stepCounter++;
        Debug.Log(stepCounter);
    }

    public bool buildCheck()
    {
        return (stepCounter >= maxSteps);
    }
}



/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class units : MonoBehaviour
{
    public GameObject unit0;
    public GameObject builder;

    public GameObject move;
    public GameObject attack;

    public GameObject[] moves;

    public int width = 20;
    public int height = 20;

    GameObject clickedUnit;
    int unitX;
    int unitY;

    GameObject clickedBuilder;
    int builderX;
    int builderY;

    GameObject clickedMove;
    int moveX;
    int moveY;

    public int[,] unit; //Type, Health, Attack, Current Turn, Total Turns. (0000000, T HHA ATT)

    void Start()
    {
        moves = new GameObject[4];

        height = this.GetComponent<main>().getHeight();
        width = this.GetComponent<main>().getWidth();

        unit = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                unit[x, y] = 0;
            }
        }
    }
    public void unitUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Unit" && clickedUnit == null)
                {
                    Debug.Log("Detected Click");
                    clickedUnit = hit.transform.gameObject;
                    unitX = clickedUnit.GetComponent<unitController>().getX();
                    unitY = clickedUnit.GetComponent<unitController>().getY();

                    if (clickedUnit.GetComponent<unitController>().getCurrentTurn() != clickedUnit.GetComponent<unitController>().getTotalTurns())
                    {
                        showMoves(unitX, unitY);
                    }
                }

                else if (hit.transform.tag == "Move")
                {
                    clickedMove = hit.transform.gameObject;
                    moveX = clickedMove.GetComponent<button>().getX();
                    moveY = clickedMove.GetComponent<button>().getY();

                    unit[unitX, unitY] += 10;
                    clickedUnit.GetComponent<unitController>().resetStats();


                    moveUnit(unitX, unitY, moveX, moveY);
                    refreshUnits();
                    hideMoves();
                }

                else if (hit.transform.tag == "Builder" && clickedBuilder == null)
                {
                    clickedBuilder = hit.transform.gameObject;
                    builderX = clickedBuilder.GetComponent<unitController>().getX();
                    builderY = clickedBuilder.GetComponent<unitController>().getY();

                    if (clickedBuilder.GetComponent<unitController>().getCurrentTurn() != clickedBuilder.GetComponent<unitController>().getTotalTurns())
                    {
                        showMoves(builderX, builderY);
                    }
                }

                else if (hit.transform.tag == "Move")
                {
                    clickedMove = hit.transform.gameObject;
                    builderX = clickedMove.GetComponent<button>().getX();
                    builderY = clickedMove.GetComponent<button>().getY();

                    unit[builderX, builderY] += 10;
                    clickedBuilder.GetComponent<unitController>().resetStats();


                    moveUnit(builderX, builderY, moveX, moveY);
                    refreshUnits();
                    hideMoves();
                }

                else
                {
                    clickedUnit = null;
                    clickedBuilder = null;
                    hideMoves();
                }
            }
        }
    }

    public void spawnUnit(int x, int y, int setID)
    {
        unit[x, y] = setID;
    }

    public void refreshUnits()
    {
        GameObject[] deleteList = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if ((unit[x, y] / 1000000) == 1)
                {
                    GameObject spawn = Instantiate(unit0, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    spawn.GetComponent<unitController>().setMyID(unit[x, y]);
                    spawn.GetComponent<unitController>().resetStats();
                    spawn.GetComponent<unitController>().setCoord(x, y);
                }

                if ((unit[x, y] / 1000000) == 2)
                {
                    GameObject spawn = Instantiate(builder, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    spawn.GetComponent<unitController>().setMyID(unit[x, y]);
                    spawn.GetComponent<unitController>().resetStats();
                    spawn.GetComponent<unitController>().setCoord(x, y);
                }
            }
        }

        this.GetComponent<main>().checkFog();
        this.GetComponent<main>().refreshFog();
    }

    public void showMoves(int x, int y)
    {
        if (this.GetComponent<main>().getTile(x, y + 1) != 0 && unit[x, y + 1] == 0 && this.GetComponent<main>().getAddon(x, y + 1) == 0 && this.GetComponent<structures>().getStructure(x, y + 1) == 0)
        {
            moves[0] = Instantiate(move, new Vector3((x * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[0].transform.Rotate(90, 0, 0);
            moves[0].GetComponent<button>().setCoord(x, y + 1);
        }

        if (this.GetComponent<main>().getTile(x + 1, y) != 0 && unit[x + 1, y] == 0 && this.GetComponent<main>().getAddon(x + 1, y) == 0 && this.GetComponent<structures>().getStructure(x + 1, y) == 0)
        {
            moves[1] = Instantiate(move, new Vector3(((x + 1) * 20), 5, (y * 20)), Quaternion.identity);
            moves[1].transform.Rotate(90, 0, 0);
            moves[1].GetComponent<button>().setCoord(x + 1, y);
        }

        if (this.GetComponent<main>().getTile(x, y - 1) != 0 && unit[x, y - 1] == 0 && this.GetComponent<main>().getAddon(x, y - 1) == 0 && this.GetComponent<structures>().getStructure(x, y - 1) == 0)
        {
            moves[2] = Instantiate(move, new Vector3((x * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[2].transform.Rotate(90, 0, 0);
            moves[2].GetComponent<button>().setCoord(x, y - 1);
        }

        if (this.GetComponent<main>().getTile(x - 1, y) != 0 && unit[x - 1, y] == 0 && this.GetComponent<main>().getAddon(x - 1, y) == 0 && this.GetComponent<structures>().getStructure(x - 1, y) == 0)
        {
            moves[3] = Instantiate(move, new Vector3(((x - 1) * 20), 5, (y * 20)), Quaternion.identity);
            moves[3].transform.Rotate(90, 0, 0);
            moves[3].GetComponent<button>().setCoord(x - 1, y);
        }

        if (this.GetComponent<main>().getTile(x - 1, y + 1) != 0 && unit[x - 1, y + 1] == 0 && this.GetComponent<main>().getAddon(x - 1, y + 1) == 0 && this.GetComponent<structures>().getStructure(x - 1, y + 1) == 0)
        {
            moves[0] = Instantiate(move, new Vector3(((x - 1) * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[0].transform.Rotate(90, 0, 0);
            moves[0].GetComponent<button>().setCoord(x - 1, y + 1);
        }

        if (this.GetComponent<main>().getTile(x + 1, y + 1) != 0 && unit[x + 1, y + 1] == 0 && this.GetComponent<main>().getAddon(x + 1, y + 1) == 0 && this.GetComponent<structures>().getStructure(x + 1, y + 1) == 0)
        {
            moves[1] = Instantiate(move, new Vector3(((x + 1) * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[1].transform.Rotate(90, 0, 0);
            moves[1].GetComponent<button>().setCoord(x + 1, y + 1);
        }

        if (this.GetComponent<main>().getTile(x + 1, y - 1) != 0 && unit[x + 1, y - 1] == 0 && this.GetComponent<main>().getAddon(x + 1, y - 1) == 0 && this.GetComponent<structures>().getStructure(x + 1, y - 1) == 0)
        {
            moves[2] = Instantiate(move, new Vector3(((x + 1) * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[2].transform.Rotate(90, 0, 0);
            moves[2].GetComponent<button>().setCoord(x + 1, y - 1);
        }

        if (this.GetComponent<main>().getTile(x - 1, y - 1) != 0 && unit[x - 1, y - 1] == 0 && this.GetComponent<main>().getAddon(x - 1, y - 1) == 0 && this.GetComponent<structures>().getStructure(x - 1, y - 1) == 0)
        {
            moves[3] = Instantiate(move, new Vector3(((x - 1) * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[3].transform.Rotate(90, 0, 0);
            moves[3].GetComponent<button>().setCoord(x - 1, y - 1);
        }
    }

    public void hideMoves()
    {
        GameObject[] deleteList = GameObject.FindGameObjectsWithTag("Move");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }
    }

    public void moveUnit(int oldX, int oldY, int newX, int newY)
    {
        unit[newX, newY] = unit[oldX, oldY];
        unit[oldX, oldY] = 0;
    }

    public int getUnit(int x, int y)
    {
        return unit[x, y];
    }

    public void resetTurns()
    {
        GameObject[] unitList = GameObject.FindGameObjectsWithTag("Unit");

        foreach (GameObject index in unitList)
        {
            int x = index.GetComponent<unitController>().getX();
            int y = index.GetComponent<unitController>().getY();

            int currentTurn = (unit[x, y] % 100) / 10;

            for (int i = 0; i < currentTurn; i++)
            {
                unit[x, y] -= 10;
            }

            refreshUnits();
        }
    }

    public float distance(int x1, int y1, int x2, int y2)
    {
        int a = Mathf.Abs(x1 - x2);
        int b = Mathf.Abs(y1 - y2);
        float c = Mathf.Sqrt((b * b) + (a * a));

        return c;
    }
}*/

