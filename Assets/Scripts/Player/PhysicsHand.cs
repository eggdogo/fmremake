using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using easyInputs;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PhysicsHand : MonoBehaviour
{
    [Header("PID")]

    [SerializeField] Rigidbody playerRigidbody;

    [SerializeField] float frequency = 50f;
    [SerializeField] float damping = 1f;

    [SerializeField] float rotfrequency = 100f;
    [SerializeField] float rotdamping = 0.9f;
    [SerializeField] Transform target;

    [Space]
    [Header("Springs")]
    [SerializeField] float climbForce = 1000f;
    [SerializeField] float climbDrag = 500f;

    [Space]
    [Header("Other Stuff")]
    public bool IsLocomotion;

    public XRNode node;
    XRController controller;
    Vector3 _previousPosition;
    Rigidbody _rigidbody;
    public bool _isColliding;
    float triggerValue;
    float gripValue;

    [Header("CLIMBING")]
    public string ClimbTag;
    public bool IsClimbing;
    public PhysicsHand otherHand;
    public float GripValue, TriggerValue;
    Rigidbody climbingon;
    FixedJoint Hinge;
    public ParticleSystem Grabed;
    public AudioSource GrabSFX;
    private bool ClimbBool = false;
    public float ClimbHandDistanceLimit = 5;
    private bool isHandMovementEnabled = false;
    private FixedJoint movementJoint;

    void Start()
    {
        transform.position = target.position;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.maxAngularVelocity = float.PositiveInfinity;
        _previousPosition = transform.position;
        controller = target.GetComponent<XRController>();


    }

    void FixedUpdate()
    {
        PIDMovement();
        PIDRotation();
        if (isHandMovementEnabled && !movementJoint)
        {
            MoveWithHand();
        }
        
        if (_isColliding)
        {
            HookesLaw();
        }

        // Climbing input handling
        EasyHand inputHand = node == XRNode.LeftHand ? EasyHand.LeftHand : EasyHand.RightHand;
        triggerValue = EasyInputs.GetTriggerButtonFloat(inputHand);
        gripValue = EasyInputs.GetGripButtonFloat(inputHand);

        if (triggerValue <= 0.8f || gripValue <= 0.8f)
        {
            IsClimbing = false;
            if (gameObject.GetComponent<FixedJoint>())
            {
                FixedJoint hingeJoint = this.GetComponent<FixedJoint>();
                Destroy(hingeJoint);
                if (ClimbBool == true)
                {
                    ClimbBool = false;
                }
            }
        }
        else if (triggerValue > 0.8f && gripValue > 0.8f)
        {
            IsClimbing = true;
            Climb(climbingon);
        }
    }




    void Climb(Rigidbody climbingon)
    {

        if (climbingon != null)
        {
            if (!gameObject.GetComponent<FixedJoint>())
            {
                FixedJoint hinge = gameObject.AddComponent<FixedJoint>();
                hinge.connectedBody = climbingon;
                Hinge = hinge;
                if (ClimbBool == false)
                {
                    ClimbBool = true;
                    Grabed.Play();
                    GrabSFX.Play();
                }
            }

        }
    }

    void PIDMovement()
    {
        float kp = (6f * frequency) * (6f * frequency) * 0.25f;
        float kd = 4.5f * frequency * damping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Vector3 force = (target.position - transform.position) * ksg + (playerRigidbody.velocity - _rigidbody.velocity) * kdg;
        _rigidbody.AddForce(force, ForceMode.Acceleration);
    }

    void PIDRotation()
    {
        float kp = (6f * rotfrequency) * (6f * rotfrequency) * 0.25f;
        float kd = 4.5f * rotfrequency * rotdamping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Quaternion q = target.rotation * Quaternion.Inverse(transform.rotation);
        if (q.w < 0)
        {
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }
        q.ToAngleAxis(out float angle, out Vector3 axis);
        axis.Normalize();
        axis *= Mathf.Deg2Rad;
        Vector3 torque = ksg * axis * angle + -_rigidbody.angularVelocity * kdg;
        _rigidbody.AddTorque(torque, ForceMode.Acceleration);
    }

    void HookesLaw()
    {
        Vector3 displacementFromResting = transform.position - target.position;
        Vector3 force = displacementFromResting * climbForce;
        float drag = GetDrag();

        playerRigidbody.AddForce(force, ForceMode.Acceleration);
        playerRigidbody.AddForce(drag * -playerRigidbody.velocity * climbDrag, ForceMode.Acceleration);
    }

    float GetDrag()
    {
        Vector3 handVelocity = (target.localPosition - _previousPosition) / Time.fixedDeltaTime;
        float drag = 1 / handVelocity.magnitude + 0.01f;
        drag = drag > 1 ? 1 : drag;
        drag = drag < 0.03f ? 0.03f : drag;
        _previousPosition = transform.position;
        return drag;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _isColliding = true;
        controller.SendHapticImpulse(0.25f, 0.25f);

        if (collision.gameObject.tag == ClimbTag)
        {
            if (collision.gameObject.GetComponent<Rigidbody>())
            {
                climbingon = collision.gameObject.GetComponent<Rigidbody>();
            }
            else
            {
                climbingon = null;
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isColliding = false;

        if (collision.gameObject.tag == ClimbTag)
        {
            climbingon = null;
        }
    }
    private bool isTeleporting = false;

    // Add this function to initiate the smooth teleport
    public void TeleportPlayerSmooth(Vector3 newPosition, float duration)
    {
        if (!isTeleporting)
        {
            StartCoroutine(TeleportRoutine(newPosition, duration));
        }
    }

    private IEnumerator TeleportRoutine(Vector3 newPosition, float duration)
    {
        isTeleporting = true;

        // Disable collisions
        DisableCollisions();

        // Store original positions
        Vector3 originalPlayerPosition = playerRigidbody.position;
        Vector3 originalHandPosition = transform.position;
        Vector3 originalOtherHandPosition = otherHand.transform.position;

        // Calculate movement over time
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Smoothly interpolate the player's position
            Vector3 currentPlayerPosition = Vector3.Lerp(originalPlayerPosition, newPosition, t);
            playerRigidbody.MovePosition(currentPlayerPosition);

            // Update hand positions
            UpdateHandPositions();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is set
        playerRigidbody.position = newPosition;

        // Reset velocities
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        // Update hand positions one last time
        UpdateHandPositions();

        // Re-enable collisions
        EnableCollisions();

        isTeleporting = false;

    }

    private void DisableCollisions()
    {
        // Disable collision detection on the player's Rigidbody
        playerRigidbody.detectCollisions = false;

        // Disable collisions on the hands
        _rigidbody.detectCollisions = false;
        if (otherHand != null)
        {
            otherHand._rigidbody.detectCollisions = false;
        }
    }

    public void EnableCollisions()
    {
        // Re-enable collision detection on the player's Rigidbody
        playerRigidbody.detectCollisions = true;
        playerRigidbody.isKinematic = false;
        _rigidbody.isKinematic = false;
        // Re-enable collisions on the hands
        _rigidbody.detectCollisions = true;
        if (otherHand != null)
        {
            otherHand._rigidbody.detectCollisions = true;
        }
    }

    private void UpdateHandPositions()
    {
        // Update this hand's position and rotation
        transform.position = target.position;
        transform.rotation = target.rotation;

        // Update the other hand's position and rotation
        if (otherHand != null)
        {
            otherHand.transform.position = otherHand.target.position;
            otherHand.transform.rotation = otherHand.target.rotation;
        }
    }

// New function to enable hand movement
    public void EnableHandMovement()
    {
        if (!movementJoint)
        {
            movementJoint = gameObject.AddComponent<FixedJoint>();
            movementJoint.connectedBody = playerRigidbody;
            isHandMovementEnabled = true;
        }
    }

    // New function to disable hand movement
    public void DisableHandMovement()
    {
        if (movementJoint)
        {
            Destroy(movementJoint);
            isHandMovementEnabled = false;
        }
    }

    private void MoveWithHand()
    {
        // Add force to the playerRigidbody to make it follow the hand smoothly
        Vector3 force = (target.position - transform.position) * climbForce;
        playerRigidbody.AddForce(force, ForceMode.Acceleration);
    }
}
