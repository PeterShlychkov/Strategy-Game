using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Units : MonoBehaviour
{
    public GameObject unit0;
    public GameObject builder;

    public GameObject move;
    public GameObject attack;
    public GameObject build;
    public GameObject attackS;

    public GameObject[] moves;

    public int width = 20;
    public int height = 20;
    public int IDLength = 5;

    GameObject clickedUnit;
    int unitX;
    int unitY;

    GameObject clickedBuilder;
    int builderX;
    int builderY;

    GameObject clickedMove;
    int moveX;
    int moveY;

    bool unitClicked = true;

    public int[,,] unit; //Type, Health, Attack, Current Turn, Total Turns.

    public void unitStart()
    {
        moves = new GameObject[8];

        height = this.GetComponent<main>().getHeight();
        width = this.GetComponent<main>().getWidth();

        unit = new int[width, height, IDLength];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i  = 0; i < IDLength; i++)
                {
                    unit[x, y, i] = 0;
                }
            }
        }
    }
    public void unitUpdate()
    {
        checkLife();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Unit" && clickedUnit == null || hit.transform.tag == "Builder" && clickedBuilder == null)
                {
                    if (hit.transform.tag == "Unit")
                    {
                        unitClicked = true;
                        clickedUnit = hit.transform.gameObject;
                        unitX = clickedUnit.GetComponent<unitController>().getX();
                        unitY = clickedUnit.GetComponent<unitController>().getY();

                        if (unit[unitX, unitY, 3] != unit[unitX, unitY, 4])
                        {
                            showMoves(unitX, unitY, false);
                        }
                    }
                    else
                    {
                        unitClicked = false;
                        clickedBuilder = hit.transform.gameObject;
                        builderX = clickedBuilder.GetComponent<unitController>().getX();
                        builderY = clickedBuilder.GetComponent<unitController>().getY();
                        if (unit[builderX, builderY, 3] != unit[builderX, builderY, 4])
                        {
                            showMoves(builderX, builderY, true);
                        }
                    }
                }

                else if (hit.transform.tag == "Build")
                {
                    int[] spawnID = { 1, 8, 0, 0, 1 };
                    this.GetComponent<structures>().spawnStructure(builderX, builderY, spawnID);
                    this.GetComponent<structures>().refreshStructures();
                    clearTile(builderX, builderY);
                    Destroy(clickedBuilder);
                    hideMoves();
                    this.GetComponent<main>().checkFog();
                    this.GetComponent<main>().refreshFog();
                }

                else if (hit.transform.tag == "Attack")
                {
                    clickedMove = hit.transform.gameObject;
                    moveX = clickedMove.GetComponent<button>().getX();
                    moveY = clickedMove.GetComponent<button>().getY();
                    if (unitClicked)
                    {
                        unit[unitX, unitY, 3] += 1;
                        clickedUnit.GetComponent<unitController>().resetStats();
                        attackUnit(moveX, moveY, unitX, unitY);
                    }
                    else
                    {
                        unit[builderX, builderY, 3] += 1;
                        clickedBuilder.GetComponent<unitController>().resetStats();
                        attackUnit(moveX, moveY, builderX, builderY);
                    }

                    this.GetComponent<enemyAction>().checkLife();
                    this.GetComponent<enemyAction>().refreshUnits();
                    refreshUnits();
                    hideMoves();
                }

                else if (hit.transform.tag == "CityAttack")
                {
                    clickedMove = hit.transform.gameObject;
                    moveX = clickedMove.GetComponent<button>().getX();
                    moveY = clickedMove.GetComponent<button>().getY();
                    if (unitClicked)
                    {
                        unit[unitX, unitY, 3] += 1;
                        clickedUnit.GetComponent<unitController>().resetStats();

                        attackStructure(moveX, moveY, unitX, unitY);
                    }
                    else
                    {
                        unit[builderX, builderY, 3] += 1;
                        clickedBuilder.GetComponent<unitController>().resetStats();

                        attackStructure(moveX, moveY, builderX, builderY);
                    }

                    this.GetComponent<enemyAction>().checkLife();
                    this.GetComponent<enemyAction>().refreshStructures();
                    refreshUnits();
                    hideMoves();
                }

                else if (hit.transform.tag == "Move")
                {
                    if (unitClicked == true)
                    {
                        clickedMove = hit.transform.gameObject;
                        moveX = clickedMove.GetComponent<button>().getX();
                        moveY = clickedMove.GetComponent<button>().getY();

                        unit[unitX, unitY, 3] += 1;
                        clickedUnit.GetComponent<unitController>().resetStats();


                        moveUnit(unitX, unitY, moveX, moveY);
                        refreshUnits();
                        hideMoves();
                    }
                    else
                    {
                        clickedMove = hit.transform.gameObject;
                        moveX = clickedMove.GetComponent<button>().getX();
                        moveY = clickedMove.GetComponent<button>().getY();

                        unit[builderX, builderY, 3] += 1;
                        clickedBuilder.GetComponent<unitController>().resetStats();

                        moveUnit(builderX, builderY, moveX, moveY);
                        refreshUnits();
                        hideMoves();
                    }
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

    public void spawnUnit(int x, int y, int[] setID)
    {
        for (int i = 0; i < IDLength; i++)
        {
            unit[x, y, i] = setID[i];
        }
    }

    public void refreshUnits()
    {
        GameObject[] deleteList = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }

        deleteList = GameObject.FindGameObjectsWithTag("Builder");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (unit[x, y, 0] == 1)
                {
                    GameObject spawn = Instantiate(unit0, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    spawn.GetComponent<unitController>().resetStats();
                    spawn.GetComponent<unitController>().setCoord(x, y);
                }

                if (unit[x, y, 0] == 2)
                {
                    GameObject spawn = Instantiate(builder, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    spawn.GetComponent<unitController>().resetStats();
                    spawn.GetComponent<unitController>().setCoord(x, y);
                }
            }
        }
        this.GetComponent<main>().checkFog();
        this.GetComponent<main>().refreshFog();
    }

    public void showMoves(int x, int y, bool builder)
    {
        if (this.GetComponent<enemyAction>().getUnit(x, y + 1) != 0)
        {
            moves[0] = Instantiate(attack, new Vector3((x * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[0].transform.Rotate(90, 0, 0);
            moves[0].GetComponent<button>().setCoord(x, y + 1);
        }
        else if (this.GetComponent<enemyAction>().getStructure(x, y + 1) != 0)
        {
            moves[0] = Instantiate(attackS, new Vector3((x * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[0].transform.Rotate(90, 0, 0);
            moves[0].GetComponent<button>().setCoord(x, y + 1);
        }
        else if (this.GetComponent<main>().getTile(x, y + 1) != 0 && unit[x, y + 1, 0] == 0 && this.GetComponent<main>().getAddon(x, y + 1) == 0 && this.GetComponent<structures>().getStructure(x, y + 1) == 0)
        {
            moves[0] = Instantiate(move, new Vector3((x * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[0].transform.Rotate(90, 0, 0);
            moves[0].GetComponent<button>().setCoord(x, y + 1);
        }

        if (this.GetComponent<enemyAction>().getUnit(x + 1, y) != 0)
        {
            moves[1] = Instantiate(attack, new Vector3(((x + 1) * 20), 5, (y * 20)), Quaternion.identity);
            moves[1].transform.Rotate(90, 0, 0);
            moves[1].GetComponent<button>().setCoord(x + 1, y);
        }
        else if (this.GetComponent<enemyAction>().getStructure(x + 1, y) != 0)
        {
            moves[1] = Instantiate(attackS, new Vector3(((x + 1) * 20), 5, (y * 20)), Quaternion.identity);
            moves[1].transform.Rotate(90, 0, 0);
            moves[1].GetComponent<button>().setCoord(x + 1, y);
        }
        else if (this.GetComponent<main>().getTile(x + 1, y) != 0 && unit[x + 1, y, 0] == 0 && this.GetComponent<main>().getAddon(x + 1, y) == 0 && this.GetComponent<structures>().getStructure(x + 1, y) == 0)
        {
            moves[1] = Instantiate(move, new Vector3(((x + 1) * 20), 5, (y * 20)), Quaternion.identity);
            moves[1].transform.Rotate(90, 0, 0);
            moves[1].GetComponent<button>().setCoord(x + 1, y);
        }

        if (this.GetComponent<enemyAction>().getUnit(x, y - 1) != 0)
        {
            moves[2] = Instantiate(attack, new Vector3((x * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[2].transform.Rotate(90, 0, 0);
            moves[2].GetComponent<button>().setCoord(x, y - 1);
        }
        else if (this.GetComponent<enemyAction>().getStructure(x, y - 1) != 0)
        {
            moves[2] = Instantiate(attackS, new Vector3((x * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[2].transform.Rotate(90, 0, 0);
            moves[2].GetComponent<button>().setCoord(x, y - 1);
        }
        else if (this.GetComponent<main>().getTile(x, y - 1) != 0 && unit[x, y - 1, 0] == 0 && this.GetComponent<main>().getAddon(x, y - 1) == 0 && this.GetComponent<structures>().getStructure(x, y - 1) == 0)
        {
            moves[2] = Instantiate(move, new Vector3((x * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[2].transform.Rotate(90, 0, 0);
            moves[2].GetComponent<button>().setCoord(x, y - 1);
        }

        if (this.GetComponent<enemyAction>().getUnit(x - 1, y) != 0)
        {
            moves[3] = Instantiate(attack, new Vector3(((x - 1) * 20), 5, (y * 20)), Quaternion.identity);
            moves[3].transform.Rotate(90, 0, 0);
            moves[3].GetComponent<button>().setCoord(x - 1, y);
        }
        else if (this.GetComponent<enemyAction>().getStructure(x - 1, y) != 0)
        {
            moves[3] = Instantiate(attackS, new Vector3(((x - 1) * 20), 5, (y * 20)), Quaternion.identity);
            moves[3].transform.Rotate(90, 0, 0);
            moves[3].GetComponent<button>().setCoord(x - 1, y);
        }
        else if (this.GetComponent<main>().getTile(x - 1, y) != 0 && unit[x - 1, y, 0] == 0 && this.GetComponent<main>().getAddon(x - 1, y) == 0 && this.GetComponent<structures>().getStructure(x - 1, y) == 0)
        {
            moves[3] = Instantiate(move, new Vector3(((x - 1) * 20), 5, (y * 20)), Quaternion.identity);
            moves[3].transform.Rotate(90, 0, 0);
            moves[3].GetComponent<button>().setCoord(x - 1, y);
        }

        if (this.GetComponent<enemyAction>().getUnit(x - 1, y + 1) != 0)
        {
            moves[4] = Instantiate(attack, new Vector3(((x - 1) * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[4].transform.Rotate(90, 0, 0);
            moves[4].GetComponent<button>().setCoord(x - 1, y + 1);
        }
        else if (this.GetComponent<enemyAction>().getStructure(x - 1, y + 1) != 0)
        {
            moves[4] = Instantiate(attackS, new Vector3(((x - 1) * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[4].transform.Rotate(90, 0, 0);
            moves[4].GetComponent<button>().setCoord(x - 1, y + 1);
        }
        else if (this.GetComponent<main>().getTile(x - 1, y + 1) != 0 && unit[x - 1, y + 1, 0] == 0 && this.GetComponent<main>().getAddon(x - 1, y + 1) == 0 && this.GetComponent<structures>().getStructure(x - 1, y + 1) == 0)
        {
            moves[4] = Instantiate(move, new Vector3(((x - 1) * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[4].transform.Rotate(90, 0, 0);
            moves[4].GetComponent<button>().setCoord(x - 1, y + 1);
        }

        if (this.GetComponent<enemyAction>().getUnit(x + 1, y + 1) != 0)
        {
            moves[5] = Instantiate(attack, new Vector3(((x + 1) * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[5].transform.Rotate(90, 0, 0);
            moves[5].GetComponent<button>().setCoord(x + 1, y + 1);
        }
        else if (this.GetComponent<enemyAction>().getStructure(x + 1, y + 1) != 0)
        {
            moves[5] = Instantiate(attackS, new Vector3(((x + 1) * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[5].transform.Rotate(90, 0, 0);
            moves[5].GetComponent<button>().setCoord(x + 1, y + 1);
        }
        else if (this.GetComponent<main>().getTile(x + 1, y + 1) != 0 && unit[x + 1, y + 1, 0] == 0 && this.GetComponent<main>().getAddon(x + 1, y + 1) == 0 && this.GetComponent<structures>().getStructure(x + 1, y + 1) == 0)
        {
            moves[5] = Instantiate(move, new Vector3(((x + 1) * 20), 5, ((y + 1) * 20)), Quaternion.identity);
            moves[5].transform.Rotate(90, 0, 0);
            moves[5].GetComponent<button>().setCoord(x + 1, y + 1);
        }

        if (this.GetComponent<enemyAction>().getUnit(x + 1, y - 1) != 0)
        {
            moves[6] = Instantiate(attack, new Vector3(((x + 1) * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[6].transform.Rotate(90, 0, 0);
            moves[6].GetComponent<button>().setCoord(x + 1, y - 1);
        }
        else if (this.GetComponent<enemyAction>().getStructure(x + 1, y - 1) != 0)
        {
            moves[6] = Instantiate(attackS, new Vector3(((x + 1) * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[6].transform.Rotate(90, 0, 0);
            moves[6].GetComponent<button>().setCoord(x + 1, y - 1);
        }
        else if (this.GetComponent<main>().getTile(x + 1, y - 1) != 0 && unit[x + 1, y - 1, 0] == 0 && this.GetComponent<main>().getAddon(x + 1, y - 1) == 0 && this.GetComponent<structures>().getStructure(x + 1, y - 1) == 0)
        {
            moves[6] = Instantiate(move, new Vector3(((x + 1) * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[6].transform.Rotate(90, 0, 0);
            moves[6].GetComponent<button>().setCoord(x + 1, y - 1);
        }

        if (this.GetComponent<enemyAction>().getUnit(x - 1, y - 1) != 0)
        {
            moves[7] = Instantiate(attack, new Vector3(((x - 1) * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[7].transform.Rotate(90, 0, 0);
            moves[7].GetComponent<button>().setCoord(x - 1, y - 1);
        }
        else if (this.GetComponent<enemyAction>().getStructure(x - 1, y - 1) != 0)
        {
            moves[7] = Instantiate(attackS, new Vector3(((x - 1) * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[7].transform.Rotate(90, 0, 0);
            moves[7].GetComponent<button>().setCoord(x - 1, y - 1);
        }
        else if (this.GetComponent<main>().getTile(x - 1, y - 1) != 0 && unit[x - 1, y - 1, 0] == 0 && this.GetComponent<main>().getAddon(x - 1, y - 1) == 0 && this.GetComponent<structures>().getStructure(x - 1, y - 1) == 0)
        {
            moves[7] = Instantiate(move, new Vector3(((x - 1) * 20), 5, ((y - 1) * 20)), Quaternion.identity);
            moves[7].transform.Rotate(90, 0, 0);
            moves[7].GetComponent<button>().setCoord(x - 1, y - 1);
        }

        if (builder)
        {
            GameObject button = Instantiate(build, new Vector3((x * 20), 10, (y * 20)), Quaternion.identity);
            button.transform.Rotate(90, 0, 0);
        }
    }

    public void hideMoves()
    {
        GameObject[] deleteList = GameObject.FindGameObjectsWithTag("Move");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }

        deleteList = GameObject.FindGameObjectsWithTag("Build");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }
        
        deleteList = GameObject.FindGameObjectsWithTag("Attack");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }

        deleteList = GameObject.FindGameObjectsWithTag("CityAttack");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }
    }

    public void moveUnit(int oldX, int oldY, int newX, int newY)
    {
        int[] copyArray = new int[IDLength];
        for (int i = 0; i < IDLength; i++)
        {
            copyArray[i] = unit[oldX, oldY, i];
        }

        for (int i = 0; i < IDLength; i++)
        {
            unit[newX, newY, i] = copyArray[i];
        }

        clearTile(oldX, oldY);
    }

    public int getUnit(int x, int y)
    {
        return unit[x, y, 0];
    }

    public void resetTurns()
    {
        GameObject[] unitList = GameObject.FindGameObjectsWithTag("Unit");

        foreach (GameObject index in unitList)
        {
            int x = index.GetComponent<unitController>().getX();
            int y = index.GetComponent<unitController>().getY();

            int currentTurn = unit[x, y, 3];

            for (int i = 0; i < currentTurn; i++)
            {
                unit[x, y, 3] -= 1;
            }

            refreshUnits();
        }

        unitList = GameObject.FindGameObjectsWithTag("Builder");

        foreach (GameObject index in unitList)
        {
            int x = index.GetComponent<unitController>().getX();
            int y = index.GetComponent<unitController>().getY();

            int currentTurn = unit[x, y, 3];

            for (int i = 0; i < currentTurn; i++)
            {
                unit[x, y, 3] -= 1;
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

    public void attackUnit(int x, int y, int unitX, int unitY)
    {
        this.GetComponent<enemyAction>().unit[x, y, 1] -= unit[unitX, unitY, 2];
        this.GetComponent<enemyAction>().checkLife();
        this.GetComponent<enemyAction>().refreshUnits();
    }

    public void attackStructure(int x, int y, int unitX, int unitY)
    {
        this.GetComponent<enemyAction>().structure[x, y, 1] -= unit[unitX, unitY, 2];
        this.GetComponent<enemyAction>().checkLife();
        this.GetComponent<enemyAction>().refreshStructures();
    }

    public void checkLife()
    {
        for (int x = 0; x < width; x ++)
        {
            for (int y = 0; y < height; y++)
            {
                if (unit[x, y, 0] != 0)
                {
                    if (unit[x, y, 1] <= 0)
                    {
                        clearTile(x, y);
                    }
                }
            }
        }
    }

    public void clearTile(int x, int y)
    {
        for (int i = 0; i < IDLength; i++)
        {
            unit[x, y, i] = 0;
        }
    }
}
