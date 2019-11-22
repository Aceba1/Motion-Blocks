using UnityEngine;

namespace MotionBlocks
{
    public class ModuleFloater : Module
    {
        public float MinHeight = -85f;
        public float MaxHeight = 400f;
        public float MaxStrength = 14f;
        public float VelocityDampen = 0.08f;
        void FixedUpdate()
        {
            if (block.IsAttached && block.tank != null && !block.tank.beam.IsActive)
            {
                Vector3 blockCenter = block.centreOfMassWorld;
                block.tank.rbody.AddForceAtPosition(Vector3.up * Mathf.Clamp(
                        (MaxStrength / MaxHeight) * (MaxHeight - blockCenter.y) 
                        - block.tank.rbody.GetPointVelocity(blockCenter).y * VelocityDampen, 0f, MaxStrength * 1.25f), 
                    blockCenter, ForceMode.Impulse);
            }
        }
    }
}
