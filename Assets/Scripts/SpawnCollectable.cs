using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollectable : MonoBehaviour
{
    [SerializeField] Collectable coinPrefab;
    [SerializeField] Collectable heartPrefab;

    public void Spawn()
    {
        for (int i = 0; i < 8; i++)
        {
            //Collectable item = Instantiate(Random.value < 0.1f ? heartPrefab : coinPrefab, transform.position, Quaternion.identity);
            Collectable item = (Random.value < 0.1f ? heartPrefab : coinPrefab).GetPooledInstance<Collectable>(transform.position);
            item.Spawn();
        }
    }
}
