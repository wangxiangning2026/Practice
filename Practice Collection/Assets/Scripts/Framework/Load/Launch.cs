using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch : MonoBehaviour
{
    private IEnumerator Start()
    {
        Debug.Log("开始启动流程");
        yield return null;
    }

    private IEnumerator LoadMainScene()
    {
        yield return null;
    }
}
