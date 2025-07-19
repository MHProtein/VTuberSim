using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Core;
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

        public VBuffUI(GameObject go, bool isPermanent, string buffName)
        {
            gameObject = go;
            text = go.GetComponentInChildren<TMP_Text>();
            this.isPermanent = isPermanent;
            this.buffName = buffName;
        }

        public void SetText(int value)
        {
            if (isPermanent)
                text.text = $"{buffName} Layer: {value}";
            else
                text.text = $"{buffName} Duration: {value}";
        }
    }

    public class VBuffGroupUI : VUIBehaviour
    {
        [SerializeField] private GameObject buffCellPrefab;

        private Dictionary<uint, VBuffUI> _buffUIs;
        private VAnimationQueue _animationQueue = new VAnimationQueue();

        protected override void Awake()
        {
            base.Awake();
            _buffUIs = new Dictionary<uint, VBuffUI>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            var ctr = VBattleRootEventCenter.Instance;
            ctr.RegisterListener(VBattleEventKey.OnBuffAdded, OnBuffAdded);
            ctr.RegisterListener(VBattleEventKey.OnBuffRemoved, OnBuffRemoved);
            ctr.RegisterListener(VBattleEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            var ctr = VBattleRootEventCenter.Instance;
            ctr.RemoveListener(VBattleEventKey.OnBuffAdded, OnBuffAdded);
            ctr.RemoveListener(VBattleEventKey.OnBuffRemoved, OnBuffRemoved);
            ctr.RemoveListener(VBattleEventKey.OnBuffValueUpdated, OnBuffValueUpdated);
        }

        private void OnBuffAdded(Dictionary<string, object> msg)
        {
            bool isFromCard   = msg["IsFromCard"]   as bool? ?? false;
            bool shouldTwice  = msg["ShouldPlayTwice"] as bool? ?? false;
            bool isPermanent  = (bool)msg["IsPermanent"];
            string buffName   = (string)msg["BuffName"];
            int    value      = (int)msg["Value"];
            uint   id         = (uint)msg["Id"];

            // instantiate
            var go = Instantiate(buffCellPrefab);
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.zero;

            var ui = new VBuffUI(go, isPermanent, buffName);
            ui.SetText(value);
            _buffUIs[id] = ui;

            // enqueue scale‑in then punch
            _animationQueue.Enqueue(AnimationType.ScaleIn, go.transform, () => RaiseEvents(isFromCard, shouldTwice));
            //_animationQueue.Enqueue(AnimationType.Punch, go.transform, () => RaiseEvents(isFromCard, shouldTwice));
        }

        private void OnBuffValueUpdated(Dictionary<string, object> msg)
        {
            uint id = (uint)msg["Id"];
            if (_buffUIs.TryGetValue(id, out var ui))
            {
                ui.SetText((int)msg["Value"]);
                // only punch on update
                _animationQueue.Enqueue(AnimationType.Punch, ui.gameObject.transform,
                    () => RaiseEvents( msg["IsFromCard"]   as bool? ?? false,
                        msg["ShouldPlayTwice"] as bool? ?? false));
            }
            else
            {
                // fallback
                RaiseEvents(false, false);
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

        private void RaiseEvents(bool isFromCard, bool shouldPlayTwice)
        {
            if (shouldPlayTwice)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnPlayTheSecondTime, new Dictionary<string, object>());
                return;
            }
            if (isFromCard)
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard, new Dictionary<string, object>());
            }
        }
    }
}
