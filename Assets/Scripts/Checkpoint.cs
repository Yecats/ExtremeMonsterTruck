using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public bool IsNext;
    public bool isFinish;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && IsNext) 
        {
            //Setup for the next checkpoint
            this.IsNext = false;

            if (!this.isFinish)
            {
                GameManager.Instance.CreateNewCheckpoint_UI();
                this.transform.parent.GetChild(this.transform.GetSiblingIndex() + 1).GetComponent<Checkpoint>().IsNext = true; 
            }
            else
            {
                GameManager.Instance.EndGame();
            }
        }
    }
}
