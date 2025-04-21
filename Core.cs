using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.ItemFramework;
using MelonLoader;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Properties;

[assembly: MelonInfo(typeof(QualityEffects.QualityEffects), "QualityEffects", "1.0.0", "SoulDaSythe", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace QualityEffects
{
    public static class BuildInfo
    {
        public const string Name = "Quality Effects";

        public const string Description = "This weed is awful, I can barely feel it!";

        public const string Author = "SoulDaSythe & Jumble";

        public const string Company = null;

        public const string Version = "1.0";

        public const string DownloadLink = null;
    }
    public class QualityEffects : MelonMod
    {

        public static float LastMultiplier { get; private set; }
        public static bool AthleticActive { get; private set; }
        public static float AthleticDifference = 0f;
        public static float EnergizingDifference = 0f;
        public static bool EnergizingActive { get; private set; }

        [HarmonyPatch(typeof(Il2CppScheduleOne.Properties.Athletic), "ApplyToPlayer")]
        public static class Patch_Athletic_ApplyToPlayer
        {
            public static void Postfix(Athletic __instance, Player player)
            {
                try
                {
                    var movement = player.GetComponentInChildren<Il2CppScheduleOne.PlayerScripts.PlayerMovement>();
                    if (movement == null)
                    {
                        Melon<QualityEffects>.Logger.Msg("[ATHLETIC] ❌ PlayerMovement not found in children.");
                        return;
                    }
                    Melon<QualityEffects>.Logger.Msg($"[ATHLETIC] Base Value -> {movement.MoveSpeedMultiplier}");
                    float qualityMultiplier = QualityEffectsHelper.GetMultiplier(player.ConsumedProduct, "ATHLETIC");

                    float newMoveSpeedMultiplier = ((movement.MoveSpeedMultiplier - 1) * qualityMultiplier) + 1;
                    AthleticDifference = newMoveSpeedMultiplier - 1;
                    movement.MoveSpeedMultiplier = newMoveSpeedMultiplier;
                    if (EnergizingActive && EnergizingDifference != 0f)
                    {
                        newMoveSpeedMultiplier += EnergizingDifference;
                    }
                    AthleticActive = true;
                    Melon<QualityEffects>.Logger.Msg($"[ATHLETIC] ✅ Applied multiplier: {qualityMultiplier:F2}, New Speed: {newMoveSpeedMultiplier:F2}");
                }
                catch (System.Exception ex)
                {
                    Melon<QualityEffects>.Logger.Msg($"[ATHLETIC] ❌ Exception in Postfix: {ex.Message}");
                }
            }      
        }

        [HarmonyPatch(typeof(Il2CppScheduleOne.Properties.Athletic), "ClearFromPlayer")]
        public static class Patch_Athletic_ClearFromPlayer
        {
            public static void Postfix()
            {
                AthleticActive = false;
            }
        }

        [HarmonyPatch(typeof(Il2CppScheduleOne.Properties.Energizing), "ApplyToPlayer")]
        public static class Patch_Energizing_ApplyToPlayer
        {

            public static void Postfix(Energizing __instance, Player player)
            {
                try
                {
                    var movement = player.GetComponentInChildren<Il2CppScheduleOne.PlayerScripts.PlayerMovement>();
                    if (movement == null)
                    {
                        Melon<QualityEffects>.Logger.Msg("[Energizing] ❌ PlayerMovement not found in children.");
                        return;
                    }

                    float qualityMultiplier = QualityEffectsHelper.GetMultiplier(player.ConsumedProduct, "Energizing");
                    Melon<QualityEffects>.Logger.Msg($"[Energizing] Base Value -> {movement.MoveSpeedMultiplier}");
                    float newMoveSpeedMultiplier = ((movement.MoveSpeedMultiplier - 1) * qualityMultiplier) + 1;
                    EnergizingDifference = newMoveSpeedMultiplier - 1;
                    movement.MoveSpeedMultiplier = newMoveSpeedMultiplier;
                    if (AthleticActive && AthleticDifference != 0f)
                    {
                        newMoveSpeedMultiplier += AthleticDifference;
                    }

                    EnergizingActive = true;
                    Melon<QualityEffects>.Logger.Msg($"[Energizing] ✅ Applied multiplier: {qualityMultiplier:F2}, New Speed: {movement.MoveSpeedMultiplier:F2}");
                }
                catch (System.Exception ex)
                {
                    Melon<QualityEffects>.Logger.Msg($"[Energizing] ❌ Exception in Postfix: {ex.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(Il2CppScheduleOne.Properties.Energizing), "ClearFromPlayer")]
        public static class Patch_Energizing_ClearFromPlayer
        {
            public static void Postfix()
            {
                EnergizingActive = false;
            }
        }

        [HarmonyPatch(typeof(Il2CppScheduleOne.Properties.AntiGravity), "ApplyToPlayer")]
        public static class Patch_AntiGrav_ApplyToPlayer
        {
            public static void Postfix(AntiGravity __instance, Player player)
            {
                try
                {
                    var movement = player.GetComponentInChildren<Il2CppScheduleOne.PlayerScripts.PlayerMovement>();
                    if (movement == null)
                    {
                        Melon<QualityEffects>.Logger.Msg("[AntiGrav] ❌ PlayerMovement not found in children.");
                        return;
                    }

                    float qualityMultiplier = QualityEffectsHelper.GetMultiplier(player.ConsumedProduct, "AntiGrav");

                    movement.gravityMultiplier += qualityMultiplier;

                    Melon<QualityEffects>.Logger.Msg($"[AntiGrav] ✅ Applied multiplier: {qualityMultiplier:F2}, New GravityMult: {movement.gravityMultiplier:F2}");
                }
                catch (System.Exception ex)
                {
                    Melon<QualityEffects>.Logger.Msg($"[AntiGrav] ❌ Exception in Postfix: {ex.Message}");
                }
            }      
        }

        [HarmonyPatch(typeof(Il2CppScheduleOne.Properties.AntiGravity), "ClearFromPlayer")]
        public static class Patch_AntiGrav_ClearFromPlayer
        {
            public static void Postfix(AntiGravity __instance, Player player)
            {
                var movement = player.GetComponentInChildren<Il2CppScheduleOne.PlayerScripts.PlayerMovement>();
                movement.gravityMultiplier = 1.4f;
            }
        }

        /*[HarmonyPatch(typeof(Shrinking), nameof(Shrinking.ApplyToPlayer))]
        public static class Patch_Shrinking_ApplyToPlayer
        {
            public static bool Prefix(Shrinking __instance, Player player)
            {
                Transform playerTransform = player.transform;
                float multiplier = QualityEffectsHelper.GetMultiplier(player.ConsumedProduct, "Shrinking");
                float baseTarget = 0.8f;
                float scalemod = 0f;

                switch (multiplier)
                {
                    case 0.5f:
                        scalemod = 1f;
                        break;
                    case 0.75f:
                        scalemod = 0.9f;
                        break;
                    case 1:
                        scalemod = 0.8f;
                        break;
                    case 1.25f:
                        scalemod = 0.7f;
                        break;
                    case 1.5f:
                        scalemod = 0.6f;
                        break;
                    default:
                        scalemod = 0.8f;
                        break;
                }
                float scaledTarget = Mathf.Clamp(scalemod, 0.5f, 1.0f);
                QualityScaler.StartScaleLerp(playerTransform, scaledTarget, 1f);
                return false;
            }
        }*/     
    }
    public static class QualityEffectsHelper
    {
        public static List<float> defaultMultipliers = new() { 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1f };
        public static Dictionary<string, List<float>> multipliers = new()
        {
            { "ATHLETIC", defaultMultipliers },
            { "Energizing", defaultMultipliers },
            { "AntiGrav", new List<float> { 0.2f, 0.1f, 0f, -0.3f, -0.6f, 0f } }
        };

        public static float GetMultiplier(object instance, string drugName)
        {
            try
            {
                // Ensure we're working with an Il2CppSystem.Object
                var il2cppInstance = new Il2CppSystem.Object(((Il2CppSystem.Object)instance).Pointer);

                // Get the "Quality" field from the IL2CPP type
                var qualityField = il2cppInstance.GetIl2CppType().GetField("Quality");
                if (qualityField == null)
                {
                    Melon<QualityEffects>.Logger.Msg($"[{drugName}] ❌ No Quality field found.");
                    return 1f;
                }

                // Get the boxed enum object
                var boxedQuality = qualityField.GetValue(il2cppInstance);
                if (boxedQuality == null)
                {
                    Melon<QualityEffects>.Logger.Msg($"[{drugName}] ⚠️ Quality field is null.");
                    return 1f;
                }

                // Properly unbox and convert to EQuality
                var il2cppBoxed = new Il2CppSystem.Object(boxedQuality.Pointer);
                var enumObj = Il2CppSystem.Enum.ToObject(Il2CppType.Of<EQuality>(), il2cppBoxed);
                int intVal = Il2CppSystem.Convert.ToInt32(enumObj);
                EQuality quality = (EQuality)intVal;

                var effectMultipliers = multipliers[drugName];

                if (effectMultipliers == null || effectMultipliers.Count < 6)
                {
                    Melon<QualityEffects>.Logger.Msg($"[{drugName}] ⚠️ Null or incomplete multiplier list.");
                    return 1f;
                }

                float multiplier = quality switch
                {
                    EQuality.Trash => effectMultipliers[0],
                    EQuality.Poor => effectMultipliers[1],
                    EQuality.Standard => effectMultipliers[2],
                    EQuality.Premium => effectMultipliers[3],
                    EQuality.Heavenly => effectMultipliers[4],
                    _ => effectMultipliers[5]
                };

                Melon<QualityEffects>.Logger.Msg($"[{drugName}] Quality: {quality} ({intVal}) → Multiplier: {multiplier}");
                return multiplier;
            }
            catch (System.Exception ex)
            {
                Melon<QualityEffects>.Logger.Msg($"[{drugName}] ❌ Exception while reading Quality: {ex.Message}");
                return 1f;
            }
        }
    }
}
