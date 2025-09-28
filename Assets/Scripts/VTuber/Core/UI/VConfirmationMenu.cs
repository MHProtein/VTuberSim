using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.UI
{
    public class VConfirmationMenu : VUIBehaviour
    {
        [SerializeField] private GameObject window;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private TMP_Text content;
        [SerializeField] private Transform detailParent;
        [SerializeField] private GameObject detailPrefab;
        
        private List<GameObject> _details = new List<GameObject>();
        private Action _confirmAction;
        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(Confirm);
            returnButton.onClick.AddListener(Hide);
        }

        private void Confirm()
        {
            Hide();
            _confirmAction?.Invoke();
        }

        public void Show(string contentStr, List<string> detailStrs, Action confirmAction)
        {
            window.SetActive(true);
            content.text = contentStr;
            foreach (var detailStr in detailStrs)
            {
                var detail = Instantiate(detailPrefab, detailParent);
                detail.GetComponent<TMP_Text>().text = detailStr;
                _details.Add(detail);
            }
            _confirmAction = confirmAction;
        }

        public void Hide()
        {
            window.SetActive(false);
            foreach (var detail in _details)
            {
                Destroy(detail.gameObject);
            }
            _details.Clear();
        }
    }
}