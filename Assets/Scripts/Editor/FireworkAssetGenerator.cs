using UnityEngine;
using UnityEditor;
using System.IO;

public static class FireworkAssetGenerator
{
    private const string RootPath = "Assets/ScriptableObjects";

    [MenuItem("Tools/Seoul Firework/Generate Default ScriptableObjects")]
    public static void GenerateDefaultScriptableObjects()
    {
        EnsureFolder("Assets", "ScriptableObjects");

        CreateOrUpdateFirework(
            "Firework_Peony",
            FireworkType.Peony,
            "Peony",
            0.8f,
            10,
            800,
            30);

        CreateOrUpdateFirework(
            "Firework_Niagara",
            FireworkType.Niagara,
            "Niagara",
            3.5f,
            35,
            8500,
            90);

        CreateOrUpdateFirework(
            "Firework_Ring",
            FireworkType.Ring,
            "Ring",
            2.0f,
            25,
            3200,
            120);

        CreateOrUpdateFirework(
            "Firework_Willow",
            FireworkType.Willow,
            "Willow",
            4.0f,
            45,
            5000,
            260);

        CreateOrUpdateFirework(
            "Firework_Crossette",
            FireworkType.Crossette,
            "Crossette",
            6.0f,
            80,
            18000,
            650);

        CreateOrUpdateCombo(
            "Combo_GoldenBridgeShower",
            "골든 브리지 샤워",
            new[] { FireworkType.Niagara, FireworkType.Willow },
            false,
            2.5f,
            15000,
            400,
            "골든 브리지 샤워!");

        CreateOrUpdateCombo(
            "Combo_LoveSparkle",
            "러브 스파클",
            new[] { FireworkType.Ring, FireworkType.Peony, FireworkType.Ring },
            true,
            3.0f,
            10000,
            500,
            "러브 스파클!");

        CreateOrUpdateCombo(
            "Combo_ArtisanClassic",
            "장인의 정석",
            new[] { FireworkType.Peony, FireworkType.Peony, FireworkType.Peony, FireworkType.Peony },
            true,
            2.5f,
            5000,
            250,
            "장인의 정석!");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Default FireworkData(5) + ComboRule(3) generated in Assets/ScriptableObjects/");
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = parent + "/" + child;
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, child);
        }
    }

    private static void CreateOrUpdateFirework(
        string fileName,
        FireworkType type,
        string displayName,
        float cooldown,
        int resourceCost,
        int audienceGain,
        int satisfactionGain)
    {
        string assetPath = Path.Combine(RootPath, fileName + ".asset").Replace("\\", "/");
        FireworkData data = AssetDatabase.LoadAssetAtPath<FireworkData>(assetPath);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<FireworkData>();
            AssetDatabase.CreateAsset(data, assetPath);
        }

        SerializedObject so = new SerializedObject(data);
        so.FindProperty("type").enumValueIndex = (int)type;
        so.FindProperty("displayName").stringValue = displayName;
        so.FindProperty("cooldown").floatValue = cooldown;
        so.FindProperty("resourceCost").intValue = resourceCost;
        so.FindProperty("audienceGain").intValue = audienceGain;
        so.FindProperty("satisfactionGain").intValue = satisfactionGain;
        so.ApplyModifiedProperties();

        EditorUtility.SetDirty(data);
    }

    private static void CreateOrUpdateCombo(
        string fileName,
        string comboName,
        FireworkType[] sequence,
        bool enforceOrder,
        float timeLimit,
        int bonusAudience,
        int bonusSatisfaction,
        string toastMessage)
    {
        string assetPath = Path.Combine(RootPath, fileName + ".asset").Replace("\\", "/");
        ComboRule rule = AssetDatabase.LoadAssetAtPath<ComboRule>(assetPath);

        if (rule == null)
        {
            rule = ScriptableObject.CreateInstance<ComboRule>();
            AssetDatabase.CreateAsset(rule, assetPath);
        }

        SerializedObject so = new SerializedObject(rule);
        so.FindProperty("comboName").stringValue = comboName;
        SerializedProperty sequenceProp = so.FindProperty("requiredSequence");
        sequenceProp.arraySize = sequence.Length;
        for (int i = 0; i < sequence.Length; i++)
        {
            sequenceProp.GetArrayElementAtIndex(i).enumValueIndex = (int)sequence[i];
        }

        so.FindProperty("enforceOrder").boolValue = enforceOrder;
        so.FindProperty("timeLimitSeconds").floatValue = timeLimit;
        so.FindProperty("bonusAudience").intValue = bonusAudience;
        so.FindProperty("bonusSatisfaction").intValue = bonusSatisfaction;
        so.FindProperty("toastMessage").stringValue = toastMessage;
        so.ApplyModifiedProperties();

        EditorUtility.SetDirty(rule);
    }
}
