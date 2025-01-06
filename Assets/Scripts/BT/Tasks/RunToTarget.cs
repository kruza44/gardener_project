using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace BehaviorDesigner.Runtime.Tasks.Custom
{
    public class RunToTarget : Action
    {
        [RequiredField] public SharedTransform target;
        [RequiredField] public SharedVector2 moveDirection;
        [RequiredField] public SharedFloat targetDistance;
        [RequiredField] public SharedFloat walkSpeed;
        [RequiredField] public SharedBool isWalking;    // AnimatorManager에게 알려주기 위함
        public float runMultiplier;
        public float stopDistance;
        public float nextWaypointDist = 1f;  // 다음 waypoint로 바꾸기 위한 최소 거리
        public float rePathRate = 0.1f; // path recalculate 시간
        private float lastRePath = float.NegativeInfinity;
        private MoveManager _move;
        private AnimatorManager _animManager;
        private Seeker _seeker;
        private Path path;
        private int currentWaypoint = 0;

        // 타이머 추가해야할 수도 있음 (일정 시간 쫓다보면 포기하기로)


        public override void OnAwake()
        {
            _move = GetComponent<MoveManager>();
            _animManager = GetComponent<AnimatorManager>();
            _seeker = GetComponent<Seeker>();
        }

        public override void OnStart()
        {
            isWalking.Value = true;
            _animManager._animator.speed = runMultiplier;

            _seeker.StartPath(this.transform.position, target.Value.position, OnPathComplete);
        }

        public override TaskStatus OnUpdate()
        {
            if(Time.time > lastRePath + rePathRate && _seeker.IsDone())
            {
                lastRePath = Time.time;

                // Recalculate path
                _seeker.StartPath(this.transform.position, target.Value.position, OnPathComplete);
            }

            if(path == null)
            {
                return TaskStatus.Running;
            }

            // 처음부터 거리가 가까울 수도 있으므로
            if(targetDistance.Value <= stopDistance)
            {
                return TaskStatus.Success;
            }


            // 여러 waypoint가 가까이 있어 겹쳐서 발견될 수 있으므로 loop 안에서 처리함
            float distToWaypoint;
            while (true)
            {
                distToWaypoint = Vector3.Distance(this.transform.position, path.vectorPath[currentWaypoint]);
                if(distToWaypoint < nextWaypointDist)
                {
                    if(currentWaypoint + 1 < path.vectorPath.Count)
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        break;
                    }
                } else { break; }
            }

            moveDirection.Value = (path.vectorPath[currentWaypoint] - this.transform.position).normalized;

            return TaskStatus.Running;
        }

        public override void OnFixedUpdate()
        {
            _move.AddMove(moveDirection.Value * (walkSpeed.Value * runMultiplier));
        }

        public override void OnEnd()
        {
            isWalking.Value = false;
            _animManager._animator.speed = 1f;
        }

        public void OnPathComplete(Path p)
        {
            // Debug.Log("Path calculated. Error?: " + p.error);

            p.Claim(this);

            if(!p.error)
            {
                // path Recalculate, waypoint 초기화
                if(path != null) { path.Release(this); }
                path = p;
                currentWaypoint = 0;
            } else { p.Release(this); }
        }
    }
}
