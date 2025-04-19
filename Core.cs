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

        public const string Author = "SoulDaSythe";

        public const string Company = null;

        public const string Version = "1.0";

        public const string DownloadLink = null;
    }
    public class QualityEffects : MelonMod
    {

        public static float LastMultiplier { get; private set; }

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

                    float qualityMultiplier = QualityEffectsHelper.GetMultiplier(player.ConsumedProduct, "ATHLETIC");

                    movement.MoveSpeedMultiplier = ((movement.MoveSpeedMultiplier - 1) * qualityMultiplier) + 1;

                    Melon<QualityEffects>.Logger.Msg($"[ATHLETIC] ✅ Applied multiplier: {qualityMultiplier:F2}, New Speed: {movement.MoveSpeedMultiplier:F2}");
                }
                catch (System.Exception ex)
                {
                    Melon<QualityEffects>.Logger.Msg($"[ATHLETIC] ❌ Exception in Postfix: {ex.Message}");
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
                        float gravitymod = 0f;
                        switch (qualityMultiplier)
                        {
                            case 0.5f:
                                gravitymod = 0.2f;
                                break;
                            case 0.75f:
                                gravitymod = 0.1f;
                                break;
                            case 1:
                                gravitymod = 0f;
                                break;
                            case 1.25f:
                                gravitymod = -0.3f;
                                break;
                            case 1.5f:
                                gravitymod = -0.6f;
                                break;
                            default:
                                gravitymod = 0;
                                break;
                        }
                        movement.gravityMultiplier += gravitymod;

                        Melon<QualityEffects>.Logger.Msg($"[AntiGrav] ✅ Applied multiplier: {gravitymod:F2}, New GravityMult: {movement.gravityMultiplier:F2}");
                    }
                    catch (System.Exception ex)
                    {
                        Melon<QualityEffects>.Logger.Msg($"[AntiGrav] ❌ Exception in Postfix: {ex.Message}");
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

         
            public static class QualityEffectsHelper
            {
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

                        // Apply multiplier
                        float multiplier = quality switch
                        {
                            EQuality.Trash => 0.5f,
                            EQuality.Poor => 0.75f,
                            EQuality.Standard => 1f,
                            EQuality.Premium => 1.25f,
                            EQuality.Heavenly => 1.5f,
                            _ => 1f
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
    }
}
