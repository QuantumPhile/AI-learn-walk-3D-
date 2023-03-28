// Code by Kanento

using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class WalkerAgent : Agent
{
    // Référence au corps du Walker
    private Rigidbody m_Rigidbody;

    // Variables de config
    public float maxVelocity = 10f;
    public float moveForce = 10f;
    public float rotationSpeed = 100f;

    public override void Initialize()
    {
        // Récupération de la référence au Rigidbody
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Réinitialisation de l'agent par épisode
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = new Vector3(0f, 0.5f, 0f);
        transform.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observation de la position et de la vitesse du Walker
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(m_Rigidbody.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Récupération de l'action de déplacement
        float moveInput = actions.ContinuousActions[0];

        // Mouvement du Walker en fonction de l'action
        m_Rigidbody.AddForce(transform.forward * moveInput * moveForce);

        // Limitation de la vitesse maximale
        if (m_Rigidbody.velocity.magnitude > maxVelocity)
        {
            m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * maxVelocity;
        }

        // Récupération de l'action de rotation
        float rotateInput = actions.ContinuousActions[1];

        // Rotation du Walker en fonction de l'action
        transform.Rotate(transform.up * rotateInput * rotationSpeed * Time.deltaTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Mode Heuristique pour contrôler manuellement le Walker
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Fin de l'épisode si le Walker tombe ou entre en collision avec un obstacle
        if (collision.gameObject.CompareTag("Obstacle") || transform.localPosition.y < -0.5f)
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Reward if walker touch the cible/ en français : récompense si le walker touche l'objectif
        if (other.gameObject.CompareTag("Reward"))
        {
            SetReward(1f);
            EndEpisode();
        }
    }
}


//Code by Kanento