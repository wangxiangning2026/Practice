using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    public bool isHead;
    public int Index { get; private set; }
    public void Init(int index) => Index = index;
}
