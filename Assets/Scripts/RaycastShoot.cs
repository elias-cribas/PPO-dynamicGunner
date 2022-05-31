using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastShoot : MonoBehaviour
{
    public GameObject impactEffect;
    public ParticleSystem MuzzleFlash;
    public int gunDamage = 50;
    public float fireRate = .25f;
    public float weaponRange = 50f;
    float hitForce = 100f;
    public Transform gunEnd;

    private Camera fpsCan;
    private WaitForSeconds shotDurantion = new WaitForSeconds(.07f);
    private AudioSource gunAudio;
    private LineRenderer laserLine;
    private float nextFire;

    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        fpsCan = GetComponentInParent<Camera>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.deltaTime > nextFire)
        {
            nextFire = Time.time + fireRate;

            StartCoroutine(ShotEffect());

        }
    }

    private IEnumerator ShotEffect()
    {
        MuzzleFlash.Play();
        gunAudio.Play();

        Vector3 rayOrigin = fpsCan.ViewportToWorldPoint(new Vector3(.5f, .5f, 0));
        RaycastHit hit;

        laserLine.SetPosition(0, gunEnd.position);

        if (Physics.Raycast(rayOrigin, fpsCan.transform.forward, out hit, weaponRange))
        {
            laserLine.SetPosition(1, hit.point);

            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();

            if (target != null)
            {
                target.takeDamage(gunDamage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * hitForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }
        else
        {
            laserLine.SetPosition(1, rayOrigin + (fpsCan.transform.forward * weaponRange));
        }

        laserLine.enabled = true;
        yield return shotDurantion;
        laserLine.enabled = false;
    }
}
