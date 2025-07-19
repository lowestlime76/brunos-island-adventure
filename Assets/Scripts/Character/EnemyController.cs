using System;
using UnityEngine;
using RPG.Utility;


namespace RPG.Character
{
    public class EnemyController : MonoBehaviour
    {

        public float chaseRange = 2.5f;
        public float attackRange = .75f;
        [NonSerialized] public float distanceFromPlayer;
        [NonSerialized] public Vector3 orginalPosition;
        [NonSerialized] public Movement movementCmp;
        [NonSerialized] public GameObject player;
        [NonSerialized] public Patrol patrolCmp;

        private Health healthCmp;
        [NonSerialized] public Combat combatCmp;

        public CharacterStatsSO stats;

        private AIBaseState currentState;
        public AIReturnState returnState = new AIReturnState();
        public AIChaseState chaseState = new AIChaseState();
        public AIAttackState attackState = new AIAttackState();
        public AIPatrolState patrolState = new AIPatrolState();
        public AIDefeatedState defeatedState = new AIDefeatedState();

        private void Awake()
        {
            if (stats == null)
            {
                Debug.LogWarning($"{name} does not have stats.");
            }
            currentState = returnState;

            player = GameObject.FindWithTag(Constants.PLAYER_TAG);
            movementCmp = GetComponent<Movement>();
            patrolCmp = GetComponent<Patrol>();
            healthCmp = GetComponent<Health>();
            combatCmp = GetComponent<Combat>();

            orginalPosition = transform.position;
        }

        private void Start()
        {
            currentState.EnterState(this);

            healthCmp.healthPoints = stats.health;
            combatCmp.damage = stats.damage;

            if (healthCmp.sliderCmp != null)
            {
                healthCmp.sliderCmp.maxValue = stats.health;
                healthCmp.sliderCmp.value = stats.health;
            }
        }

        private void OnEnable()
        {
            healthCmp.OnStartDefeated += HandleStartDefeated;
        }

        private void OnDisable()
        {
            healthCmp.OnStartDefeated -= HandleStartDefeated;
        }

        private void Update()
        {
            CalculateDistanceFromPlayer();

            currentState.UpdateState(this);
        }

        public void SwitchState(AIBaseState newState)
        {
            currentState = newState;
            currentState.EnterState(this);
        }

        private void CalculateDistanceFromPlayer()
        {
            if (player == null) return;

            Vector3 enemyPosition = transform.position;
            Vector3 playerPosition = player.transform.position;

            distanceFromPlayer = Vector3.Distance(
                enemyPosition, playerPosition
            );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(
                transform.position,
                chaseRange
            );
        }

        private void HandleStartDefeated()
        {
            SwitchState(defeatedState);
            currentState.EnterState(this);
        }
    }
}
