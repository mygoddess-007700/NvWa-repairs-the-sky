using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public int num;
    public Stone [] stones;
    public int stonesLen;
    public float absorptionDistance;

    private Stone selectedStone;

    void Start()
    {
        stonesLen = stones.Length;  
    }

    void LateUpdate()
    {
        if (GameController.instance.answerHole == num)
        {
            return;
        }
        for (int i = 0; i < stonesLen; i++)
        {   
            if (stones[i].unselectable)
            {
                continue;
            }
            if (((Vector2)stones[i].transform.position - (Vector2)transform.position).magnitude < absorptionDistance)
            {
                if (num == 0)
                {
                    stones[i].isInHole0 = true;
                }
                else if (num == 1)
                {
                    stones[i].isInHole1 = true;
                }
                
                stones[i].transform.position = (Vector2)transform.position;
                if (!stones[i].dragging)
                {
                    stones[i].unselectable = true;
                    selectedStone = stones[i];
                    GameController.instance.GetAnswer(num, stones[i].num);
                }
            }
            else
            {
                if (num == 0)
                {
                    stones[i].isInHole0 = false;
                }
                else if (num == 1)
                {
                    stones[i].isInHole1 = false;
                }
            }
        }
    }

    public void ReleaseStone()
    {
        if (selectedStone != null)
        {
            selectedStone.unselectable = true;
            StartCoroutine(selectedStone.ReadyReturn());
            selectedStone = null;
        }
    }
}
