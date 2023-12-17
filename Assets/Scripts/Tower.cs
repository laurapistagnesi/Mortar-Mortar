using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rappresenta una torre nel gioco
[System.Serializable]
public class Tower
{
    //Identificatore univoco della torre
    public int towerIndex;
    //Lista di proiettili associati alla torre
    public List<GameObject> bulletList;
}

