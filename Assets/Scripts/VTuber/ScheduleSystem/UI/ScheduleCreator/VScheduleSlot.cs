// using System.Collections.Generic;
// using UnityEngine;
// using VTuber.Core.Foundation;
//
//
// namespace VTuber.ScheduleSystem.UI
// {
//     public class VScheduleSlot : VUIBehaviour
//     {
//         public VEventUI Item => _item;
//         public Vector2Int Coordination => _coordination;
//         private VSchedule _schedule;
//         private Vector2Int _coordination;
//         private VEventUI _item;
//
//         public void Initialize(Vector2Int coordination, VSchedule schedule)
//         {
//             _coordination = coordination;
//             _schedule = schedule;
//         }
//
//         public void SetItem(VEventUI item)
//         {
//             _item = item;
//         }
//
//         public void RemoveItem()
//         {
//             _item = null;
//         }
//
//         public void ResetItem()
//         {
//             if (_item is null)
//                 return;
//             
//             _item.Despawn();
//         }
//
//         public void SetIndicator(int height, float offsetY)
//         {
//             if (height == 1)
//             {
//                 _schedule.ChangeIndicatorScale(1.0f);
//                 _schedule.ChangeIndicatorPosition(transform.position);
//                 if (_item is not null)
//                 {
//                     _schedule.ChangeIndicatorColor(Color.red);
//                     return;
//                 }
//
//                 _schedule.ChangeIndicatorColor(Color.black);
//                 return;
//             }
//
//             if (height == 2)
//             {
//                 _schedule.ChangeIndicatorScale(2.0f);
//                 if (_coordination.y == 0)
//                 {
//                     var position = (_schedule.Slots[0, _coordination.x].transform.position +
//                                     _schedule.Slots[1, _coordination.x].transform.position) / 2f;
//                     _schedule.ChangeIndicatorPosition(position);
//                     if (_schedule.Slots[1, _coordination.x].Item is null
//                         && _item is null)
//                     {
//                         _schedule.ChangeIndicatorColor(Color.black);
//                         return;
//                     }
//
//                     _schedule.ChangeIndicatorColor(Color.red);
//                     return;
//                 }
//
//                 if (_coordination.y == 1)
//                 {
//                     if (offsetY > 0.0f)
//                     {
//                         if (_schedule.Slots[0, _coordination.x].Item is null && _item is null)
//                         {
//                             var position = (_schedule.Slots[0, _coordination.x].transform.position +
//                                             _schedule.Slots[1, _coordination.x].transform.position) / 2f;
//                             _schedule.ChangeIndicatorColor(Color.black);
//                             _schedule.ChangeIndicatorPosition(position);
//                             return;
//                         }
//                         if (_schedule.Slots[2, _coordination.x].Item is null && _item is null)
//                         {
//                             var position = (_schedule.Slots[1, _coordination.x].transform.position +
//                                             _schedule.Slots[2, _coordination.x].transform.position) / 2f;
//                             _schedule.ChangeIndicatorColor(Color.black);
//                             _schedule.ChangeIndicatorPosition(position);
//                             return;
//                         }
//                     }
//                     else
//                     {
//                         if (_schedule.Slots[2, _coordination.x].Item is null && _item is null)
//                         {
//                             var position = (_schedule.Slots[1, _coordination.x].transform.position +
//                                             _schedule.Slots[2, _coordination.x].transform.position) / 2f;
//                             _schedule.ChangeIndicatorColor(Color.black);
//                             _schedule.ChangeIndicatorPosition(position);
//                             return;
//                         }
//
//                         if (_schedule.Slots[0, _coordination.x].Item is null && _item is null)
//                         {
//                             var position = (_schedule.Slots[0, _coordination.x].transform.position +
//                                             _schedule.Slots[1, _coordination.x].transform.position) / 2f;
//                             _schedule.ChangeIndicatorColor(Color.black);
//                             _schedule.ChangeIndicatorPosition(position);
//                             return;
//                         }
//                     }
//
//                     _schedule.ChangeIndicatorColor(Color.red);
//                     _schedule.ChangeIndicatorPosition(transform.position);
//                     _schedule.ChangeIndicatorScale(2.0f);
//                 }
//
//                 if (_coordination.y == 2)
//                 {
//                     var position = (_schedule.Slots[1, _coordination.x].transform.position +
//                                     _schedule.Slots[2, _coordination.x].transform.position) / 2f;
//                     _schedule.ChangeIndicatorPosition(position);
//                     if (_schedule.Slots[1, _coordination.x].Item is null && _item is null)
//                     {
//                         _schedule.ChangeIndicatorColor(Color.black);
//                         return;
//                     }
//
//                     _schedule.ChangeIndicatorColor(Color.red);
//                     return;
//                 }
//             }
//
//             if (height == 3)
//             {
//                 _schedule.ChangeIndicatorScale(3.0f);
//                 var position = _schedule.Slots[1, _coordination.x].transform.position;
//                 _schedule.ChangeIndicatorPosition(position);
//                 for (int y = 0; y < 3; y++)
//                 {
//                     if (_schedule.Slots[y, _coordination.x]._item is not null)
//                     {
//                         _schedule.ChangeIndicatorColor(Color.red);
//                         return;
//                     }
//                 }
//
//                 _schedule.ChangeIndicatorColor(Color.black);
//             }
//         }
//
//
//         public bool FindPosition(int height, float offsetY, out List<VScheduleSlot> parents,
//             out Transform transformParent, out Vector3 position)
//         {
//             parents = null;
//             transformParent = null;
//             position = Vector3.zero;
//
//             if (_item is not null)
//             {
//                 return false;
//             }
//
//             if (height == 1)
//             {
//                 parents = new List<VScheduleSlot>()
//                 {
//                     this
//                 };
//                 transformParent = transform;
//                 position = transform.position;
//                 return true;
//             }
//
//             if (height == 2)
//             {
//                 if (_coordination.y == 0)
//                 {
//                     if (_schedule.Slots[1, _coordination.x].Item is null)
//                     {
//                         parents = new List<VScheduleSlot>()
//                         {
//                             _schedule.Slots[0, _coordination.x],
//                             _schedule.Slots[1, _coordination.x]
//                         };
//                         transformParent = _schedule.Slots[1, _coordination.x].transform;
//                         position = (_schedule.Slots[0, _coordination.x].transform.position +
//                                     _schedule.Slots[1, _coordination.x].transform.position) / 2f;
//                         return true;
//                     }
//
//                     return false;
//                 }
//
//                 if (_coordination.y == 1)
//                 {
//                     if (offsetY > 0.0f)
//                     {
//                         if (_schedule.Slots[0, _coordination.x].Item is null)
//                         {
//                             parents = new List<VScheduleSlot>()
//                             {
//                                 _schedule.Slots[0, _coordination.x],
//                                 _schedule.Slots[1, _coordination.x]
//                             };
//                             transformParent = _schedule.Slots[1, _coordination.x].transform;
//                             position = (_schedule.Slots[0, _coordination.x].transform.position +
//                                         _schedule.Slots[1, _coordination.x].transform.position) / 2f;
//                             return true;
//                         }
//
//                         if (_schedule.Slots[2, _coordination.x].Item is null)
//                         {
//                             parents = new List<VScheduleSlot>()
//                             {
//                                 _schedule.Slots[1, _coordination.x],
//                                 _schedule.Slots[2, _coordination.x]
//                             };
//                             transformParent = _schedule.Slots[2, _coordination.x].transform;
//                             position = (_schedule.Slots[1, _coordination.x].transform.position +
//                                         _schedule.Slots[2, _coordination.x].transform.position) / 2f;
//                             return true;
//                         }
//                     }
//                     else
//                     {
//                         if (_schedule.Slots[2, _coordination.x].Item is null)
//                         {
//                             parents = new List<VScheduleSlot>()
//                             {
//                                 _schedule.Slots[1, _coordination.x],
//                                 _schedule.Slots[2, _coordination.x]
//                             };
//                             transformParent = _schedule.Slots[2, _coordination.x].transform;
//                             position = (_schedule.Slots[1, _coordination.x].transform.position +
//                                         _schedule.Slots[2, _coordination.x].transform.position) / 2f;
//                             return true;
//                         }
//                         if (_schedule.Slots[0, _coordination.x].Item is null)
//                         {
//                             parents = new List<VScheduleSlot>()
//                             {
//                                 _schedule.Slots[0, _coordination.x],
//                                 _schedule.Slots[1, _coordination.x]
//                             };
//                             transformParent = _schedule.Slots[1, _coordination.x].transform;
//                             position = (_schedule.Slots[0, _coordination.x].transform.position +
//                                         _schedule.Slots[1, _coordination.x].transform.position) / 2f;
//                             return true;
//                         }
//                     }
//                 }
//
//                 if (_coordination.y == 2)
//                 {
//                     if (_schedule.Slots[1, _coordination.x].Item is null)
//                     {
//                         parents = new List<VScheduleSlot>()
//                         {
//                             _schedule.Slots[1, _coordination.x],
//                             _schedule.Slots[2, _coordination.x]
//                         };
//                         transformParent = _schedule.Slots[2, _coordination.x].transform;
//                         position = (_schedule.Slots[1, _coordination.x].transform.position +
//                                     _schedule.Slots[2, _coordination.x].transform.position) / 2f;
//                         return true;
//                     }
//                 }
//
//                 return false;
//             }
//
//             if (height == 3)
//             {
//                 for (int y = 0; y < 3; y++)
//                 {
//                     if (_schedule.Slots[y, _coordination.x]._item is not null)
//                     {
//                         return false;
//                     }
//                 }
//
//                 parents = new List<VScheduleSlot>()
//                 {
//                     _schedule.Slots[0, _coordination.x],
//                     _schedule.Slots[1, _coordination.x],
//                     _schedule.Slots[2, _coordination.x]
//                 };
//                 transformParent = _schedule.Slots[2, _coordination.x].transform;
//                 position = _schedule.Slots[1, _coordination.x].transform.position;
//                 return true;
//             }
//
//             return false;
//         }
//     }
// }
//
