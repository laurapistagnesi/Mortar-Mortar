using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerManager : MonoBehaviour
{
    public List<Tower> towers;

  
    public List<GameObject> GetProjectilesForTower(int towerIndex)
    {
        Tower tower = towers.Find(t => t.towerIndex == towerIndex);

        if (tower != null)
        {
            // Riduci localmente le dimensioni di tutti i proiettili associati alla torre
            foreach (GameObject projectile in tower.bulletList)
            {
                if (projectile != null)
                {
                    projectile.transform.localScale *= 0.2f;
                }
            }

            return tower.bulletList;
        }

        return null;
    }
}
