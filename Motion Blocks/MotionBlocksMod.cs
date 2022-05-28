using MotionBlocks.ModuleLoaders;
using UnityEngine;

namespace MotionBlocks
{
    public class MotionBlocksMod : ModBase
    {
        internal static Transform dummyTransform;

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