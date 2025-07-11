using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Buff;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{

    class VBuffUI
    {
        public TMP_Text text;
        public GameObject gameObject;
        public bool isPermanent;
        public string buffName;

        public VBuffUI(GameObject gameObject, bool isPermanent, string buffName)
        {
            this.gameObject = gameObject;
            this.text = gameObject.GetComponentInChildren<TMP_Text>();
            this.isPermanent = isPermanent;
            this.buffName = buffName;
        }
        
        public void SetText(int value)
        {
            if (isPermanent)
            {
                text.text = $"{buffName} Layer: {value}";
            }
            else
            {
                text.text = $"{buffName} Duration: {value}";
            }
        }
    }
    
    public class VBuffGroupUI : VUIBehaviour
    {
        [SerializeField] private GameObject buffCellPrefab;

        private Dictionary<int, VBuffUI> _buffUIs;

        protected override void Awake()
        {
            base.Awake();
            _buffUIs = new Dictionary<int, VBuffUI>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnBuffAdded, OnBuffAdded);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnBuffRemoved, OnBuffRemoved);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnBuffAdded, OnBuffAdded);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnBuffRemoved, OnBuffRemoved);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
        }

        private void OnBuffValueUpdated(Dictionary<string, object> messagedict)
        {
            if (_buffUIs.TryGetValue((int)messagedict["Id"], out VBuffUI buffUI))
            {
                buffUI.SetText((int)messagedict["Value"]);
            }
        }

        private void OnBuffRemoved(Dictionary<string, object> messagedict)
        {
            int id = (int)messagedict["Id"];
            if (_buffUIs.TryGetValue((int)messagedict["Id"], out VBuffUI buffUI))
            {
                Destroy(buffUI.gameObject);
                _buffUIs.Remove(id);
            }
        }
        
        private void OnBuffAdded(Dictionary<string, object> messagedict)
        {
            VBuff buff = messagedict["Buff"] as VBuff;
            if (buff is null)
                return;

            VBuffUI buffUI = new VBuffUI(Instantiate<GameObject>(buffCellPrefab, transform), buff.IsPermanent, buff.GetBuffName());
            buffUI.SetText(buff.IsPermanent ? buff.Layer : buff.Duration);
            _buffUIs.Add(buff.Id, buffUI);
        }
    }
}