using PrimeTween;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(FadeScreenManager))]
class FadeScreenManagerEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();

    FadeScreenManager fadeScreen = (FadeScreenManager)target;

    EditorGUILayout.LabelField("Debug Stuffs", EditorStyles.boldLabel);
    if(GUILayout.Button("Animate"))
        fadeScreen.Animate();
  }
}
#endif