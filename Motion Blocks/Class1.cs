//using Harmony;
using Nuterra.BlockInjector;
using System;
using System.Reflection;
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
            var gso_rag = new BlockPrefabBuilder(/*"GSOBlock(111)", false*/) //Use a reference if you want quick functionality
                .SetBlockID(9210)
                .SetName("GSO Reflex Axis Gyro")
                .SetDescription("This 'RAG' is capable of providing a corrective force against idle rotation, and give you the push you need for those sharp angles")
                .SetPrice(6000)
                .SetHP(100)
                .SetFaction(FactionSubTypes.GSO)
                .SetCategory(BlockCategories.Accessories)
                .SetIcon(GameObjectJSON.SpriteFromImage(GameObjectJSON.ImageFromFile(Properties.Resources.gso_reflex_axis_gyro_png)))
                .SetMass(6f)
                .SetSizeManual(
                    new IntVector3[] {
                        IntVector3.zero, IntVector3.right, IntVector3.forward, new IntVector3(1,0,1) },
                    new Vector3[] {
                        new Vector3(0f, 0f, -0.5f), new Vector3(1f, 0f, -0.5f),
                        new Vector3(-0.5f, 0f, 0f), new Vector3(-0.5f, 0f, 1f),
                        new Vector3(0f, 0f, 1.5f), new Vector3(1f, 0f, 1.5f),
                        new Vector3(1.5f, 0f, 0f), new Vector3(1.5f, 0f, 1f) })
                .SetModel(GameObjectJSON.MeshFromData(Properties.Resources.gso_rag_base), true, gso_mat);
            GameObject drum = new GameObject("Gyro_Drum");
            drum.AddComponent<MeshFilter>().sharedMesh = GameObjectJSON.MeshFromData(Properties.Resources.gso_rag_drum);
            drum.AddComponent<MeshRenderer>().sharedMaterial = gso_mat;
            drum.transform.parent = gso_rag.Prefab.transform;
            drum.transform.localPosition = new Vector3(0.5f, 0f, 0.5f);
            gso_rag.RegisterLater();
            var gyro = gso_rag.Prefab.AddComponent<ModuleReflexGyro>();
            gyro.MaxStrength = 200f;
            gyro.Strength = 200f;
            gyro.AllAxis = false;
        }
    }

    public class ModuleReflexGyro : Module
    {
        /// <summary>
        /// Aid in all axis, or only Yaw
        /// </summary>
        public bool AllAxis;
        public float Strength = 200;
        public float MaxStrength = 200f;
        private Vector3 TiltInput;
        public Transform Drum;
        public Vector3 Debt = Vector3.zero;
        public float MemoryExtent = 2f;

        public void OnPool()
        {
            block.AttachEvent.Subscribe(new Action(this.OnAttach));
            block.DetachEvent.Subscribe(new Action(this.OnDetach));
            Drum = transform.Find("Gyro_Drum");
        }

        private void OnAttach()
        {
            base.block.tank.control.driveControlEvent.Subscribe(new Action<TankControl.ControlState>(this.OnControlInput));
        }

        private void OnDetach()
        {
            block.tank.control.driveControlEvent.Unsubscribe(new Action<TankControl.ControlState>(this.OnControlInput));
            TiltInput = Vector3.zero;
            Debt = Vector3.zero;
        }

        private void OnControlInput(TankControl.ControlState controlState)
        {
            TiltInput = controlState.InputRotation * MaxStrength;
        }

        public void FixedUpdate()
        {
            if (block.tank)
            {
                Transform rootblock = block.tank.rootBlockTrans;
                Rigidbody rbody = block.tank.rbody;

                var localAngularVelocity = rootblock.InverseTransformVector(rbody.angularVelocity) * Strength + TiltInput;
                if (!AllAxis)
                {
                    var projectionAxis = rootblock.InverseTransformDirection(transform.up);
                    localAngularVelocity = Vector3.Project(localAngularVelocity, projectionAxis);
                }
                Debt = Vector3.ClampMagnitude(Debt + localAngularVelocity, MaxStrength * MemoryExtent);

                float _m = Mathf.Min(rbody.mass + 25f, 25f) / 25f;
                var torque = rootblock.TransformVector(Vector3.ClampMagnitude(Debt * _m, MaxStrength));
                rbody.AddTorque(-torque, ForceMode.Impulse);
                Drum.Rotate(transform.InverseTransformVector(torque) * 0.2f, Space.Self);
            }
        }
    }
}