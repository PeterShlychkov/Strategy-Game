using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structures : MonoBehaviour
{
    public GameObject city;

    public GameObject build;
    public GameObject train;
    public GameObject research;
    public GameObject upgrade;

    public GameObject[] buttons;

    public int width = 20;
    public int height = 20;
    public int IDLength = 5;

    GameObject clickedBuilding;
    int buildingX;
    int buildingY;

    private int spawnX;
    private int spawnY;

    public int[,,] structure; //Type, Health, Attack, Current Training, Total Training.

    public void structureStart()
    {
        buttons = new GameObject[4];

        height = this.GetComponent<main>().getHeight();
        width = this.GetComponent<main>().getWidth();

        structure = new int[width, height, IDLength];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i = 0; i < IDLength; i++)
                {
                    structure[x, y, i] = 0;
                }
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
        Debug.Log("Player Structure at " + spawnX + ", " + spawnY);
    }
    public void structureUpdate()
    {
        checkLife();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "City" && clickedBuilding == null)
                {
                    clickedBuilding = hit.transform.gameObject;
                    buildingX = clickedBuilding.GetComponent<structureController>().getX();
                    buildingY = clickedBuilding.GetComponent<structureController>().getY();
                    showMenu(buildingX, buildingY);
                }

                else if (hit.transform.tag == "CityTrain")
                {
                    if (structure[buildingX, buildingY, 3] < structure[buildingX, buildingY, 4])
                    {
                        int[] unitID = { 1, 6, 2, 0, 1 }; // ID of unit to be spawned.
                        int turns = 3; // Number of turns to spawn unit.
                        int[,] list = new int[3, IDLength]; // (City X. City Y.), (Current Turn. Total Turns.), Unit ID.

                        list[0, 0] = buildingX;
                        list[0, 1] = buildingY;
                        list[1, 0] = 0;
                        list[1, 1] = turns;
                        for (int i = 0; i < IDLength; i++)
                        {
                            list[2, i] = unitID[i];
                        }

                        this.GetComponent<turnController>().actions.Add(list);
                        structure[buildingX, buildingY, 3]++;
                    }
                    else
                    {
                        Debug.Log("Something is already training in this city.");
                    }
                    hideMoves("CityTrain");
                    hideMoves("CityUp");
                    hideMoves("CityRes");
                    hideMoves("CityBuild");
                }

                else if (hit.transform.tag == "CityBuild")
                {
                    if (structure[buildingX, buildingY, 3] < structure[buildingX, buildingY, 4])
                    {
                        int[] unitID = { 2, 2, 1, 0, 1 }; // ID of unit to be spawned.
                        int turns = 6; // Number of turns to spawn unit.
                        int[,] list = new int[3, IDLength]; // (City X. City Y.), (Current Turn. Total Turns.), Unit ID.

                        list[0, 0] = buildingX;
                        list[0, 1] = buildingY;
                        list[1, 0] = 0;
                        list[1, 1] = turns;
                        for (int i = 0; i < IDLength; i++)
                        {
                            list[2, i] = unitID[i];
                        }

                        this.GetComponent<turnController>().actions.Add(list);
                        structure[buildingX, buildingY, 3]++;
                    }
                    else
                    {
                        Debug.Log("Something is already training in this city.");
                    }
                    hideMoves("CityTrain");
                    hideMoves("CityUp");
                    hideMoves("CityRes");
                    hideMoves("CityBuild");
                }

                else
                {
                    clickedBuilding = null;
                    hideMoves("CityTrain");
                    hideMoves("CityUp");
                    hideMoves("CityRes");
                    hideMoves("CityBuild");
                }
            }
        }
    }

    public bool isTileValid(int x, int y)
    {
        bool output = false;
        if (this.GetComponent<units>().unit[x, y, 0] == 0 && structure[x, y, 0] == 0 &&
            this.GetComponent<main>().tile[x, y] != 0 && this.GetComponent<main>().addon[x, y] == 0 &&
            this.GetComponent<enemyAction>().unit[x, y, 0] == 0 &&
            this.GetComponent<enemyAction>().structure[x, y, 0] == 0)
        {
            output = true;
        }

        return output;
    }

    public void spawnStructure(int x, int y, int[] setID)
    {
        for (int i = 0; i < IDLength; i++)
        {
            structure[x, y, i] = setID[i];
        }
    }

    public void refreshStructures()
    {
        GameObject[] deleteList = GameObject.FindGameObjectsWithTag("City");
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
                    GameObject spawn = Instantiate(city, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    spawn.GetComponent<structureController>().setCoord(x, y);
                }
            }
        }
    }

    public void showMenu(int x, int y)
    {
        buttons[0] = Instantiate(train, new Vector3((x * 20), 5, ((y + 1) * 20)), Quaternion.identity);
        buttons[0].transform.Rotate(90, 0, 0);
        buttons[0].GetComponent<button>().setCoord(x, y + 1);

        buttons[1] = Instantiate(build, new Vector3((x * 20), 5, ((y - 1) * 20)), Quaternion.identity);
        buttons[1].transform.Rotate(90, 0, 0);
        buttons[1].GetComponent<button>().setCoord(x + 1, y);
    }

    public void hideMoves(string tag)
    {
        GameObject[] deleteList = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }
    }

    public int[] getSpawnCord(int x, int y)
    {
        int[] output = new int[2];
        bool cordGood = false;

        while (!cordGood)
        {
            int rand = Random.Range(1, 5);

            if (!(isTileValid(x, y + 1)) &&
                !(isTileValid(x, y - 1)) &&
                !(isTileValid(x + 1, y)) &&
                !(isTileValid(x - 1, y)))
            {
                Debug.Log("No Coordinates Found.");
                output[0] = -1;
                break;
            }

            if (rand == 1 && isTileValid(x, y + 1))
            {
                output[0] = x;
                output[1] = y + 1;
                cordGood = true;
            } 

            if (rand == 2 && isTileValid(x, y - 1))
            {
                output[0] = x;
                output[1] = y - 1;
                cordGood = true;
            } 

            if (rand == 3 && isTileValid(x + 1, y))
            {
                output[0] = x + 1;
                output[1] = y;
                cordGood = true;
            }


            if (rand == 4 && isTileValid(x - 1, y))
            {
                output[0] = x - 1;
                output[1] = y;
                cordGood = true;
            }
        }
   
        return output;
    }

    public int getStructure(int x, int y)
    {
        return structure[x, y, 0];
    }

    public float distance(int x1, int y1, int x2, int y2)
    {
        int a = Mathf.Abs(x1 - x2);
        int b = Mathf.Abs(y1 - y2);
        float c = Mathf.Sqrt((b * b) + (a * a));

        return c;
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
            structure[x, y, i] = 0;
        }
    }
}
