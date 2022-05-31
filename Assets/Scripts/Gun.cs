using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour 
{
    public int bulletMag = 30;
    public int bulletLeft;
    public int currentBullets;

    public float damage = 50f;
    public float range = 50f;
    public float fireRate = 15f;
    public float impactForce = 30f;

    public Camera fpsCan;
    public ParticleSystem MuzzleFlash;
    public GameObject impactEffect;
    public Transform firePoint;
    public LineRenderer lineRenderer;
    
    private Animator anim;

    private bool isRunning;
    private bool isWalking;

    private float nextTimeToFire = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentBullets = bulletMag;
    }

    void Update()
    {
        //Walk
        if(Input.GetButton("Horizontal") && !isRunning || Input.GetButton("Vertical") && !isRunning )
        {
            anim.SetBool("Walk",true);
        }else
        {
            anim.SetBool("Walk",false);
        }

        //Running
        if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift)){
            isRunning = true;
            anim.SetBool("Run",true);
        }else{
            isRunning = false;
            anim.SetBool("Run",false);
        }

        //Aim
        if(Input.GetButton("Fire2")){
            anim.SetBool("Aim",true);
        }else{
            anim.SetBool("Aim",false);
        }
        
        // Shooting
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            anim.SetBool("Shoot",true);
            nextTimeToFire = Time.time + 1f / fireRate;
            StartCoroutine(Shoot());
        }else
        {
            anim.SetBool("Shoot", false);
        }
    }

    IEnumerator Shoot()
    {
        anim.SetBool("Shoot", true);
        MuzzleFlash.Play();
        RaycastHit hitheg;
        if (Physics.Raycast(fpsCan.transform.position, fpsCan.transform.forward, out hitheg, range))
        {
            Debug.Log(hitheg.transform.name);

            Target target = hitheg.transform.GetComponent<Target>();

            if (target != null)
            {
                target.takeDamage(damage);
            }

            if (hitheg.rigidbody)
            {
                hitheg.rigidbody.AddForce(-hitheg.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hitheg.point, Quaternion.LookRotation(hitheg.normal));
            Destroy(impactGO, 2f);

            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hitheg.point);
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hitheg.point + firePoint.forward * 100);
        }

        lineRenderer.enabled = true;

        yield return new WaitForSeconds(.02f);

        lineRenderer.enabled = false;

        currentBullets--;

    }

}
