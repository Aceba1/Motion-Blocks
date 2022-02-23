using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using System.Reflection;

namespace MotionBlocks.ModuleLoaders
{
    public class JSONModuleFloater : JSONModuleLoader
    {
        public override bool CreateModuleForBlock(int blockID, ModdedBlockDefinition def, TankBlock block, JToken data)
        {
            if (data.Type == JTokenType.Object)
            {
                JObject obj = (JObject)data;
                try
                {
                    ModuleFloater floater = base.GetOrAddComponent<ModuleFloater>(block);
                    floater.MinHeight = CustomParser.LenientTryParseFloat(obj, "MinHeight", floater.MinHeight);
                    floater.MaxHeight = CustomParser.LenientTryParseFloat(obj, "MaxHeight", floater.MaxHeight);
                    floater.MaxStrength = CustomParser.LenientTryParseFloat(obj, "MaxStrength", floater.MaxStrength);
                    floater.VelocityDampen = CustomParser.LenientTryParseFloat(obj, "VelocityDampen", floater.VelocityDampen);
                    return true;
                }
                catch (Exception e)
                {
                    d.LogException(e);
                    d.LogError("Destroying added ModuleFloater");
                    ModuleFloater failedModule = block.GetComponent<ModuleFloater>();
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
            return "ModuleFloater";
        }
    }
}
