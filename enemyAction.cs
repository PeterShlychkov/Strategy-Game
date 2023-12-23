using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAction : MonoBehaviour
{
    public GameObject enemyUnit;
    public GameObject enemyBuilder;
    public GameObject enemyCity;

    public GameObject[] moves;

    public int width = 20;
    public int height = 20;
    public int unitIDLength = 5;
    public int structureIDLength = 5;

    public int[,,] unit; //Type, Health, Attack, Current Turn, Total Turns.
    public int[,,] structure; //Type, Health, Attack, Current Training, Total Training.

    private int spawnX;
    private int spawnY;

    public void enemyStart()
    {
        moves = new GameObject[4];

        height = this.GetComponent<main>().getHeight();
        width = this.GetComponent<main>().getWidth();

        unit = new int[width, height, unitIDLength];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                unit[x, y, 0] = 0;
            }
        }

        structure = new int[width, height, structureIDLength];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                structure[x, y, 0] = 0;
            }
        }

        spawnX = Random.Range(0, width);
        spawnY = Random.Range(0, height);
        while (this.GetComponent<main>().getTile(spawnX, spawnY) == 0 || this.GetComponent<main>().getAddon(spawnX, spawnY) != 0)
        {
            spawnX = Random.Range(0, width);
            spawnY = Random.Range(0, height);
        }
        int[] spawnID = { 1, 8, 0, 0, 1 };
        spawnStructure(spawnX, spawnY, spawnID);
        refreshStructures();
        Debug.Log("Enemy Structure at " + spawnX + ", " + spawnY);
    }

    public void checkLife()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (structure[x, y, 0] != 0)
                {
                    if (structure[x, y, 1] <= 0)
                    {
                        clearStructureTile(x, y);
                    }
                }

                if (unit[x, y, 0] != 0)
                {
                    if (unit[x, y, 1] <= 0)
                    {
                        clearUnitTile(x, y);
                    }
                }
            }
        }
    }

    public void clearUnitTile(int x, int y)
    {
        for (int i = 0; i < unitIDLength; i++)
        {
            unit[x, y, i] = 0;
        }
    }

    public void clearStructureTile(int x, int y)
    {
        for (int i = 0; i < structureIDLength; i++)
        {
            structure[x, y, i] = 0;
        }
    }

    public float distance(int x1, int y1, int x2, int y2)
    {
        int a = Mathf.Abs(x1 - x2);
        int b = Mathf.Abs(y1 - y2);
        float c = Mathf.Sqrt((b * b) + (a * a));

        return c;
    }

    public void action()
    {
        GameObject[] unitList = GameObject.FindGameObjectsWithTag("EnemyUnit");

        GameObject[] playerUnitList = GameObject.FindGameObjectsWithTag("Unit");

        GameObject[] cityList = GameObject.FindGameObjectsWithTag("EnemyCity");

        GameObject[] playerCityList = GameObject.FindGameObjectsWithTag("City");

        GameObject[] builderList = GameObject.FindGameObjectsWithTag("EnemyBuilder");

        GameObject[] playerBuilderList = GameObject.FindGameObjectsWithTag("Builder");

        int closestPlayer = width + height;
        int[] closestCity = new int[2];
        foreach (GameObject city in cityList)
        {
            int cityX = city.GetComponent<structureController>().getX();
            int cityY = city.GetComponent<structureController>().getY();
            foreach (GameObject pCity in playerCityList)
            {
                int playerCityX = pCity.GetComponent<structureController>().getX();
                int playerCityY = pCity.GetComponent<structureController>().getY();
                if (distance(cityX, cityY, playerCityX, playerCityY) < closestPlayer)
                {
                    closestPlayer = (int)distance(cityX, cityY, playerCityX, playerCityY);
                    closestCity[0] = playerCityX;
                    closestCity[1] = playerCityY;
                }
            }

            foreach (GameObject unit in playerUnitList)
            {
                int playerUnitX = unit.GetComponent<unitController>().getX();
                int playerUnitY = unit.GetComponent<unitController>().getY();
                if (distance(cityX, cityY, playerUnitX, playerUnitY) < closestPlayer)
                {
                    closestPlayer = (int)distance(cityX, cityY, playerUnitX, playerUnitY);
                }
            }
        }

        int passive = 0;
        int aggresive = 0;
        bool makeUnit;
        if (closestPlayer > (width / 3))
        {
            passive++;
            makeUnit = false;
        }
        else
        {
            aggresive++;
            makeUnit = true;
        }

        foreach (GameObject unit in unitList)
        {
            int unitX = unit.GetComponent<unitController>().getX();
            int unitY = unit.GetComponent<unitController>().getY();
            if (attack(unitX, unitY) == 0)
            {
                if (passive < aggresive)
                {
                    moveTowards(unitX, unitY, closestCity[0], closestCity[1]);
                }
                else
                {
                    wander(unitX, unitY);
                }
            }
        }

        foreach (GameObject builder in builderList)
        {
            int builderX = builder.GetComponent<unitController>().getX();
            int builderY = builder.GetComponent<unitController>().getY();
            if (distance(closestCity[0], closestCity[1], builderX, builderY) <= 7)
            {
                clearUnitTile(builderX, builderY);
                int[] spawnID = { 1, 8, 0, 0, 1 };
                spawnStructure(builderX, builderY, spawnID);
                refreshStructures();
            }
            if (passive > aggresive)
            {
                moveTowards(builderX, builderY, closestCity[0], closestCity[1]);
            }
            else
            {
                wander(builderX, builderY);
            }
        }

        foreach (GameObject city in cityList)
        {
            int buildingX = city.GetComponent<structureController>().getX();
            int buildingY = city.GetComponent<structureController>().getY();
            if (makeUnit && structure[buildingX, buildingY, 3] < structure[buildingX, buildingY, 4])
            {
                Debug.Log("Attempting to Spawn Unit");
                int[] unitID = { 1, 6, 2, 0, 1 }; // ID of unit to be spawned.
                int turns = 3; // Number of turns to spawn unit.
                int[,] list = new int[3, unitIDLength]; // (City X. City Y.), (Current Turn. Total Turns.), Unit ID.

                list[0, 0] = buildingX;
                list[0, 1] = buildingY;
                list[1, 0] = 0;
                list[1, 1] = turns;
                for (int i = 0; i < unitIDLength; i++)
                {
                    list[2, i] = unitID[i];
                }

                this.GetComponent<turnController>().enemyActions.Add(list);
                structure[buildingX, buildingY, 3]++;
            }
            else if (structure[buildingX, buildingY, 3] < structure[buildingX, buildingY, 4])
            {
                Debug.Log("Attempting to Spawn Builder");
                int[] unitID = { 2, 2, 1, 0, 1 }; // ID of unit to be spawned.
                int turns = 6; // Number of turns to spawn unit.
                int[,] list = new int[3, unitIDLength]; // (City X. City Y.), (Current Turn. Total Turns.), Unit ID.

                list[0, 0] = buildingX;
                list[0, 1] = buildingY;
                list[1, 0] = 0;
                list[1, 1] = turns;
                for (int i = 0; i < unitIDLength; i++)
                {
                    list[2, i] = unitID[i];
                }

                this.GetComponent<turnController>().enemyActions.Add(list);
                structure[buildingX, buildingY, 3]++;
            }
        }

        refreshStructures();
        refreshUnits();
    }
    
    public int attack(int x, int y)
    {
        int canAttack = 1;
        if ((this.GetComponent<units>().unit[x, y + 1, 0] == 0 && this.GetComponent<structures>().structure[x, y + 1, 0] == 0) &&
               (this.GetComponent<units>().unit[x + 1, y, 0] == 0 && this.GetComponent<structures>().structure[x + 1, y, 0] == 0) &&
               (this.GetComponent<units>().unit[x, y - 1, 0] == 0 && this.GetComponent<structures>().structure[x, y - 1, 0] == 0) &&
               (this.GetComponent<units>().unit[x - 1, y, 0] == 0 && this.GetComponent<structures>().structure[x - 1, y, 0] == 0) &&
               (this.GetComponent<units>().unit[x - 1, y + 1, 0] == 0 && this.GetComponent<structures>().structure[x - 1, y + 1, 0] == 0) &&
               (this.GetComponent<units>().unit[x + 1, y + 1, 0] == 0 && this.GetComponent<structures>().structure[x + 1, y + 1, 0] == 0) &&
               (this.GetComponent<units>().unit[x - 1, y - 1, 0] == 0 && this.GetComponent<structures>().structure[x - 1, y - 1, 0] == 0) &&
               (this.GetComponent<units>().unit[x + 1, y - 1, 0] == 0 && this.GetComponent<structures>().structure[x + 1, y - 1, 0] == 0))
        {
            canAttack = 0;
        }
        else
        {
            int[] output = new int[2];
            bool cordGood = false;

            while (!cordGood)
            {
                int rand = Random.Range(1, 9);

                if (rand == 1 && (this.GetComponent<units>().unit[x, y + 1, 0] != 0 || this.GetComponent<structures>().structure[x, y + 1, 0] != 0))
                {
                    output[0] = x;
                    output[1] = y + 1;
                    cordGood = true;
                }

                if (rand == 2 && (this.GetComponent<units>().unit[x + 1, y, 0] != 0 || this.GetComponent<structures>().structure[x + 1, y, 0] != 0))
                {

                    output[0] = x + 1;
                    output[1] = y;
                    cordGood = true;
                }

                if (rand == 3 && (this.GetComponent<units>().unit[x, y - 1, 0] != 0 || this.GetComponent<structures>().structure[x, y - 1, 0] != 0))
                {
                    output[0] = x;
                    output[1] = y - 1;
                    cordGood = true;
                }


                if (rand == 4 && (this.GetComponent<units>().unit[x - 1, y, 0] != 0 || this.GetComponent<structures>().structure[x - 1, y, 0] != 0))
                {
                    output[0] = x - 1;
                    output[1] = y;
                    cordGood = true;
                }

                if (rand == 5 && (this.GetComponent<units>().unit[x - 1, y + 1, 0] != 0 || this.GetComponent<structures>().structure[x - 1, y + 1, 0] != 0))
                {
                    output[0] = x - 1;
                    output[1] = y + 1;
                    cordGood = true;
                }

                if (rand == 6 && (this.GetComponent<units>().unit[x + 1, y + 1, 0] != 0 || this.GetComponent<structures>().structure[x + 1, y + 1, 0] != 0))
                {

                    output[0] = x + 1;
                    output[1] = y + 1;
                    cordGood = true;
                }

                if (rand == 7 && (this.GetComponent<units>().unit[x - 1, y - 1, 0] != 0 || this.GetComponent<structures>().structure[x - 1, y - 1, 0] != 0))
                {
                    output[0] = x - 1;
                    output[1] = y - 1;
                    cordGood = true;
                }


                if (rand == 8 && (this.GetComponent<units>().unit[x + 1, y - 1, 0] != 0 || this.GetComponent<structures>().structure[x + 1, y - 1, 0] != 0))
                {
                    output[0] = x + 1;
                    output[1] = y - 1;
                    cordGood = true;
                }
            }

            if (this.GetComponent<units>().unit[output[0], output[1], 0] != 0)
            {
                attackUnit(output[0], output[1], x, y);
            }
            else if (this.GetComponent<structures>().structure[output[0], output[1], 0] != 0)
            {
                attackStructure(output[0], output[1], x, y);
            }
        }
        return canAttack;
    }
    public void moveTowards(int x, int y, int destinationX, int destinationY)
    {
        if (destinationX == x && destinationY > y)
        {
            int errorCounter = 0;
            bool cordGood = false;
            while (!cordGood)
            {
                int xAdder = 0;
                int yAdder = Random.Range(0, 2);

                if (this.GetComponent<structures>().isTileValid(x + xAdder, y + yAdder))
                {
                    cordGood = true;
                    moveUnit(x, y, (x + xAdder), (y + yAdder));
                }
                else
                {
                    errorCounter++;
                }

                if (errorCounter >= 100)
                {
                    wander(x, y);
                    break;
                }
            }
        }

        if (destinationX == x && destinationY < y)
        {
            int errorCounter = 0;
            bool cordGood = false;
            while (!cordGood)
            {
                int xAdder = 0;
                int yAdder = Random.Range(0, -2);

                if (this.GetComponent<structures>().isTileValid(x + xAdder, y + yAdder))
                {
                    cordGood = true;
                    moveUnit(x, y, (x + xAdder), (y + yAdder));
                }
                else
                {
                    errorCounter++;
                }

                if (errorCounter >= 100)
                {
                    wander(x, y);
                    break;
                }
            }
        }

        if (destinationX > x && destinationY == y)
        {
            int errorCounter = 0;
            bool cordGood = false;
            while (!cordGood)
            {
                int xAdder = Random.Range(0, 2);
                int yAdder = 0;

                if (this.GetComponent<structures>().isTileValid(x + xAdder, y + yAdder))
                {
                    cordGood = true;
                    moveUnit(x, y, (x + xAdder), (y + yAdder));
                }
                else
                {
                    errorCounter++;
                }

                if (errorCounter >= 100)
                {
                    wander(x, y);
                    break;
                }
            }
        }

        if (destinationX < x && destinationY == y)
        {
            int errorCounter = 0;
            bool cordGood = false;
            while (!cordGood)
            {
                int xAdder = Random.Range(0, -2);
                int yAdder = 0;

                if (this.GetComponent<structures>().isTileValid(x + xAdder, y + yAdder))
                {
                    cordGood = true;
                    moveUnit(x, y, (x + xAdder), (y + yAdder));
                }
                else
                {
                    errorCounter++;
                }

                if (errorCounter >= 100)
                {
                    wander(x, y);
                    break;
                }
            }
        }

        if (destinationX < x && destinationY > y)
        {
            int errorCounter = 0;
            bool cordGood = false;
            while (!cordGood)
            {
                int xAdder = Random.Range(-1, 1);
                int yAdder = Random.Range(0, 2);

                if (this.GetComponent<structures>().isTileValid(x + xAdder, y + yAdder))
                {
                    cordGood = true;
                    moveUnit(x, y, (x + xAdder), (y + yAdder));
                }
                else
                {
                    errorCounter++;
                }

                if (errorCounter >= 100)
                {
                    wander(x, y);
                    break;
                }
            }
        }

        if (destinationX > x && destinationY > y)
        { 
            int errorCounter = 0;
            bool cordGood = false;
            while (!cordGood)
            {
                int xAdder = Random.Range(0, 2);
                int yAdder = Random.Range(0, 2);

                if (this.GetComponent<structures>().isTileValid(x + xAdder, y + yAdder))
                {
                    cordGood = true;
                    moveUnit(x, y, (x + xAdder), (y + yAdder));
                }
                else
                {
                    errorCounter++;
                }

                if (errorCounter >= 100)
                {
                    wander(x, y);
                    break;
                }
            }
        }

        if (destinationX > x && destinationY < y)
        {
            int errorCounter = 0;
            bool cordGood = false;
            while (!cordGood)
            {
                int xAdder = Random.Range(0, 2);
                int yAdder = Random.Range(-1, 1);

                if (this.GetComponent<structures>().isTileValid(x + xAdder, y + yAdder))
                {
                    cordGood = true;
                    moveUnit(x, y, (x + xAdder), (y + yAdder));
                }
                else
                {
                    errorCounter++;
                }

                if (errorCounter >= 100)
                {
                    wander(x, y);
                    break;
                }
            }
        }

        if (destinationX < x && destinationY < y)
        {
            int errorCounter = 0;
            bool cordGood = false;
            while (!cordGood)
            {
                int xAdder = Random.Range(-1, 1);
                int yAdder = Random.Range(-1, 1);

                if (this.GetComponent<structures>().isTileValid(x + xAdder, y + yAdder))
                {
                    cordGood = true;
                    moveUnit(x, y, (x + xAdder), (y + yAdder));
                }
                else
                {
                    errorCounter++;
                }

                if (errorCounter >= 100)
                {
                    wander(x, y);
                    break;
                }
            }
        }
    }

    public void wander(int x, int y)
    {
        if (!(this.GetComponent<structures>().isTileValid(x, y + 1)) &&
               !(this.GetComponent<structures>().isTileValid(x + 1, y)) &&
               !(this.GetComponent<structures>().isTileValid(x, y - 1)) &&
               !(this.GetComponent<structures>().isTileValid(x - 1, y)) &&
               !(this.GetComponent<structures>().isTileValid(x - 1, y + 1)) &&
               !(this.GetComponent<structures>().isTileValid(x + 1, y + 1)) &&
               !(this.GetComponent<structures>().isTileValid(x - 1, y - 1)) &&
               !(this.GetComponent<structures>().isTileValid(x + 1, y - 1)))
        {
            Debug.Log("Unable to wander enemy unit.");
        }
        else
        {
            bool cordGood = false;
            while (!cordGood)
            {
                int xAdder = Random.Range(-1, 2);
                int yAdder = Random.Range(-1, 2);

                if (this.GetComponent<structures>().isTileValid(x + xAdder, y + yAdder))
                {
                    cordGood = true;
                    moveUnit(x, y, (x + xAdder), (y + yAdder));
                }
            }
        }
    }

    public void moveUnit(int oldX, int oldY, int newX, int newY)
    {
        int[] copyArray = new int[unitIDLength];
        for (int i = 0; i < unitIDLength; i++)
        {
            copyArray[i] = unit[oldX, oldY, i];
        }

        for (int i = 0; i < unitIDLength; i++)
        {
            unit[newX, newY, i] = copyArray[i];
        }

        clearUnitTile(oldX, oldY);
    }

    public int getUnit(int x, int y)
    {
        return unit[x, y, 0];
    }

    public void resetTurns()
    {
        GameObject[] unitList = GameObject.FindGameObjectsWithTag("EnemyUnit");

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

        unitList = GameObject.FindGameObjectsWithTag("EnemyBuilder");

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

    public void attackUnit(int x, int y, int unitX, int unitY)
    {
        this.GetComponent<units>().unit[x, y, 1] -= unit[unitX, unitY, 2];
        this.GetComponent<units>().checkLife();
        this.GetComponent<units>().refreshUnits();
    }

    public void attackStructure(int x, int y, int unitX, int unitY)
    {
        this.GetComponent<structures>().structure[x, y, 1] -= unit[unitX, unitY, 2];
        this.GetComponent<structures>().checkLife();
        this.GetComponent<structures>().refreshStructures();
    }
    public void spawnUnit(int x, int y, int[] setID)
    {
        for (int i = 0; i < unitIDLength; i++)
        {
            Debug.Log(x + ", " + y);
            unit[x, y, i] = setID[i];
        }
    }

    public void refreshUnits()
    {
        GameObject[] deleteList = GameObject.FindGameObjectsWithTag("EnemyUnit");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }

        deleteList = GameObject.FindGameObjectsWithTag("EnemyBuilder");
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
                    GameObject spawn = Instantiate(enemyUnit, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    spawn.GetComponent<unitController>().resetStats();
                    spawn.GetComponent<unitController>().setCoord(x, y);
                }

                if (unit[x, y, 0] == 2)
                {
                    GameObject spawn = Instantiate(enemyBuilder, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    spawn.GetComponent<unitController>().resetStats();
                    spawn.GetComponent<unitController>().setCoord(x, y);
                }
            }
        }
        this.GetComponent<main>().checkFog();
        this.GetComponent<main>().refreshFog();
    }

    public void spawnStructure(int x, int y, int[] setID)
    {
        for (int i = 0; i < structureIDLength; i++)
        {
            structure[x, y, i] = setID[i];
        }
    }

    public void refreshStructures()
    {
        GameObject[] deleteList = GameObject.FindGameObjectsWithTag("EnemyCity");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (structure[x, y, 0] == 1)
                {
                    GameObject spawn = Instantiate(enemyCity, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    spawn.GetComponent<structureController>().setCoord(x, y);
                }
            }
        }
    }
    public int getStructure(int x, int y)
    {
        return structure[x, y, 0];
    }
}
