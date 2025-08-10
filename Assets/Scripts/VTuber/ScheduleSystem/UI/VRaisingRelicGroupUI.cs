using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    class VRaisingRelicUI
    {
        public TMP_Text text;
        public GameObject gameObject;
        public bool isPermanent;
        public string relicName;

        public VRaisingRelicUI(GameObject go, bool isPermanent, string relicName)
        {
            gameObject = go;
            text = go.GetComponentInChildren<TMP_Text>();
            this.isPermanent = isPermanent;
            this.relicName = relicName;
        }

        public void SetText(int value)
        {
            if (isPermanent)
                text.text = $"{relicName}";
            else
                text.text = $"{relicName} 层: {value}";
        }
    }

    public class VRaisingRelicGroupUI : VUIBehaviour
    {
        [SerializeField] private GameObject buffCellPrefab;

        private Dictionary<uint, VRaisingRelicUI> _buffUIs;
        private readonly VAnimationQueue _animationQueue = new VAnimationQueue();

        protected override void Awake()
        {
            base.Awake();
            _buffUIs = new Dictionary<uint, VRaisingRelicUI>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRelicAdded, OnBuffAdded);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRelicRemoved, OnBuffRemoved);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnRelicValueChanged, OnBuffValueUpdated);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRelicAdded, OnBuffAdded);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRelicRemoved, OnBuffRemoved);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnRelicValueChanged, OnBuffValueUpdated);
        }
        
        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            foreach (var ui in _buffUIs)
            {
                Destroy(ui.Value.gameObject);
            }
            _buffUIs.Clear();
        }
        
        private void OnBuffAdded(Dictionary<string, object> msg)
        {
            uint id = (uint)msg["Id"];
            
            if( _buffUIs.ContainsKey(id))
            {
                OnBuffValueUpdated(msg);
                return;
            }
            
            bool isPermanent = (bool)msg["IsPermanent"];
            string buffName = (string)msg["RelicName"];
            int value = (int)msg["Value"];
            
            // instantiate
            var go = Instantiate(buffCellPrefab, transform);
            var ui = new VRaisingRelicUI(go, isPermanent, buffName);
            _buffUIs[id] = ui;
            ui.SetText(value);

            // enqueue scale‑in then punch
            _animationQueue.Enqueue(Tween.Scale(ui.gameObject.transform, Vector3.one, 0.4f));
        }

        private void OnBuffValueUpdated(Dictionary<string, object> msg)
        {
            uint id = (uint)msg["Id"];
            if (_buffUIs.TryGetValue(id, out var ui))
            {
                ui.SetText((int)msg["Value"]);
                // only punch on update

                _animationQueue.Enqueue(Tween.PunchScale(ui.gameObject.transform, Vector3.one * 1.3f, 0.4f));
            }
        }

        private void OnBuffRemoved(Dictionary<string, object> msg)
        {
            uint id = (uint)msg["Id"];
            if (_buffUIs.TryGetValue(id, out var ui))
            {
                Destroy(ui.gameObject);
                _buffUIs.Remove(id);
            }
        }
    }
}
