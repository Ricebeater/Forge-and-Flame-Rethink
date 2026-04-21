using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy), true)]
public class EnemyEditor : Editor
{
    private Editor profileEditor;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // วาดตัวแปรทุกตัวในสคริปต์ ยกเว้น "maxHP" และ "damage" (และ m_Script ที่เป็นช่องบอกชื่อสคริปต์)
        // คุณสามารถเพิ่มชื่อตัวแปรที่อยากซ่อนในนี้ได้เลย
        DrawPropertiesExcluding(serializedObject, "m_Script", "maxHP", "damage");

        serializedObject.ApplyModifiedProperties();

        Enemy enemy = (Enemy)target;

        // วาด Profile Editor ต่อท้าย
        if (enemy.profile != null)
        {
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Enemy Profile Settings (Edit SO directly)", EditorStyles.boldLabel);

            CreateCachedEditor(enemy.profile, null, ref profileEditor);
            profileEditor.OnInspectorGUI();

            EditorGUILayout.EndVertical();
        }
    }
}