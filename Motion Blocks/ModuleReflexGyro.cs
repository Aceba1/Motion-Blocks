using System;
using UnityEngine;

namespace MotionBlocks
{
    public class ModuleReflexGyro : Module
    {
        /// <summary>
        /// Aid in all axis, or only Yaw
        /// </summary>
        public bool AllAxis;
        public float Strength = 200;
        public float MaxStrength = 200f;
        public float DrumAnimSpeed = 0.2f;
        public Vector3 Axis = Vector3.up;

        private Vector3 TiltInput;
        Transform Drum;
        Vector3 Debt = Vector3.zero;
        public float MemoryExtent = 2f;

        public void OnPool()
        {
            block.AttachedEvent.Subscribe(new Action(this.OnAttach));
            block.DetachingEvent.Subscribe(new Action(this.OnDetach));
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
                if (block.tank.beam.IsActive)
                    Debt = Debt * 0.95f;

                Debt = Vector3.ClampMagnitude(Debt + localAngularVelocity, MaxStrength * MemoryExtent);

                float ratio = Strength * 0.5f;

                float _m = Mathf.Min(rbody.mass, ratio) / ratio;
                var torque = rootblock.TransformVector(Vector3.ClampMagnitude(Debt, MaxStrength)) * _m;
                rbody.AddTorque(-torque, ForceMode.Impulse);
                Drum.Rotate(transform.InverseTransformVector(torque) * DrumAnimSpeed, Space.Self);
            }
        }
    }
}
