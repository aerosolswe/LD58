using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.DOTweenEditor;
using System.Linq;

[CustomEditor(typeof(StateSwitcher))]
public class StateSwitcherEditor : Editor
{
    private StateSwitcher switcher;
    private SerializedProperty statesProp;
    private SerializedProperty selectedStateProp;
    private SerializedProperty executeOnAwakeProp;
    private SerializedProperty executeStateProp;

    private void OnEnable()
    {
        switcher = (StateSwitcher)target;
        statesProp = serializedObject.FindProperty("states");
        selectedStateProp = serializedObject.FindProperty("SelectedState");
        executeOnAwakeProp = serializedObject.FindProperty("ExecuteOnAwake");
        executeStateProp = serializedObject.FindProperty("ExecuteState");
    }

    private void OnDisable()
    {
        DOTweenEditorPreview.Stop();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("State Switcher", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(executeOnAwakeProp);

        if (executeOnAwakeProp.boolValue)
        {
            EditorGUILayout.PropertyField(executeStateProp);
        }

        // State buttons
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < statesProp.arraySize; i++)
        {
            GUI.backgroundColor = selectedStateProp.intValue == i ? Color.green : Color.white;
            if (GUILayout.Button($"State {i}"))
            {
                DOTweenEditorPreview.Start();
                selectedStateProp.intValue = i;
                switcher.ApplyState(i);

                foreach (var anim in switcher.States[i].animations)
                {
                    Tween previewTween = anim.Apply();
                    if (previewTween != null)
                        DOTweenEditorPreview.PrepareTweenForPreview(previewTween);
                }

                SceneView.RepaintAll();
                EditorUtility.SetDirty(switcher);
            }

            // Remove state button
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                statesProp.DeleteArrayElementAtIndex(i);
                selectedStateProp.intValue = Mathf.Clamp(selectedStateProp.intValue, 0, statesProp.arraySize - 1);
                break;
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("+ Add State"))
        {
            statesProp.arraySize++;
            var newState = statesProp.GetArrayElementAtIndex(statesProp.arraySize - 1);
            newState.FindPropertyRelative("animations").ClearArray();
        }

        EditorGUILayout.Space();

        // Draw selected state's animations
        if (selectedStateProp.intValue < statesProp.arraySize && selectedStateProp.intValue >= 0)
        {
            SerializedProperty stateProp = statesProp.GetArrayElementAtIndex(selectedStateProp.intValue);
            SerializedProperty animsProp = stateProp.FindPropertyRelative("animations");

            EditorGUILayout.LabelField($"Animations for State {selectedStateProp.intValue}", EditorStyles.boldLabel);

            for (int j = 0; j < animsProp.arraySize; j++)
            {
                EditorGUILayout.BeginHorizontal();

                SerializedProperty animProp = animsProp.GetArrayElementAtIndex(j);
                EditorGUILayout.PropertyField(animProp, new GUIContent($"Animation {j}"), true);

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    animsProp.DeleteArrayElementAtIndex(j);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Animation"))
            {
                GenericMenu menu = new GenericMenu();

                // Find all non-abstract types that inherit from any StateAnimation<>
                var baseType = typeof(StateAnimation<>);
                var animTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => !type.IsAbstract &&
                                   type.BaseType != null &&
                                   type.BaseType.IsGenericType &&
                                   type.BaseType.GetGenericTypeDefinition() == baseType)
                    .ToList();

                foreach (var type in animTypes)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        var newAnim = Activator.CreateInstance(type);
                        animsProp.arraySize++;
                        animsProp.GetArrayElementAtIndex(animsProp.arraySize - 1).managedReferenceValue = newAnim;
                        serializedObject.ApplyModifiedProperties();
                    });
                }

                menu.ShowAsContext();
            }
        }

        if (GUILayout.Button("Auto-Sync States"))
        {
            if (switcher.States.Count > 1)
            {
                var template = switcher.States[0]; // use first state as template

                foreach (var state in switcher.States)
                {
                    if (state == template)
                        continue;

                    // ensure state has at least as many animations as the template
                    for (int i = 0; i < template.animations.Count; i++)
                    {
                        if (i >= state.animations.Count)
                        {
                            // add missing animation slot
                            var templateAnim = template.animations[i];
                            var type = templateAnim.GetType();

                            var json = JsonUtility.ToJson(templateAnim);
                            var newAnim = (IStateAnimation)Activator.CreateInstance(type);
                            JsonUtility.FromJsonOverwrite(json, newAnim);

                            state.animations.Add(newAnim);
                        } else
                        {
                            // same index exists but could be wrong type
                            var templateAnim = template.animations[i];
                            var currentAnim = state.animations[i];

                            if (currentAnim.GetType() != templateAnim.GetType())
                            {
                                var type = templateAnim.GetType();
                                var json = JsonUtility.ToJson(templateAnim);
                                var newAnim = (IStateAnimation)Activator.CreateInstance(type);
                                JsonUtility.FromJsonOverwrite(json, newAnim);

                                state.animations[i] = newAnim;
                            }
                        }
                    }
                }

                EditorUtility.SetDirty(switcher);
                serializedObject.Update();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
