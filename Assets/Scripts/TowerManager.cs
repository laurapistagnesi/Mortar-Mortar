using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gestore delle torri
[System.Serializable]
public class TowerManager : MonoBehaviour
{
    //Lista delle torri
    public List<Tower> towers;

    //Consente di ottenere la lista di proiettili associati a una torre specifica
    public List<GameObject> GetProjectilesForTower(int towerIndex)
    {
        Tower tower = towers.Find(t => t.towerIndex == towerIndex);
        return tower != null ? tower.bulletList : null;
    }
}

