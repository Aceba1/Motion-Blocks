using System;
using UnityEngine;

namespace MotionBlocks
{
    class ModuleDensityShift : Module
    {
        public float Mass;
        public float Color = 0f;
        public Vector2 Range;
        public void OnSpawn()
        {
            Mass = Range.x;
            UpdateMass();
        }

        public void SetMass(float mass)
        {
            Mass = mass;
            UpdateMass();
        }

        public void UpdateMass()
        {
            block.ChangeMass(Mass);
        }

        MeshRenderer _mr;
        MeshRenderer mr
        {
            get
            {
                if (_mr == null)
                {
                    _mr = transform.Find("Ballast_Weight").GetComponent<MeshRenderer>();
                }
                return _mr;
            }
        }
        public void SetColor(Color color)
        {
            mr.material.color = color;
        }

        private void OnAttachChange()
        {
            Color -= 0.1f;
            UpdateMass();
        }

        public void Update()
        {
            if (Color != Mass)
            {
                if (Color.Approximately(Mass, 0.01f))
                    Color = Mass;
                else
                    Color = Color * 0.925f + Mass * 0.075f;
                float c = Mathf.Clamp01(1f - ((Color - Range.x) / (Range.y - Range.x)));
                SetColor(new Color(c * 0.6f + 0.4f, c * 0.8f + 0.2f, c * 0.775f + 0.225f));
            }
        }

        public void OnPool()
        {
            base.block.serializeEvent.Subscribe(new Action<bool, TankPreset.BlockSpec>(this.OnSerialize));
            base.block.serializeTextEvent.Subscribe(new Action<bool, TankPreset.BlockSpec>(this.OnSerialize));
            block.AttachEvent.Subscribe(new Action(OnAttachChange));
            block.DetachEvent.Subscribe(new Action(OnAttachChange));
        }

        private void OnSerialize(bool saving, TankPreset.BlockSpec blockSpec)
        {
            if (saving)
            {
                ModuleDensityShift.SerialData serialData = new ModuleDensityShift.SerialData()
                {
                    mass = Mass
                };
                serialData.Store(blockSpec.saveState);
            }
            else
            {
                ModuleDensityShift.SerialData serialData2 = Module.SerialData<ModuleDensityShift.SerialData>.Retrieve(blockSpec.saveState);
                if (serialData2 != null)
                {
                    SetMass(serialData2.mass);
                }
            }
        }

        [Serializable]
        private new class SerialData : Module.SerialData<ModuleDensityShift.SerialData>
        {
            public float mass;
        }
    }
}
