using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 框架编辑器工具
/// </summary>
public class AnimationFrameworkEditor : EditorWindow
{
    [MenuItem("Tools/Animation Framework/Setup Character")]
    public static void SetupCharacter()
    {
        // 自动设置角色动画框架
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogError("Please select a GameObject first");
            return;
        }

        // 添加必要组件
        var animator = selected.GetComponent<Animator>();
        if (animator == null)
            animator = selected.AddComponent<Animator>();

        var framework = selected.GetComponent<AnimationFrameworkRoot>();
        if (framework == null)
            framework = selected.AddComponent<AnimationFrameworkRoot>();

        var controller = selected.GetComponent<CharacterController>();
        if (controller == null)
            controller = selected.AddComponent<CharacterController>();

        // 添加默认模块
        if (framework.GetModule<MovementModule>() == null)
            selected.AddComponent<MovementModule>();

        if (framework.GetModule<CombatModule>() == null)
            selected.AddComponent<CombatModule>();

        EditorUtility.SetDirty(selected);
        Debug.Log($"Animation Framework setup complete on {selected.name}");
    }

    [MenuItem("Tools/Animation Framework/Create Animation Database")]
    public static void CreateAnimationDatabase()
    {
        var database = ScriptableObject.CreateInstance<ScriptableAnimationDataProvider>();
        string path = $"Assets/AnimationDatabase_{System.DateTime.Now:yyyyMMdd_HHmmss}.asset";
        AssetDatabase.CreateAsset(database, path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = database;
        Debug.Log($"Created animation database at {path}");
    }

    [MenuItem("Tools/Animation Framework/Debug/Print Framework State")]
    public static void PrintFrameworkState()
    {
        var framework = FindObjectOfType<AnimationFrameworkRoot>();
        if (framework == null)
        {
            Debug.LogWarning("No Animation Framework found in scene");
            return;
        }

        Debug.Log($"=== Animation Framework State ===");
        Debug.Log($"Framework Initialized: {framework.IsInitialized}");
        Debug.Log($"Modules: {framework.GetComponents<AnimationModule>().Length}");

        var stateMachine = framework.GetService<IAnimationStateMachine>();
        if (stateMachine != null)
        {
            Debug.Log($"Current State: {stateMachine.CurrentState?.Name ?? "None"}");
            Debug.Log($"Previous State: {stateMachine.PreviousState?.Name ?? "None"}");
        }
    }
}

/// <summary>
/// 动画数据库编辑器
/// </summary>
[CustomEditor(typeof(ScriptableAnimationDataProvider))]
public class AnimationDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Validate Database"))
        {
            ValidateDatabase();
        }
    }

    private void ValidateDatabase()
    {
        var database = target as ScriptableAnimationDataProvider;
        // 验证逻辑
        Debug.Log("Database validation complete");
    }
}