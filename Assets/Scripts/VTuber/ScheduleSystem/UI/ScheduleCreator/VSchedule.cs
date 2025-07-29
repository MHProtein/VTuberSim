//
// using System.Collections.Generic;
// using PrimeTween;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace VTuber.ScheduleSystem.UI
// {
//     public class VSchedule : VScheduleTable
//     {
//         public Vector2Int slotSize;
//         [SerializeField] protected GameObject itemPrefab;
//         [SerializeField] protected Transform indicator;
//         [SerializeField] protected Image indicatorLeft;
//         [SerializeField] protected Image indicatorRight;
//         
//         public VScheduleSlot[,] Slots => slots;
//         protected VScheduleSlot[,] slots;
//         protected List<EventData> items;
//     
//         protected override void Awake()
//         {
//             PrimeTweenConfig.warnEndValueEqualsCurrent = false;
//             slots = new VScheduleSlot[slotSize.y, slotSize.x];
//             var slotList = GetComponentsInChildren<VScheduleSlot>();
//             
//             int i = 0; 
//             for (int y = 0; y < slotSize.y; y++)
//             {
//                 for (int x = 0; x < slotSize.x; x++)
//                 {    
//                     slots[y, x] = slotList[i++];
//                     slots[y, x].Initialize(new Vector2Int(x, y), this);
//                 }
//             }
//         }
//
//         public void ResetSchedule()
//         {            
//             
//             for (int x = 0; x < slotSize.x; x++)
//             {
//                 for (int y = 0; y < slotSize.y; y++)
//                 {
//                     slots[y, x].ResetItem();
//                 }
//             }
//         }
//         
//         public void ChangeIndicatorPosition(Vector2 position)
//         {
//             Tween.Position(indicator, position, 0.2f);
//         }
//         
//         public void ChangeIndicatorScale(float scale)
//         {
//             Tween.ScaleY(indicator,scale, 0.2f);
//         }
//         
//         public void ChangeIndicatorColor(Color color)
//         {
//             indicatorLeft.color = color;
//             indicatorRight.color = color;
//         }
//     }
// }
//
//
