using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using static UnityEngine.UI.Image;

public class AgentLogic : MonoBehaviour
{
    public float range = 2;
    
    private NavMeshAgent _agent;
    private Transform _playerReference;
    private bool isChasing = true;

    [SerializeField] private Material[] materialStates = new Material[2];
    MeshRenderer _materialReference;

    //State machine

    private enum States
    {
        IDLE,
        PATROL,
        ALERTED,
        CHASE
    }

    private States NEXT_STATE = States.IDLE;
    private States CURRENT_STATE = States.IDLE;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _materialReference = this.GetComponent<MeshRenderer>();

        CURRENT_STATE = States.PATROL;
        NEXT_STATE = States.PATROL;

        //_sentinelLogicReference = GameObject.Find("Logic").GetComponent<FieldOfView>();
        _playerReference = GameObject.FindGameObjectWithTag("Player").transform;
        
    }

    private void Start()
    {
        _agent.SetDestination(transform.position);

    }

    private void Update()
    {
        switch (CURRENT_STATE)
        {
            case States.IDLE:
                SM_IDLE();
                break;
            case States.PATROL:
                SM_PATROL();
                break;
            case States.ALERTED:
                SM_ALERTED();
                break;
            case States.CHASE: 
                SM_CHASE();
                break;
        }

        Debug.Log(CURRENT_STATE);

    }

   

    private void LateUpdate()
    {
        if (CURRENT_STATE != NEXT_STATE) 
        {
            switch (NEXT_STATE)
            {
                case States.IDLE:
                    StateToIdle();
                    break;
                case States.PATROL:
                    StateToPatrol();
                    break;
                case States.ALERTED:
                    StateToAlerted();
                    break;
                case States.CHASE:
                    StateToChase();
                    break;
            }
        }

        
    }

    

    #region State Machine Transitions

    private void StateToIdle()
    {
        _materialReference.material = this.materialStates[0];
        CURRENT_STATE = States.IDLE;
    }
    private void StateToPatrol()
    {
        _materialReference.material = this.materialStates[1];
        CURRENT_STATE = States.PATROL;
    }

    private void StateToAlerted()
    {
        _materialReference.material = this.materialStates[2];
        CURRENT_STATE = States.ALERTED;
    }
    private void StateToChase()
    {
        _materialReference.material = this.materialStates[2];
        CURRENT_STATE = States.CHASE;
    }


    #endregion


    #region State Machines
    private void SM_IDLE()
    {
        Debug.Log("Idling...");

        StartCoroutine(SM_IDLE_UPDATE());
    }
    protected IEnumerator SM_IDLE_UPDATE()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 2f));
        if (CURRENT_STATE != States.ALERTED)
            NEXT_STATE = States.PATROL;
    }

    private void SM_PATROL()
    {
        if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    
                    Debug.Log("Patrolling...");
                    Vector3 point;
                    if (RandomPoint(transform.position, range, out point))
                    {
                        if (CURRENT_STATE == States.ALERTED)
                        {
                            StateToAlerted();
                            return;
                        }
                        else if (UnityEngine.Random.Range(0, 100) < 50)
                        {
                            NEXT_STATE = States.IDLE;
                            return;
                        }

                        Debug.DrawRay(point, Vector3.zero, UnityEngine.Color.blue, 1.0f);
                        _agent.SetDestination(point);
                        Debug.Log("New Destination");
                    }
                    return;
                }
                else return;
            }
        }
       
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;

    }

    private void SM_ALERTED()
    {
        if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    NEXT_STATE = States.IDLE;
                    return;
                }
                else return;
            }
        }
    }
    
    private void SM_CHASE()
    {
        _agent.SetDestination(_playerReference.position);

        if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    NEXT_STATE = States.IDLE;
                    return;
                }
                else return;
            }
        }

    }

    public void PlayerSpotted()
    {
        NEXT_STATE = States.CHASE;
    }

    #endregion


    #region Events
    //private void OnEnable()
    //{
    //    _sentinelLogicReference.OnPlayerSpotted += SentinelLogicReference_OnPlayerSpotted;
    //}


    //private void OnDisable()
    //{
    //    _sentinelLogicReference.OnPlayerSpotted -= SentinelLogicReference_OnPlayerSpotted;
    //}

    #endregion

   


}
