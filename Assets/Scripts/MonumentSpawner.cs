using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MonumentSpawner : NetworkBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject monumentPrefab;
    List<Monument> monuments = new List<Monument>();
    int currentId = 0;

    public void SpawnMonument(string name)
    {
        if (!isServer) return;

        if (monuments.Count >= currentId + 1)
        {
            monuments[currentId].SetPlayerName(name);
        }
        else
        {
            GameObject m_monument = Instantiate(monumentPrefab, spawnPoints[currentId].transform.position, spawnPoints[currentId].transform.rotation);
            if (m_monument != null)
            {
                m_monument.GetComponent<Monument>().SetPlayerName(name);
                NetworkServer.Spawn(m_monument);
            }
        }
        currentId++;
        currentId %= spawnPoints.Length;
    }
}
