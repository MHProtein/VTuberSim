// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Serialization;
// using VTuber.BattleSystem.Card;
// using VTuber.Character;
// using VTuber.Core.Foundation;
// using VTuber.Core.StateMachine;
// using VTuber.Dialogue.UI;
// using VTuber.ScheduleSystem.Core;
// using VTuber.ScheduleSystem.Schedule;
// using VTuber.ScheduleSystem.UI;
// using Yarn.Unity;
//
// namespace VTuber.Dialogue
// {
//     public class DialogueManagerTMP : VMonoBehaviour
//     {
//
//         private VCharacter _character;
//         private VStateMachine _stateMachine;
//         
//         protected override void Awake()
//         {
//             base.Awake();
//             VDataLoader loader = new VDataLoader(@"Assets\Resources\Configurations\NewCards.xlsx");
//         }
//
//         protected override void OnEnable()
//         {
//             base.OnEnable();
//             _stateMachine.OnEnable();
//         }
//         
//         protected override void OnDisable()
//         {
//             base.OnDisable();
//             _stateMachine.OnDisable();
//         }
//
//         protected override void Start()
//         {
//             base.Start();
//             _stateMachine.SwitchState(VStateType.ScheduleCreation);
//         }
//         
//         public void ModifySchedule()
//         {
//             _stateMachine.SwitchState(VStateType.ScheduleModify);
//         }
//
//         public void PauseSchedule()
//         {
//             _stateMachine.PauseSchedule();
//         }
//
//         public void ContinueSchedule()
//         {
//             _stateMachine.ContinueSchedule();
//         }
//         
//     }
// }

