using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.UI
{
    public class VConfirmationMenu : VUIBehaviour
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private TMP_Text content;
        [SerializeField] private Transform detailParent;
        [SerializeField] private GameObject detailPrefab;
        
        private List<GameObject> _details = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();
        }

        public void Show(string contentStr, List<string> detailStrs)
        {
            content.text = contentStr;
            foreach (var detailStr in detailStrs)
            {
                var detail = Instantiate(detailPrefab, detailParent);
                detail.GetComponent<TMP_Text>().text = detailStr;
                _details.Add(detail);
            }
        }

        public void Hide()
        {
            foreach (var detail in _details)
            {
                Destroy(detail.gameObject);
            }
            _details.Clear();
        }
    }
}