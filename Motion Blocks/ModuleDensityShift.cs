using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace MotionBlocks
{
    class ModuleDensityShift : ModuleVariableMass
    {
        public float Color = 0f;
        public Vector2 Range;
        public Vector2 ScaleRange = new Vector2(0.2f, 1.0f);
        public float animationDuration = 0.25f;
        public bool ScaleBallast = false;

        MeshRenderer _mr = null;
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
        private void SetColor(float color)
        {
            this.Color = color;
            float c = Mathf.Clamp01(1f - ((color - Range.x) / (Range.y - Range.x)));
            SetColor(new Color(c * 0.6f + 0.4f, c * 0.8f + 0.2f, c * 0.775f + 0.225f));
        }

        private float m_RemainingColorAnimDuration;
        private float m_LastColor;
        private float m_NextColor;

        // Reflection
        internal static BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        // The implemented EventNoParams will execute actions in the order they were subscribed
        // The component pool will also execute the base class methods before inherited class methods
        // This means SetBlockColor is guaranteed to execute after SetMassCubeScale
        // This will set CurrentFulfillment correctly
        private void SetBlockColor()
        {
            float num = Mathf.Lerp(this.Range.x, this.Range.y, base.CurrentFulfillment);
            if (Color == num)
            {
                return;
            }
            this.m_NextColor = num;
            this.m_LastColor = this.Color;
            this.m_RemainingColorAnimDuration = this.animationDuration;
        }

        internal static MethodInfo BaseUpdate = typeof(ModuleVariableMass).GetMethod("Update", InstanceFlags);

        public void Update()
        {
            if (this.ScaleBallast)
            {
                BaseUpdate.Invoke(this, null);
            }
            if (this.m_RemainingColorAnimDuration != 0f)
            {
                this.m_RemainingColorAnimDuration = Mathf.Max(this.m_RemainingColorAnimDuration - Time.deltaTime, 0f);
                float colorT = 1f - this.m_RemainingColorAnimDuration / this.animationDuration;
                this.SetColor(Mathf.Lerp(this.m_LastColor, this.m_NextColor, colorT));
            }
        }

        public void OnSpawn()
        {
            base.block.MassChangedEvent.Subscribe(new Action(this.SetBlockColor));
            this.SetBlockColor();
        }

        internal static FieldInfo m_ContextMenuType = typeof(TankBlock).GetField("m_ContextMenuType", InstanceFlags);
        internal static FieldInfo m_Flags = typeof(TankBlock).GetField("m_Flags", InstanceFlags);

        private void PrePool()
        {
            // This indicates to the game that we want this block to be associated with a context menu
            m_Flags.SetValue(base.block, TankBlock.Flags.HasContextMenu);
            m_ContextMenuType.SetValue(base.block, ManHUD.HUDElementType.MassControl);

            base.block.m_DefaultMass = this.Range.x;
            base.m_MassRange = new MinMaxFloat(0.0f, this.Range.y - this.Range.x);

            float defaultScale = 1.0f;
            base.m_MassDisplayObjectAnimationDuration = this.animationDuration;

            MeshRenderer ballastToScale = this.mr;
            bool createDummyBallast = true;
            if (this.ScaleBallast)
            {
                if (ballastToScale != null)
                {
                    defaultScale = Mathf.Abs(ballastToScale.transform.localScale.x);
                    base.m_MassDisplayObject = ballastToScale.transform;
                    ballastToScale.transform.localScale = (this.ScaleRange.x + this.ScaleRange.y) / 2 * defaultScale * Vector3.one;
                    createDummyBallast = false;
                    d.Log($"[Motion Blocks] Set {mr.name} as scaling display object for {base.block.name}");
                }
            }
            if (createDummyBallast)
            {
                GameObject dummyGO = new GameObject("dummy");
                dummyGO.transform.parent = base.transform;
                dummyGO.transform.localScale = (this.ScaleRange.x + this.ScaleRange.y) / 2 * defaultScale * Vector3.one;
                dummyGO.SetActive(false);
                base.m_MassDisplayObject = dummyGO.transform;
                d.Log($"[Motion Blocks] Dummy scaling object created for {base.block.name}");
            }
            base.m_MassDisplayObjectScaleRange = new MinMaxFloat(this.ScaleRange.x * defaultScale, this.ScaleRange.y * defaultScale);
        }

        public void OnRecycle()
        {
            base.block.MassChangedEvent.Unsubscribe(new Action(this.SetBlockColor));
        }
    }
}
