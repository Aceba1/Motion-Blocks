using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using System.Reflection;

namespace MotionBlocks.ModuleLoaders
{
    public class JSONModuleReflexGyro : JSONModuleLoader
    {
        public override bool CreateModuleForBlock(int blockID, ModdedBlockDefinition def, TankBlock block, JToken data)
        {
            if (data.Type == JTokenType.Object)
            {
                JObject obj = (JObject)data;
                try
                {
                    ModuleReflexGyro gyro = base.GetOrAddComponent<ModuleReflexGyro>(block);
                    gyro.AllAxis = CustomParser.LenientTryParseBool(obj, "AllAxis", gyro.AllAxis);
                    gyro.Axis = CustomParser.LenientTryParseVector3(obj, "Axis", gyro.Axis);
                    gyro.Strength = CustomParser.LenientTryParseFloat(obj, "Strength", gyro.Strength);
                    gyro.MaxStrength = CustomParser.LenientTryParseFloat(obj, "MaxStrength", gyro.MaxStrength);
                    gyro.DrumAnimSpeed = CustomParser.LenientTryParseFloat(obj, "DrumAnimSpeed", gyro.DrumAnimSpeed);
                    gyro.MemoryExtent = CustomParser.LenientTryParseFloat(obj, "MemoryExtent", gyro.MemoryExtent);
                    return true;
                }
                catch (Exception e)
                {
                    d.LogException(e);
                    d.LogError("Destroying added ModuleReflexGyro");
                    ModuleReflexGyro failedModule = block.GetComponent<ModuleReflexGyro>();
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
            return "ModuleReflexGyro";
        }
    }
}
