//using Harmony;
using Nuterra.BlockInjector;
using UnityEngine;

namespace MotionBlocks
{
    public class Class1
    {
        public static void CreateBlocks()
        {
            //var harmony = HarmonyInstance.Create("examplepack.changethisname");
            //harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            var gso_mat = GameObjectJSON.GetObjectFromGameResources<Material>("GSO_Main");
            var host = new GameObject("Motion Blocks singleton host");
            host.AddComponent<GUIOverseer>();
            {
                var gso_rag = new BlockPrefabBuilder(/*"GSOBlock(111)", false*/) //Use a reference if you want quick functionality
                    .SetBlockID(9210)
                    .SetName("GSO Reflex Axis Gyro")
                    .SetDescription("This 'RAG' is capable of providing a corrective force against idle rotation, and give you the push you need for those sharp angles")
                    .SetPrice(6000)
                    .SetHP(1250)
                    .SetFaction(FactionSubTypes.GSO)
                    .SetCategory(BlockCategories.Accessories)
                    .SetIcon(GameObjectJSON.ImageFromFile(Properties.Resources.gso_reflex_axis_gyro_png))
                    .SetMass(8f)
                    .SetSizeManual(
                        new IntVector3[] {
                        IntVector3.zero, IntVector3.right, IntVector3.forward, new IntVector3(1,0,1) },
                        new Vector3[] {
                        new Vector3(0f, 0f, -0.5f), new Vector3(1f, 0f, -0.5f),
                        new Vector3(-0.5f, 0f, 0f), new Vector3(-0.5f, 0f, 1f),
                        new Vector3(0f, 0f, 1.5f), new Vector3(1f, 0f, 1.5f),
                        new Vector3(1.5f, 0f, 0f), new Vector3(1.5f, 0f, 1f) })
                    .SetModel(GameObjectJSON.MeshFromData(Properties.Resources.gso_rag_base), true, gso_mat)
                    .AddComponent<ModuleReflexGyro>(out ModuleReflexGyro gyro);
                GameObject drum = new GameObject("Gyro_Drum");
                drum.AddComponent<MeshFilter>().sharedMesh = GameObjectJSON.MeshFromData(Properties.Resources.gso_rag_drum);
                drum.AddComponent<MeshRenderer>().sharedMaterial = gso_mat;
                drum.transform.parent = gso_rag.Prefab.transform;
                drum.transform.localPosition = new Vector3(0.5f, 0f, 0.5f);
                gso_rag.RegisterLater();

                gyro.MaxStrength = 200f;
                gyro.Strength = 200f;
                gyro.AllAxis = false;
            }
            host.AddComponent<OptionMenuDensityShift>();
            {
                var gso_ballast = new BlockPrefabBuilder()
                    .SetBlockID(9211)
                    .SetName("GSO Graviton Ballast")
                    .SetDescription("Composed of a strange lead-like alloy which's gravitational influence can be altered through exposure to electromagnetic polarity")
                    .SetPrice(750)
                    .SetHP(600)
                    .SetFaction(FactionSubTypes.GSO)
                    .SetGrade(2)
                    .SetCategory(BlockCategories.Accessories)
                    .SetIcon(GameObjectJSON.ImageFromFile(Properties.Resources.gso_ballast_png))
                    .SetMass(4)
                    .SetSize(IntVector3.one, BlockPrefabBuilder.AttachmentPoints.All)
                    .SetModel(GameObjectJSON.MeshFromData(Properties.Resources.gso_ballast), true, gso_mat)
                    .AddComponent<ModuleDensityShift>(out ModuleDensityShift module);
                GameObject weight = new GameObject("Ballast_Weight");
                weight.AddComponent<MeshFilter>().sharedMesh = GameObjectJSON.MeshFromData(Properties.Resources.gso_ballast_weight);
                weight.AddComponent<MeshRenderer>().sharedMaterial = gso_mat;
                weight.transform.parent = gso_ballast.Prefab.transform;
                weight.transform.localPosition = Vector3.zero;
                gso_ballast.RegisterLater();

                module.Range = new Vector2(4, 16);
            }
        }
    }
}