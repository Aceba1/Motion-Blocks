using System;
using UnityEngine;

namespace MotionBlocks
{
    class GUIOverseer : MonoBehaviour
    {
        public GUIOverseer()
        {
            inst = this;
        }
        public static GUIOverseer inst;
        public static void CheckValid()
        {
            inst.gameObject.SetActive(
            OptionMenuDensityShift.inst.check_OnGUI()// ||
            );
        }
        public void OnGUI()
        {
            OptionMenuDensityShift.inst.stack_OnGUI();
        }
    }

    internal class OptionMenuDensityShift : MonoBehaviour
    {
        public OptionMenuDensityShift()
        {
            inst = this;
        }
        public static OptionMenuDensityShift inst;

        private readonly int ID = 9283;

        private bool visible = false;

        private ModuleDensityShift module;

        private Rect win;

        private float tempMass;
        private string tempString;

        private void Update()
        {
            if (!Singleton.Manager<ManPointer>.inst.DraggingItem && Input.GetMouseButtonDown(1))
            {
                win = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - 75f, 250f, 150f);
                try
                {
                    module = Singleton.Manager<ManPointer>.inst.targetVisible.block.GetComponent<ModuleDensityShift>();
                }
                catch
                {
                    module = null;
                }
                visible = module;
                if (visible)
                {
                    tempMass = module.Mass;
                    tempString = tempMass.ToString();
                }
            }
        }

        public bool check_OnGUI()
        {
            return visible && module;
        }

        public void stack_OnGUI()
        {
            if (!visible || !module)
            {
                return;
            }

            try
            {
                win = GUI.Window(ID, win, new GUI.WindowFunction(DoWindow), module.gameObject.name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DoWindow(int id)
        {
            if (module == null)
            {
                visible = false;
                return;
            }

            GUILayout.Label("Mass Range : " + module.Range.ToString());

            GUI.changed = false;
            tempString = GUILayout.TextField(tempString);
            if (GUI.changed && float.TryParse(tempString, out float stringMass))
            {
                tempMass = Mathf.Clamp(stringMass, module.Range.x, module.Range.y);
                module.SetMass(tempMass);
            }

            GUI.changed = false;
            tempMass = Mathf.Round(GUILayout.HorizontalSlider(module.Mass, module.Range.x, module.Range.y) * 4) * 0.25f;
            if (GUI.changed)
            {
                tempString = tempMass.ToString();
                module.SetMass(tempMass);
            }

            GUILayout.Label("Current Mass : " + module.Mass.ToString());

            if (GUILayout.Button("Close"))
            {
                visible = false;
                module = null;
            }
            GUI.DragWindow();
        }
    }
}
