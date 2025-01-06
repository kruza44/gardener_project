using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class ShakeCameraTask : Action
    {
        public SharedFloat shakeDuration;

        public override void OnStart()
        {
            CameraManager.Instance.ShakeCamera(shakeDuration.Value);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
