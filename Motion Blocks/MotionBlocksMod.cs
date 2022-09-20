using System;
using MotionBlocks.ModuleLoaders;
using Steamworks;
using UnityEngine;

namespace MotionBlocks
{
    public class MotionBlocksMod : ModBase
    {
        internal static Transform dummyTransform;

        // const int CurrentStable = 9014917;
        // internal static bool IsUnstableBuild = false;

        public override void DeInit()
        {
        }

        public override bool HasEarlyInit()
        {
            return true;
        }

        internal static bool Inited = false;

        public override void EarlyInit()
        {
            if (!Inited)
            {
                Inited = true;
                int currentBuild = SteamApps.GetAppBuildId();
                Console.WriteLine("[MotionBlocks] ManagedEarlyInit");
                Console.WriteLine($"[MotionBlocks] Current Build: {currentBuild}");
                // IsUnstableBuild = currentBuild != CurrentStable;
            }
        }

        public override void Init()
        {
            JSONBlockLoader.RegisterModuleLoader(new JSONModuleDensityShift());
            JSONBlockLoader.RegisterModuleLoader(new JSONModuleReflexGyro());
            JSONBlockLoader.RegisterModuleLoader(new JSONModuleFloater());
        }
    }
}