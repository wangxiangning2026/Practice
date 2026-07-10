using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return RemoteDataManager.Init();
    }
}
