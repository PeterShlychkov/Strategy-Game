using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    bool playerTurn = true;

    public ArrayList actions = new ArrayList();
    public ArrayList enemyActions = new ArrayList();

    bool t = true;

    public GameObject next;
    public GameObject defeat;
    public GameObject win;

    private void Start()
    {
        this.GetComponent<main>().mainStart();
        this.GetComponent<units>().unitStart();
        this.GetComponent<structures>().structureStart();
        this.GetComponent<enemyAction>().enemyStart();
    }
    void Update()
    {
        // Detect if "Next Button" is pressed.
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Next")
                {
                    playerTurn = false;
                }
            }
        }

        // This code runs every frame if it is players turn.
        if (playerTurn)
        {
            if (t) // This code runs only once per turn.
            {
                t = false;

                foreach (int[,] list in enemyActions)
                {
                    list[1, 0]++;
                }

                showNext();

                this.GetComponent<structures>().refreshStructures();
                this.GetComponent<main>().checkFog();
                this.GetComponent<main>().refreshFog();
            }

            playerAction();

            for (int x = 0; x < actions.Count; x++)
            {

            }

            // (City X. City Y.), (Current Turn. Total Turns.), Unit ID.
            for (int i = 0; i < actions.Count; i++)
            {
                int[,] list = (int[,])actions[i];

                if (list[1, 0] == list[1, 1])
                {
                    int[] unitID = new int[this.GetComponent<structures>().IDLength];
                    for (int z = 0; z < this.GetComponent<structures>().IDLength; z++)
                    {
                        unitID[z] = list[2, z];
                    }
                    int[] coordinates = this.GetComponent<structures>().getSpawnCord(list[0, 0], list[0, 1]);
                    int x = coordinates[0];
                    int y = coordinates[1];

                    if (x != -1)
                    {
                        this.GetComponent<units>().spawnUnit(x, y, unitID);
                    }
                    this.GetComponent<units>().refreshUnits();
                    this.GetComponent<structures>().structure[list[0, 0], list[0, 1], 3] = 0;
                    actions.RemoveAt(i);
                }
            }

        }

        // This code runs every fram if it is enemys turn.
        else
        {
            if (!t) // This code runs only once per turn.
            {
                t = true;

                foreach (int[,] list in actions)
                {
                    list[1, 0]++;
                }

                this.GetComponent<units>().resetTurns();
                hideNext();

                this.GetComponent<enemyAction>().action();
            }


            // (City X. City Y.), (Current Turn. Total Turns.), Unit ID.
            for (int i = 0; i < enemyActions.Count; i++)
            {
                int[,] list = (int[,])enemyActions[i];

                if (list[1, 0] == list[1, 1])
                {
                    int[] unitID = new int[this.GetComponent<enemyAction>().unitIDLength];
                    for (int z = 0; z < this.GetComponent<enemyAction>().unitIDLength; z++)
                    {
                        unitID[z] = list[2, z];
                    }
                    int[] coordinates = this.GetComponent<structures>().getSpawnCord(list[0, 0], list[0, 1]);
                    int x = coordinates[0];
                    int y = coordinates[1];

                    if (x != -1)
                    {
                        this.GetComponent<enemyAction>().spawnUnit(x, y, unitID);
                    }
                    this.GetComponent<enemyAction>().refreshUnits();
                    this.GetComponent<enemyAction>().structure[list[0, 0], list[0, 1], 3] = 0;
                    enemyActions.RemoveAt(i);
                }
            }

            playerTurn = true;
        }

        // Check to see if game is over.
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("City");
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("EnemyCity");
        if (playerList.Length == 0)
        {
            defeat.GetComponent<Renderer>().enabled = true;
        }
        if (enemyList.Length == 0)
        {
            win.GetComponent<Renderer>().enabled = true;
        }
    }

    public void playerAction()
    {
        this.GetComponent<structures>().structureUpdate();
        this.GetComponent<units>().unitUpdate();
    }
    public void showNext()
    {
        next.GetComponent<Renderer>().enabled = true;
        next.GetComponent<Collider>().enabled = true;
    }
    public void hideNext()
    {
        next.GetComponent<Renderer>().enabled = false;
        next.GetComponent<Collider>().enabled = false;
    }
}
