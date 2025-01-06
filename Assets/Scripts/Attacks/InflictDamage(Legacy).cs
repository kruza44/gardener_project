// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public class InflictDamage : MonoBehaviour
// {
//     // root에서 Unit을 가져와서 데미지 계산?
//     public DamageInfo damageInfo;
//     private Guid g;
//     private float activatedTime = 0;

//     void OnEnable()
//     {
//         activatedTime = 0f;
//         g = Guid.NewGuid(); // 공격판정 켜질 때마다 guid 새로 할당

//         Debug.Log("Hitbox Enabled Guid renewed");
//     }

//     void Update()
//     {
//         activatedTime += Time.deltaTime;
//     }

//     void OnTriggerStay2D(Collider2D other)
//     {
//         Unit hitUnit = other.transform.root.GetComponent<Unit>();
//         if (hitUnit != null)
//         {
//             // Check for Team ID

//             float h;    // 이전에 피격당했을 때의 activatedTime
//             if(!hitUnit.hitboxList.TryGetValue(g, out h))   // 피격대상에 Guid가 없다면
//             {
//                 SetPushSightDir(hitUnit.transform.position);
//                 hitUnit.GetHit(damageInfo);
//                 // 가해자 스탯에 따른 데미지 계산은 어떻게...?

//                 hitUnit.hitboxList.Add(g, activatedTime);
//             } else
//             {
//                 if((activatedTime - h) > damageInfo.hitboxRenewTime)   // 피격대상에 Guid가 있지만 피격판정이 갱신되어야 한다면
//                 {
//                     // Debug.Log("RenewTime");
//                     SetPushSightDir(hitUnit.transform.position);
//                     hitUnit.GetHit(damageInfo);
//                     // 가해자 스탯에 따른 데미지 계산은 어떻게...?

//                     hitUnit.hitboxList[g] = activatedTime; // Dictionary 한 번만 찾도록 최적화 필요...?
//                 } else
//                 {
//                     // Debug.Log("NotRenewTime, h: " + h);
//                 }
//             }

//             // Dictionary가 가득 차면 어떻게 해야?
//         }
//     }

//     protected virtual void SetPushSightDir(Vector3 hitUnitLocation) { }
// }
