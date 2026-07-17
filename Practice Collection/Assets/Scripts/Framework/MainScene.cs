using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    [SerializeField][FieldName("UIView预制体")] private GameObject ViewNode;
    
    IEnumerator Start()
    {
        Instantiate(ViewNode, transform);

        yield return null;
        
        UIManager.Instance.OpenUI<MainView>();
    }
}
