using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using System.Reflection;

namespace MotionBlocks.ModuleLoaders
{
    public class JSONModuleDensityShift : JSONModuleLoader
    {
        public override bool CreateModuleForBlock(int blockID, ModdedBlockDefinition def, TankBlock block, JToken data)
        {
            if (data.Type == JTokenType.Object)
            {
                JObject obj = (JObject)data;
                try
                {
                    ModuleDensityShift ballast = base.GetOrAddComponent<ModuleDensityShift>(block);
                    ballast.Range = CustomParser.LenientTryParseVector2(obj, "Range", Vector2.up);
                    ballast.ScaleRange = CustomParser.LenientTryParseVector2(obj, "ScaleRange", ballast.ScaleRange);
                    ballast.animationDuration = CustomParser.LenientTryParseFloat(obj, "AnimationDuration", ballast.animationDuration);
                    ballast.ScaleBallast = CustomParser.LenientTryParseBool(obj, "ScaleBallast", ballast.ScaleBallast);
                    return true;
                }
                catch (Exception e)
                {
                    d.LogException(e);
                    d.LogError("Destroying added ModuleDensityShift");
                    ModuleDensityShift failedModule = block.GetComponent<ModuleDensityShift>();
                    if (failedModule != null)
                    {
                        UnityEngine.GameObject.Destroy(failedModule);
                    }
                    return false;
                }
            }
            return false;
        }

        public override string GetModuleKey()
        {
            return "ModuleDensityShift";
        }
    }
}
