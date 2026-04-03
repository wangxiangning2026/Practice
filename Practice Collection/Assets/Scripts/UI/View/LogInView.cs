using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//登录视图界面
public class LogInView : UIViewBase
{
    private const string path = "Assets/Scenes/PlayGame.unity";

    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI passwordText;
    [SerializeField] private Button logInButton;

    private void Start()
    {
        logInButton.onClick.AddListener(() =>
        {
            //进入一个场景
            StartCoroutine(LoadGameScene());
        });
    }

    private IEnumerator LoadGameScene()
    {
        yield return LoadingManager.Instance.LoadSceneAsync(path, 1.0f, () => { }, () => { }, () => { });
    }
}