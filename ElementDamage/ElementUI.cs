using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BossAmiya
{
    public class ElementUI
    {
        private static ElementUI _instance;
        public static ElementUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ElementUI();
                }
                return _instance;
            }
        }
        private GameObject elementUI;
        private Dictionary<AgentModel, GameObject> elementUIs = new Dictionary<AgentModel, GameObject>();
        private Dictionary<AgentModel, DarkElementController> elementControllers = new Dictionary<AgentModel, DarkElementController>();
        public DarkElementController GetElementController(AgentModel agent)
        {
            if (elementControllers.ContainsKey(agent))
            {
                return elementControllers[agent];
            }
            return null;
        }
        public void InitAB()
        {
            var bundle = AssetBundle.LoadFromFile(Harmony_Patch.path + "/AssetsBundle/darkelement");
            elementUI = bundle.LoadAsset<GameObject>("DarkElement");
            bundle.Unload(false);
        }
        public void AddElementUIToAgent(AgentModel agent)
        {
            var newElementUI = GameObject.Instantiate(elementUI);
            var MaskSystem = newElementUI.transform.Find("MaskSystem");
            var script = MaskSystem.gameObject.AddComponent<DarkElementController>();
            elementControllers.Add(agent, script);
            script.Reset();
            newElementUI.transform.SetParent(agent.GetUnit().agentUI.transform, false);
            newElementUI.transform.localPosition = new Vector3(-300, 0, 0);
            newElementUI.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            elementUIs.Add(agent, newElementUI);
        }
        public void RemoveElementUIFromAgent(AgentModel agent)
        {
            if (elementUIs.ContainsKey(agent))
            {
                GameObject.Destroy(elementUIs[agent]);
                elementUIs.Remove(agent);
            }
            if (elementControllers.ContainsKey(agent))
            {
                elementControllers.Remove(agent);
            }
        }
    }
}
public class DarkElementController : MonoBehaviour
{
    [SerializeField]
    public Image maskImage;
    public void SetProgress(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);
        maskImage.fillAmount = percentage / 100f;
    }
    public void Reset()
    {
        maskImage = GetComponent<Image>();
    }
}