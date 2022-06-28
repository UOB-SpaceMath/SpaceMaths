using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    // [SerializeField]
    // private GameObject player;
    [SerializeField]
    private int attackDamage = 1;
    [SerializeField]
    private float weaponRange = 3.0f;
    // Where the weapon is.
    [SerializeField]
    private Transform weaponEnd;

    private RaycastHit hit;
    //private AudioSource attackAudio;
    private LineRenderer attackLine;

    void Start()
    {
        // TODO Attach attacking animation and audio to the attacker, and then get these components.
        attackLine = GetComponent<LineRenderer>();
        //attackAudio = GetComponent<AudioSource>();
        // player = GameObject.FindGameObjectWithTag("Player");
        // weaponEnd = player.transform;
    }

    void Update()
    {

    }

    // Call this method to attack.
    public bool Attack(GameObject attacker, GameObject victim)
    {
        Vector3 victimPos = victim.transform.position;
        Vector3 attackerPos = attacker.transform.position;

        // Set the beginning position of the attacking laser line.
        attackLine.SetPosition(0, weaponEnd.position);

        // If the attack line hit something, store the hit infomation in the hit variable.
        if (Physics.Raycast(attackerPos, victimPos - attackerPos, out hit, weaponRange))
        {
            // Set the end of the attack line to the hit point.
            attackLine.SetPosition(1, hit.point);
        }
        else
        {
            // If the attack didn't hit anything, the attack line will end at the maximum distance.
            attackLine.SetPosition(1, attackerPos + (victimPos - attackerPos) * weaponRange);
        }

        // Attack effects will appear every attck.
        StartCoroutine(AttackEffect());

        if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
        {
            //victim.DecreaseHealth(attackDamage);
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
