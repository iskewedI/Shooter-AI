using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerAgent : Agent
{
    PlayerLook playerLook;
    PlayerWeapon playerWeapon;

    [SerializeField] private Transform shootingPoint;

    private bool ShotAvaliable = true;
    private int StepsUntilShotIsAvaliable = 0;
    public int minStepsBetweenShots = 50;

    // Bigger reward if hit the target really fast
    private readonly float OnFastTargetHitReward = +2f;
    // Reward if hit the target.
    private readonly float OnTargetHitReward = +1f;
    // Reward if looked at the target.
    private readonly float OnTargetLookReward = +0.01f;
    // Small punishment if not looked at the target.
    private readonly float OnTargetNoLookAtPlayerReward = -0.01f;
    // Small punishment when missed but at least it hit the target wall spawn point.
    //private readonly float OnMissButWallHitReward = +0.007f;
    // Punishment when floor hit.
    //private readonly float OnFloorHit = -100f;
    // Punishment when totally missed.
    private readonly float OnMissReward = -0.033f;
    // Bigger punishment if a target despawn.
    private readonly float OnTargetDespawn = -0.01f;
    // Punishment every time it shots to encourage it to throw shots with more accuracy.
    //private readonly float OnShotReward = -0.01f;
    // Punishment for every step to ensure it doesn't stop shooting.
    private readonly float OnStepReward = -0.0001f;

    private new void OnEnable()
    {
        base.OnEnable();
        SpawnAreaController.OnTargetDespawned += HandleTargetDespawn;

        //BulletController.OnBulletHit += HandleHit;
        BulletController.OnBulletMiss += HandleMiss;
    }

    private void Start()
    {
        playerLook = GetComponent<PlayerLook>();
        playerWeapon = GetComponent<PlayerWeapon>();
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * playerLook.maxLookDistance, Color.green, 1f);

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
        Vector3 direction = transform.forward;

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

            Debug.DrawRay(transform.position, direction * playerLook.maxLookDistance, Color.red, 1f);

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

    //public void HandleHit(float timeTaken)
    //{
    //    if (timeTaken < 0.9)
    //    {
    //        Debug.Log("Target hit fast");
    //        AddReward(OnFastTargetHitReward);

    //    }
    //    else
    //    {
    //        Debug.Log("Target hit slow");
    //        AddReward(OnTargetHitReward);
    //    }
    //}

    public void HandleTargetDespawn()
    {
        Debug.Log("Despawned");
        AddReward(OnTargetDespawn);
    }

    private new void OnDisable()
    {
        base.OnDisable();

        SpawnAreaController.OnTargetDespawned -= HandleTargetDespawn;
        //BulletController.OnBulletHit -= HandleHit;
        BulletController.OnBulletMiss -= HandleMiss;
    }
}
