using System.Collections;
using UnityEngine;

public class ReturnToPoolAfterSec : MonoBehaviour
{
    public PooledObject pooledObject;
    public float hideDuration;

    private void OnEnable()
    {
        HideObject();
    }

    public void HideObject()
    {
        StartCoroutine(ReturnToPool());
    }

    private IEnumerator ReturnToPool()
    {
        yield return ExtensionClass.GetWaitForSeconds(hideDuration);
        pooledObject.ReturnToPool();
        StopCoroutine(ReturnToPool());
    }
}