using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main : MonoBehaviour
{
    public GameObject oceanTile;
    public GameObject desertTile;
    public GameObject meadowTile;
    public GameObject forestTile;
    public GameObject mountain;
    public GameObject iron;
    public GameObject fogObject;

    public int desertCount = 1;
    private int[] desertX;
    private int[] desertY;
    private int[] desertRadius;

    public int meadowCount = 1;
    private int[] meadowX;
    private int[] meadowY;
    private int[] meadowRadius;

    public int forestCount = 1;
    private int[] forestX;
    private int[] forestY;
    private int[] forestRadius;

    public int minMountainCount = 5;
    public int mountainCount = 1;
    private int[] mountainX;
    private int[] mountainY;

    public int minIronCount = 5;
    public int ironCount = 1;
    private int[] ironX;
    private int[] ironY;

    public int width = 20;
    public int height = 20;

    public int biomeRadius = 5;

    public int[,] fog; // Empty 0. Fog 1.
    public int[,] tile; //Water 0. Desert 1. Meadow 2. Forest 3.
    public int[,] addon; //Empty 0. Mountain 1. Iron 0.

    public void mainStart()
    {
        desertX = new int[desertCount];
        desertY = new int[desertCount];
        desertRadius = new int[desertCount];

        forestX = new int[forestCount];
        forestY = new int[forestCount];
        forestRadius = new int[forestCount];

        mountainX = new int[mountainCount];
        mountainY = new int[mountainCount];

        ironX = new int[ironCount];
        ironY = new int[ironCount];

        meadowX = new int[meadowCount];
        meadowY = new int[meadowCount];
        meadowRadius = new int[meadowCount];

        tile = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tile[x, y] = 0;
            }
        }

        fog = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                fog[x, y] = 1;
            }
        }

        addon = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                addon[x, y] = 0;
            }
        }

        generateDesert();

        generateMeadow();

        generateForest();

        border(1);

        generateMountain();

        generateIron();

        refreshTiles();

        checkFog();

        refreshFog();
    }
    public void mainUpdate()
    {

    }

    public void generateDesert()
    {
        for (int i = 0; i < desertCount; i++)
        {
            desertX[i] = Random.Range(0, width);
            desertY[i] = Random.Range(0, height);

            desertRadius[i] = Random.Range(0, biomeRadius) + 2;
        }

        for (int i = 0; i < desertCount; i++)
        {
            tile[desertX[i], desertY[i]] = 1;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i = 0; i < desertCount; i++)
                {
                    int a = Mathf.Abs(x - desertX[i]);
                    int b = Mathf.Abs(y - desertY[i]);
                    float c = Mathf.Sqrt((b * b) + (a * a));

                    if (c < desertRadius[i])
                    {
                        tile[x, y] = 1;
                    }
                }
            }
        }
    }

    public void generateForest()
    {
        for (int i = 0; i < forestCount; i++)
        {
            forestX[i] = Random.Range(0, width);
            forestY[i] = Random.Range(0, height);

            forestRadius[i] = Random.Range(0, biomeRadius) + 2;
        }

        for (int i = 0; i < forestCount; i++)
        {
            tile[forestX[i], forestY[i]] = 2;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i = 0; i < forestCount; i++)
                {
                    int a = Mathf.Abs(x - forestX[i]);
                    int b = Mathf.Abs(y - forestY[i]);
                    float c = Mathf.Sqrt((b * b) + (a * a));

                    if (c < forestRadius[i])
                    {
                        tile[x, y] = 2;
                    }
                }
            }
        }
    }

    public void generateMeadow()
    {
        for (int i = 0; i < meadowCount; i++)
        {
            meadowX[i] = Random.Range(0, width);
            meadowY[i] = Random.Range(0, height);

            meadowRadius[i] = Random.Range(0, biomeRadius) + 2;
        }

        for (int i = 0; i < meadowCount; i++)
        {
            tile[meadowX[i], meadowY[i]] = 3;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i = 0; i < meadowCount; i++)
                {
                    int a = Mathf.Abs(x - meadowX[i]);
                    int b = Mathf.Abs(y - meadowY[i]);
                    float c = Mathf.Sqrt((b * b) + (a * a));

                    if (c < meadowRadius[i])
                    {
                        tile[x, y] = 3;
                    }
                }
            }
        }
    }
    public void generateMountain()
    {
        int generatedNum = 0;
        while (generatedNum < minMountainCount)
        {
            for (int i = 0; i < ironCount; i++)
            {
                bool cordGood = false;
                int x = Random.Range(0, width);
                int y = Random.Range(0, height);
                while (!cordGood)
                {
                    x = Random.Range(0, width);
                    y = Random.Range(0, height);
                    
                    if (tile[x, y] != 0)
                    {
                        cordGood = true;
                    }
                }
                mountainX[i] = x;
                mountainY[i] = y;
            }

            for (int i = 0; i < mountainCount; i++)
            {
                if (tile[mountainX[i], mountainY[i]] != 0 && tile[mountainX[i], mountainY[i]] != 2)
                {
                    generatedNum++;
                    addon[mountainX[i], mountainY[i]] = 1;
                }
            }
        }
    }

    public void generateIron()
    {
        int generatedNum = 0;
        while (generatedNum < minIronCount)
        {
            for (int i = 0; i < ironCount; i++)
            {
                bool cordGood = false;
                int x = Random.Range(0, width);
                int y = Random.Range(0, height);
                while (!cordGood)
                {
                    x = Random.Range(0, width);
                    y = Random.Range(0, height);

                    if (tile[x, y] != 0)
                    {
                        cordGood = true;
                    }
                }
                ironX[i] = x;
                ironY[i] = y;
            }

            for (int i = 0; i < ironCount; i++)
            {
                if (tile[ironX[i], ironY[i]] != 0 && tile[ironX[i], ironY[i]] != 2)
                {
                    generatedNum++;
                    addon[ironX[i], ironY[i]] = 2;
                }
            }
        }
    }

    public void refreshTiles()
    {
        GameObject[] deleteList = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tile[x, y] == 0)
                {
                    GameObject tile = Instantiate(oceanTile, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    tile.transform.Rotate(90, 0, 0);
                }

                if (tile[x, y] == 1)
                {
                    GameObject tile = Instantiate(desertTile, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    tile.transform.Rotate(90, 0, 0);
                }

                if (tile[x, y] == 2)
                {
                    GameObject tile = Instantiate(forestTile, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    tile.transform.Rotate(90, 0, 0);
                }

                if (tile[x, y] == 3)
                {
                    GameObject tile = Instantiate(meadowTile, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                    tile.transform.Rotate(90, 0, 0);
                }

                if (addon[x, y] == 1)
                {
                    GameObject tile = Instantiate(mountain, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                }

                if (addon[x, y] == 2)
                {
                    GameObject tile = Instantiate(iron, new Vector3((x * 20), 0, (y * 20)), Quaternion.identity);
                }
            }
        }
    }

    public void refreshFog()
    {
        GameObject[] deleteList = GameObject.FindGameObjectsWithTag("Fog");
        foreach (GameObject index in deleteList)
        {
            Destroy(index);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (fog[x, y] == 1)
                {
                    GameObject tile = Instantiate(fogObject, new Vector3((x * 20), 10, (y * 20)), Quaternion.identity);
                    tile.transform.Rotate(90, 0, 0);
                }
            }
        }
    }

    public void checkFog()
    {
        GameObject[] checkList = GameObject.FindGameObjectsWithTag("Unit");

        for (int x = 0; x < width; x++)
        {
            for (int y  = 0; y < height; y++)
            {
                for (int i = 0; i < checkList.Length; i++)
                {
                    if (distance(x, y, checkList[i].GetComponent<unitController>().getX(), checkList[i].GetComponent<unitController>().getY()) <= 2)
                    {
                        fog[x, y] = 0;
                    }
                }
            }
        }

        checkList = GameObject.FindGameObjectsWithTag("City");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i = 0; i < checkList.Length; i++)
                {
                    if (distance(x, y, checkList[i].GetComponent<structureController>().getX(), checkList[i].GetComponent<structureController>().getY()) <= 5)
                    {
                        fog[x, y] = 0;
                    }
                }
            }
        }

        checkList = GameObject.FindGameObjectsWithTag("Builder");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i = 0; i < checkList.Length; i++)
                {
                    if (distance(x, y, checkList[i].GetComponent<unitController>().getX(), checkList[i].GetComponent<unitController>().getY()) <= 2)
                    {
                        fog[x, y] = 0;
                    }
                }
            }
        }
    }

    public int getHeight()
    {
        return height;
    }

    public int getWidth()
    {
        return width;
    }

    
    public int getTile(int x, int y)
    {
        return tile[x, y];
    }

    public int getAddon(int x, int y)
    {
        return addon[x, y];
    }

    public float distance(int x1, int y1, int x2, int y2)
    {
        int a = Mathf.Abs(x1 - x2);
        int b = Mathf.Abs(y1 - y2);
        float c = Mathf.Sqrt((b * b) + (a * a));

        return c;
    }

    public void border(int size)
    {
        for (int y = 0; y < height; y++)
        {
            tile[0, y] = 0;
            tile[height - 1, y] = 0;
        }

        for (int x = 0; x < width; x++)
        {
            tile[x, 0] = 0;
            tile[x, width - 1] = 0;
        }
    }
}
