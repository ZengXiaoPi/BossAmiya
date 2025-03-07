using System.Collections.Generic;
using UnityEngine;

namespace BossAmiya
{
    public class ElementManager
    {
        private static ElementManager _instance;
        public static ElementManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ElementManager();
                }
                return _instance;
            }
        }
        public Dictionary<AgentModel, float> ElementDamageDict = new Dictionary<AgentModel, float>();
        public Dictionary<AgentModel, bool> ElementUnitIsBreaking = new Dictionary<AgentModel, bool>();
        public void GiveElementDamage(AgentModel unit, float damage)
        {
            // When the unit is in breaking state, dont give damage to it.
            if (ElementUnitIsBreaking.ContainsKey(unit))
            {
                if (ElementUnitIsBreaking[unit] == true) return;
            }
            if (ElementDamageDict.ContainsKey(unit))
            {
                ElementDamageDict[unit] += damage;
                var script = ElementUI.Instance.GetElementController(unit);
                script.SetProgress(ElementDamageDict[unit] / 80f * 100f);
                CheckElement(unit);
            }
            else
            {
                ElementDamageDict.Add(unit, damage);
                ElementUnitIsBreaking.Add(unit, false);
                ElementUI.Instance.AddElementUIToAgent(unit);
                var script = ElementUI.Instance.GetElementController(unit);
                script.SetProgress(damage / 80f * 100f);
                CheckElement(unit);
            }
        }
        public void CheckElement(AgentModel unit)
        {
            if (ElementUnitIsBreaking.ContainsKey(unit))
            {
                if (ElementUnitIsBreaking[unit] == true) return;
            }
            if (ElementDamageDict[unit] >= 80)
            {
                var script = ElementUI.Instance.GetElementController(unit);
                script.SetProgress(100f);
                unit.AddUnitBuf(new Element_ElementBreaking(unit, script));
                ElementDamageDict.Remove(unit);
                ElementUnitIsBreaking[unit] = true;
            }
        }
        public float GetElementDamage(AgentModel unit)
        {
            if (!ElementDamageDict.ContainsKey(unit))
            {
                return -1;
            }
            else if (ElementUnitIsBreaking[unit] == true)
            {
                return -2;
            }
            else
            {
                return ElementDamageDict[unit];
            }
        }
        public void ClearDictionary()
        {
            ElementDamageDict.Clear();
            ElementUnitIsBreaking.Clear();
        }
    }
}
