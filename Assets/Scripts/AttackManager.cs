using SpaceMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private RaycastHit hit;
    //private AudioSource attackAudio;
    private LineRenderer attackLine;

    // TODO delete relatives
    [SerializeField]
    private Vector3 hitPoint;
    [SerializeField]
    private Rigidbody body;


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
        attackLine.SetPosition(0, attacker.WeaponEnd);

        // If the attack line hit something, store the hit infomation in the hit variable.
        if (Physics.Raycast(attackerPos, victimPos - attackerPos, out hit, attacker.WeaponRange))
        {
            // Set the end of the attack line to the hit point.
            hitPoint = hit.point;
            body = hit.rigidbody;
            attackLine.SetPosition(1, hit.point);
        }
        else
        {
            // If the attack didn't hit anything, the attack line will end at the maximum distance.
            attackLine.SetPosition(1, attackerPos + (victimPos - attackerPos) * attacker.WeaponRange);
        }

        // Attack effects will appear every attck.
        StartCoroutine(AttackEffect());

        if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
        {
            victim.ApplyDamage(attacker.AttackDamage);
        }


        return true;
    }

    // Turn on the audio and visual effects of attack, and keep the visual effect for a set time.
    private IEnumerator AttackEffect()
    {
        //attackAudio.Play();
        attackLine.enabled = true;
        yield return new WaitForSeconds(0.05f);
        attackLine.enabled = false;
    }
}
