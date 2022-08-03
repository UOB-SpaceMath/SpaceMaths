using SpaceMath;
using System.Collections;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    /* Attacking effects */
    private RaycastHit hit;
    private AudioSource attackAudio;
    private LineRenderer attackLine;

    [SerializeField]
    private float rotationSpeed;
    private float rotationFrames = 120;

    [SerializeField]
    private GameObject DeathExplosionPrefab;
    [SerializeField]
    private GameObject AttackExplosionPrefab;
    private AudioSource explosionAudio;

    // Call this method to attack.
    public bool Attack(Ships attacker, Ships victim)
    {
        // TODO Attach attacking animation and audio to the attacker, and then get these components.
        // attackLine should be disabled originally.
        attackLine = attacker.ShipObject.GetComponent<LineRenderer>();
        //attackAudio = attacker.ShipObject.GetComponent<AudioSource>();

        Vector3 victimPos = victim.ShipObject.transform.position;
        Vector3 attackerPos = attacker.ShipObject.transform.position;

        // Set the beginning position of the attacking laser line.
        attackLine.SetPosition(0, attacker.ShipObject.transform.position);

        // If the attack line hit something, store the hit infomation in the hit variable.
        if (Physics.Raycast(attackerPos, victimPos - attackerPos, out hit, attacker.WeaponRange))
        {
            // Set the end of the attack line to the hit point.
            attackLine.SetPosition(1, hit.point);
        }
        else
        {
            // If the attack didn't hit anything, the attack line will end at the maximum distance.
            attackLine.SetPosition(1, attackerPos + (victimPos - attackerPos) * attacker.WeaponRange);
        }

        // Attack effects will appear every attck.
        StartCoroutine(AttackEffect(attacker, victim, hit));

        return true;
    }

    // Turn on the audio and visual effects of attack, and keep the visual effect for a set time.
    private IEnumerator AttackEffect(Ships attacker, Ships victim, RaycastHit hit)
    {
        /* Rotation */
        Vector3 targetDirection = victim.ShipObject.transform.position - attacker.ShipObject.transform.position;
        targetDirection.y = 0;
        /* No effects */
        //Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        //attacker.ShipObject.transform.rotation = Quaternion.Slerp(attacker.ShipObject.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        for (int i = 1; i <= rotationFrames; i++)
        {
            Debug.Log(i * targetDirection / rotationFrames);
            attacker.ShipObject.transform.rotation = Quaternion.LookRotation(i * targetDirection / rotationFrames, Vector3.up);
            /* No effects */
            //yield return null;
        }

        /* Attacking effects */
        attackLine.enabled = true;
        attackAudio = attacker.ShipObject.GetComponent<AudioSource>();
        if(attackAudio != null)
        {
            attackAudio.Play();
        }

        if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
        {
            Vector3 explosionPosition = victim.ShipObject.transform.position;
            GameObject explosion;
            /* If victim was destroyed, trigger a bigger explosion */
            if (victim.ApplyDamage(attacker.AttackDamage) == 1)
            {
                explosion = Instantiate<GameObject>(DeathExplosionPrefab);
                explosion.transform.position = explosionPosition;
                explosionAudio = GetComponent<AudioSource>();
                explosionAudio.Play();
                yield return new WaitForSeconds(1.0f);
                Destroy(explosion);
            }
            else
            {
                explosion = Instantiate<GameObject>(AttackExplosionPrefab);
                explosion.transform.position = explosionPosition;
                yield return new WaitForSeconds(0.5f);
                Destroy(explosion);
            }
        }

        yield return new WaitForSeconds(0.5f);
        attackLine.enabled = false;
    }
}
