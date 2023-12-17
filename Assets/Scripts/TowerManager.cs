using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerManager : MonoBehaviour
{
    public List<Tower> towers;

    // Metodo per ottenere la lista di proiettili associati a una torre specifica
    public List<GameObject> GetProjectilesForTower(int towerIndex)
    {
        Tower tower = towers.Find(t => t.towerIndex == towerIndex);
        return tower != null ? tower.bulletList : null;
    }
}

