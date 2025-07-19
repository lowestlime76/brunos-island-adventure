using UnityEngine;

namespace RPG.Character
{
    public class AIReturnState : AIBaseState
    {
        Vector3 targetPosition;
        public override void EnterState(EnemyController enemy)
        {
            enemy.movementCmp.UpdateAgentSpeed(enemy.stats.walkSpeed, true);

            if (enemy.patrolCmp != null)
            {
                targetPosition = enemy.patrolCmp.GetNextPosition();

                enemy.movementCmp.MoveAgentByDestination(targetPosition);
            }
            else
            {
                enemy.movementCmp.MoveAgentByDestination(
                    enemy.orginalPosition
            );
            }
        }

        public override void UpdateState(EnemyController enemy)
        {
            if (enemy.distanceFromPlayer < enemy.chaseRange)
            {
                enemy.SwitchState(enemy.chaseState);
                return;
            }

            if (enemy.movementCmp.ReachedDestination())
            {
                if (enemy.patrolCmp != null)
                {
                    enemy.SwitchState(enemy.patrolState);
                    return;
                }
                else
                {
                    enemy.movementCmp.isMoving = false;
                    enemy.movementCmp.Rotate(enemy.movementCmp.orginalForwardVector);
                }
            }
            else
            {
                if (enemy.patrolCmp != null)
                {
                    Vector3 newForwardVector = targetPosition - enemy.transform.position;
                    newForwardVector.y = 0;

                    enemy.movementCmp.Rotate(newForwardVector);
                }
                else
                {
                    Vector3 newForwardVector = enemy.orginalPosition -
                        enemy.transform.position;
                    newForwardVector.y = 0;

                    enemy.movementCmp.Rotate(newForwardVector);
                }
            }
        }

    }
}
