using MotionBlocks.ModuleLoaders;
using UnityEngine;

namespace MotionBlocks
{
    public class MotionBlocksMod : ModBase
    {
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
                var host = new GameObject("Motion Blocks singleton host");
                host.AddComponent<OptionMenuDensityShift>();
                host.AddComponent<GUIOverseer>();
                UnityEngine.Object.DontDestroyOnLoad(host);
                Inited = true;
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