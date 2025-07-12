using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
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

        private VBuffUI _buffUIToSetParent;
        
        private bool _isFromCard = false;
        private bool _shouldPlayTwice = false;

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
                _isFromCard = messagedict["IsFromCard"] as bool? ?? false;
                _shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;
                
                buffUI.SetText((int)messagedict["Value"]);
                Tween.PunchScale(buffUI.gameObject.transform, Vector3.one * 1.3f, 0.5f).OnComplete(OnMovedToSlot);
                return;
            }
            OnMovedToSlot();
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
            _isFromCard = messagedict["IsFromCard"] as bool? ?? false;
            _shouldPlayTwice = messagedict["ShouldPlayTwice"] as bool? ?? false;

            VBuffUI buffUI = new VBuffUI(Instantiate<GameObject>(buffCellPrefab), (bool)messagedict["IsPermanent"], (string)messagedict["BuffName"]);
            buffUI.gameObject.transform.SetParent(transform);
            buffUI.gameObject.transform.localScale = Vector3.zero;
            buffUI.SetText((int)messagedict["Value"]);
            _buffUIs.Add((int)messagedict["Id"], buffUI);

            Tween.Scale(buffUI.gameObject.transform, Vector3.one, 0.5f).OnComplete(OnMovedToSlot);
            _buffUIToSetParent = buffUI;
        }

        private void OnMovedToSlot()
        {
            if (_shouldPlayTwice)
            {
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnPlayTheSecondTime, new Dictionary<string ,object>()
                {
                    
                });
                _shouldPlayTwice = false;
                return;
            }

            if (_isFromCard)
            {
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnNotifyBeginDisposeCard, new Dictionary<string ,object>()
                {
                
                });
                _isFromCard = false;
            }
        }
    }
}