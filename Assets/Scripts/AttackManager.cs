using SpaceMath;
using System.Collections;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    /* Attacking effects */
    private RaycastHit hit;
    private AudioSource attackAudio;
    private LineRenderer attackLine;

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
        Vector3 targetDirection = victim.ShipObject.transform.position - attacker.ShipObject.transform.position;
        targetDirection.y = 0;
        attacker.ShipObject.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        attackLine.enabled = true;
        attackAudio = attacker.ShipObject.GetComponent<AudioSource>();
        if(attackAudio != null)
        {
            attackAudio.Play();
        }

        if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
        {
            victim.ApplyDamage(attacker.AttackDamage);
        }

        yield return new WaitForSeconds(0.5f);
        attackLine.enabled = false;
    }
}
