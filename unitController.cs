using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
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
