using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using UnityEngine;

namespace RAIC
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ArtifactAPI), nameof(ItemAPI), nameof(RecalculateStatsAPI), nameof(ArtifactCodeAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class AdoptedItemsPlugin : BaseUnityPlugin
    {
        public const string ModVer = "0.0.1";
        public const string ModName = "AdoptedItems";
        public const string ModGuid = "com.RicoValdezio.AdoptedItems";
        public static AdoptedItemsPlugin instance;
        public static ManualLogSource logSource;
        public static AssetBundle assetBundle;

        private void Awake()
        {
            if (instance == null) instance = this;
            logSource = instance.Logger;
            //geneticAssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("GeneticsArtifact.ArtifactResources.genetics"));

            Items.MurasTriage.Init();
        }
    }
}
