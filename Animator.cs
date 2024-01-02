using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator : MonoBehaviour
{
    public int speed = 60;

    private int frameCounter;
    private int spriteCount;

    private int spriteIndex;

    public Sprite[] sprite = new Sprite[0];
    void Start()
    {
        spriteIndex = 0;
        frameCounter = 0;
        spriteCount = sprite.Length;
    }
    void Update()
    {
        if ((frameCounter * spriteCount) % speed == 0)
        {
            this.GetComponent<SpriteRenderer>().sprite = sprite[spriteIndex];

            spriteIndex++;

            if (spriteIndex >= spriteCount)
            {
                spriteIndex = 0;
            }
        }

        if (frameCounter >= speed)
        {
            frameCounter = 0;
        }

        frameCounter++;
    }
}
