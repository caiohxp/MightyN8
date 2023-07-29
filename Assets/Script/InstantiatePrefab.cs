using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePrefab : MonoBehaviour
{
    private GameObject enemyShotPrefab;
    // Start is called before the first frame update
    void Awake()
    {
        enemyShotPrefab = Resources.Load<GameObject>("EnemyShooter");
        // Instantiate(enemyShotPrefab, Vector3.zero, Quaternion.identity);
    }

}
