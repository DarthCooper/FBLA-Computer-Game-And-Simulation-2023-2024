using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialInfoSO", menuName = "ScriptableObjects/TutorialInfoSO", order = 1)]
public class TutorialDataSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")]
    public string displayName;

    public string Description;

    public bool previouslyLoaded;

    [Header("Requirements")]
    public TutorialDataSO[] Prerequisites;

    [Header("Steps")]
    public GameObject[] TutorialStepPrefabs;

    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
