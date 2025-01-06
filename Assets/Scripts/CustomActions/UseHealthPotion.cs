using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Action")]
    public class UseHealthPotion : FsmStateAction
    {
        public FsmBool potionTiming;
        private HealthPotionManager _potion;

        public override void Awake()
        {
            _potion = Owner.GetComponent<HealthPotionManager>();
        }

        public override void OnUpdate()
        {
            if(potionTiming.Value)
            {
                potionTiming.Value = false;
                _potion.UseHealthPotion();
            }
        }

        public override void OnExit()
        {
            potionTiming.Value = false;
        }
    }
}