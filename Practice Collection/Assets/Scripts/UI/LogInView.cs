using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogInView : MonoBehaviour
{
    
    [SerializeField]private TextMeshProUGUI usernameText;
    [SerializeField]private TextMeshProUGUI passwordText;
    [SerializeField]private Button logInButton;
    
    private void Start()
    {
        logInButton.onClick.AddListener(() =>
        {
            //进入一个场景
            LoadingManager.Instance.LoadSceneAsync("",1.0f,() =>{},( )=>{},()=>{});
        });
    }

    private void LoadGameScene()
    {
        
    }
    
}
