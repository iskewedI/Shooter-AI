using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerAgent : Agent
{
    PlayerLook playerLook;
    PlayerWeapon playerWeapon;

    [SerializeField] private Transform shootingPoint;

    public int minStepsBetweenShots = 50;

    private Camera agentCam;
    private bool ShotAvaliable = true;
    private int StepsUntilShotIsAvaliable = 0;

    // Reward if hit the target.
    private readonly float OnTargetHitReward = +1f;
    // Reward if looked at the target.
    private readonly float OnTargetLookReward = +0.01f;
    // Small punishment if not looked at the target.
    private readonly float OnTargetNoLookAtPlayerReward = -0.01f;
    // Punishment when totally missed.
    private readonly float OnMissReward = -0.033f;
    // Bigger punishment if a target despawn.
    private readonly float OnTargetDespawn = -0.01f;
    // Punishment for every step to ensure it doesn't stop shooting.
    private readonly float OnStepReward = -0.0001f;

    private new void OnEnable()
    {
        base.OnEnable();
        SpawnAreaController.OnTargetDespawned += HandleTargetDespawn;
    }

    private void Start()
    {
        playerLook = GetComponent<PlayerLook>();
        agentCam = playerLook.cam;

        playerWeapon = GetComponent<PlayerWeapon>();
    }

    private void FixedUpdate()
    {
        if (!ShotAvaliable)
        {
            StepsUntilShotIsAvaliable--;

            if (StepsUntilShotIsAvaliable <= 0)
                ShotAvaliable = true;
        }

        AddReward(OnStepReward);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxis("Mouse X");
        continuousActions[1] = Input.GetAxis("Mouse Y");

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKeyDown(KeyCode.W) ? 1 : 0;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ShotAvaliable);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float lookX = actions.ContinuousActions[0];
        float lookY = actions.ContinuousActions[1];

        int targetLayerMask = 1 << LayerMask.NameToLayer("Target");

        Vector3 direction = agentCam.transform.forward * playerLook.maxLookDistance;

        Vector2 lookTo = new Vector2(lookX, lookY);

        playerLook.ProcessLook(lookTo);

        RaycastHit hit;
        bool targetLookHit = Physics.Raycast(shootingPoint.position, direction, out hit, playerLook.maxLookDistance, targetLayerMask);
        if (targetLookHit)
        {
            AddReward(OnTargetLookReward);
        }
        else
        {
            AddReward(OnTargetNoLookAtPlayerReward);
        }

        // 1 -> Fire bullet. 0 -> Not fire bullet.
        bool shouldFire = actions.DiscreteActions[0] == 1;

        if (shouldFire)
        {
            if (!ShotAvaliable)
                return;

            Debug.DrawRay(shootingPoint.position, direction, Color.red, 1f);

            if (targetLookHit)
            {
                HandleHit();
                hit.transform.GetComponent<TargetController>().TakeDamage(playerWeapon.damage);
            }
            else
            {
                HandleMiss();
            }

            ShotAvaliable = false;
            StepsUntilShotIsAvaliable = minStepsBetweenShots;
        } else
        {
            // Debug where it's seeing
            Debug.DrawRay(shootingPoint.position, direction, Color.green, 1f);
        }
    }

    private void HandleHit()
    {
        Debug.Log("Target hit");
        AddReward(OnTargetHitReward);
    }


    public void HandleMiss()
    {
        Debug.Log("Miss hit");
        AddReward(OnMissReward);
    }

    public void HandleTargetDespawn()
    {
        Debug.Log("Despawned");
        AddReward(OnTargetDespawn);
    }

    private new void OnDisable()
    {
        base.OnDisable();

        SpawnAreaController.OnTargetDespawned -= HandleTargetDespawn;
    }
}
