using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICollisionHandler : MonoBehaviour
{
    WorkerAI workerAI;
    [SerializeField] public bool CollidingIsPosible;

    private void Start()
    {
        workerAI = GetComponent<WorkerAI>();
        CollidingIsPosible = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "CrateTrigger")
        {
            if (CollidingIsPosible)
            {
                workerAI.CollisionTick();

                CollidingIsPosible = false;
                StartCoroutine(EnableCollision());
            }
        }
    }

    private IEnumerator EnableCollision()
    {
        yield return new WaitForSeconds(5f);
        CollidingIsPosible = true;
    }
}
