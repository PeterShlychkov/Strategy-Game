using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class structureController : MonoBehaviour
{
    public bool enemy = false;

    public GameObject units;
    public bool show;

    private int training = 0;
    private int trainingMax = 1;

    private int myID;

    public int health;
    public int attack;

    private int x;
    private int y;
    void Start()
    {

    }
    void Update()
    {
        resetStats();
    }

    public bool getShow()
    {
        return show;
    }

    public void setShow(bool set)
    {
        show = set;
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

    public void getmyID(int ID)
    {
        myID = ID;
    }

    public void resetStats()
    {
        setStats(((myID % 1000) / 10000), ((myID % 100) / 100));
    }
    
    public void setCurrentTraining(int i)
    {
        training = i;
    }
    public int getTotalTraining()
    {
        return trainingMax;
    }

    public int getCurrentTraining()
    {
        return training;
    }

    public void addTraining()
    {
        training++;
    }

    public void removeTraining()
    {
 
        training = 0;
    }
}
