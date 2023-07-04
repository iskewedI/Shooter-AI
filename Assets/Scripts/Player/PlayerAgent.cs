using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class PlayerAgent : Agent
{
    PlayerLook playerLook;
    PlayerWeapon playerWeapon;

    // Bigger reward if hit the target really fast
    private readonly float OnFastTargetHitReward = +3f;
    // Reward if hit the target.
    private readonly float OnTargetHitReward = +1f;
    // Reward if looked at the target.
    private readonly float OnTargetLookReward = +0.1f;
    // Small punishment if not looked at the target.
    private readonly float OnTargetNoLookAtPlayerReward = -0.01f;
    // Small punishment when missed but at least it hit the target wall spawn point.
    //private readonly float OnMissButWallHitReward = +0.007f;
    // Punishment when floor hit.
    private readonly float OnFloorHit = -100f;
    // Punishment when totally missed.
    private readonly float OnMissReward = -0.001f;
    // Bigger punishment if a target despawn.
    private readonly float OnTargetDespawn= -0.4f;
    // Punishment every time it shots to encourage it to throw shots with more accuracy.
    //private readonly float OnShotReward = -0.005f;
    // Punishment for every step to ensure it doesn't stop shooting.
    private readonly float OnStepReward = -0.0001f;


    private new void OnEnable()
    {
        base.OnEnable();
        SpawnAreaController.OnTargetDespawned += HandleTargetDespawn;

        BulletController.OnBulletHit += HandleHit;
        BulletController.OnBulletMiss += HandleMiss;
    }

    private void Start()
    {
        playerLook = GetComponent<PlayerLook>();
        playerWeapon = GetComponent<PlayerWeapon>();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxis("Mouse X");
        continuousActions[1] = Input.GetAxis("Mouse Y");

        //ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        //discreteActions[0] = Input.GetKeyDown(KeyCode.W) ? 1 : 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 1 -> Fire bullet. 0 -> Not fire bullet.
        //bool shouldFire = actions.DiscreteActions[0] == 1;

        float lookX = actions.ContinuousActions[0];
        float lookY = actions.ContinuousActions[1];

        Vector2 lookTo = new Vector2(lookX, lookY);

        Collider objectHit = playerLook.ProcessLook(lookTo);
        if (objectHit != null)
        {
            if (objectHit.CompareTag("Target"))
            {
                // Has hit the target
                Debug.Log("Target looked");
                AddReward(OnTargetLookReward);
            } else
            {
                // Hit anything else
                AddReward(OnTargetNoLookAtPlayerReward);
            }
        } else
        {
            AddReward(OnTargetNoLookAtPlayerReward);
        }

        //if (shouldFire)
        //{
        //    Debug.Log("Fire");
        //    playerWeapon.FireBullet();

        //    //AddReward(OnShotReward);
        //} else
        //{
        //    AddReward(OnStepReward);
        //}

        AddReward(OnStepReward);
    }

    public void HandleHit(float timeTaken)
    {
        if (timeTaken < 0.9)
        {
            Debug.Log("Target hit fast");
            AddReward(OnFastTargetHitReward);

        }
        else
        {
            Debug.Log("Target hit slow");
            AddReward(OnTargetHitReward);
        }
        EndEpisode();
    }

    public void HandleMiss(bool floorHit)
    {
        if (floorHit)
        {
            SetReward(OnFloorHit);
            EndEpisode();
        } else
        {
            Debug.Log("Miss hit");
            AddReward(OnMissReward);

        }
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
        BulletController.OnBulletHit -= HandleHit;
        BulletController.OnBulletMiss -= HandleMiss;
    }
}
