using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 5f; // Snelheid van de vijand
    private Vector3[] startPositions; // Array met startposities

    public CubeAgent2 agent; // Referentie naar de agent voor het belonen
    public Vector3 agentPosition; // Positie van de agent

    private void Start()
    {
        // Zoek de agent in de scene
        agentPosition = agent.transform.position; // Sla de positie van de agent op

        // Definieer de startposities op basis van de positie van de agent
        startPositions = new Vector3[4];
        startPositions[0] = agentPosition + new Vector3(-47f, 0.7f, 3.11f); // Positie 1
        startPositions[1] = agentPosition + new Vector3(0.3f, 0.7f, -43f); // Positie 2
        startPositions[2] = agentPosition + new Vector3(47f, 0.7f, 3.11f); // Positie 3
        startPositions[3] = agentPosition + new Vector3(0.3f, 0.7f, 49f); // Positie 4

        // Spawn de vijand op een willekeurige startpositie
        Respawn();
    }

    private void Update()
    {
        // Beweeg de vijand vooruit langs de x-as
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    // Wordt aangeroepen wanneer dit object botst met een andere collider (geen trigger)
    private void OnCollisionEnter(Collision collision)
    {
        // Controleer of de gebotste collider de grens is
        if (collision.collider.CompareTag("Border"))
        {
            // Respawn de vijand op een nieuwe willekeurige startpositie
            // Geef een negatieve beloning aan de agent
            agent.AddReward(1.0f);
            agent.EndEpisode();
            Respawn();
        }
        // Controleer of de gebotste collider een Agent is
        else if (collision.collider.CompareTag("Agent"))
        {
            // Respawn de vijand op een nieuwe willekeurige startpositie
            Respawn();
        }
    }

    // Kies willekeurig een startpositie uit de array en spawn de vijand daar
    private void Respawn()
    {
        // Kies een willekeurige index van 0 tot 3 (aantal startposities - 1)
        int startPositionIndex = Random.Range(0, startPositions.Length);

        // Stel de positie van de vijand in op de gekozen startpositie
        transform.position = startPositions[startPositionIndex];

        Debug.Log("Vijand gespawned op startpositie " + (startPositionIndex + 1));

        // Pas de rotatie aan op basis van de startpositie
        if (startPositionIndex == 0) // Als startpositie 1 is
        {
            // Draai de y-rotatie naar 0 graden
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        // Geen specifieke rotatie voor andere startposities omdat ze hun standaard rotatie behouden
        if (startPositionIndex == 2) // Als startpositie 2 is
        {
            // Draai de y-rotatie naar 0 graden
            transform.rotation = Quaternion.Euler(0f, -180f, 0f);
        }
        if (startPositionIndex == 3) // Als startpositie 2 is
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        if (startPositionIndex == 1) // Als startpositie 2 is
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
    }
}
