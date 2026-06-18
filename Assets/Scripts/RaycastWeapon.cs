using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    class Bullet{
        public float time;
        public float impactForce = 300f;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }

    public float damage = 5;

    public LayerMask ignorThis; // ray ignores these layers
    public bool isFiring = false;
    public int fireRate = 25;
    public float bulletSpeed = 1.0f;
    public float bulletDrop = 2.0f;
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;
    public Transform smallTrail0;
    public GameObject HitSprite;

    public int ammoCount = 30;
    public int clipSize = 30;
    public int clipCount = 2;

    public Transform raycastOrigin;
    public Transform raycastDestination;
    public WeaponRecoil recoil;
    public GameObject magazine;

    public LevelMangaer levelManager;
    public Ragdoll ragdoll;

    Ray ray;
    RaycastHit hitInfo;
    float accumulatedTime;
    public float accumulatedTimeLimit;
    List<Bullet> bullets = new List<Bullet>();
    float maxLifetime = 3.0f;

    //Bullet Drop
    Vector3 GetPosition(Bullet bullet){
        Vector3 gravity = Vector3.down * bulletDrop;
        return bullet.initialPosition + (bullet.initialVelocity * bullet.time) + (0.5f * bullet.time * bullet.time * gravity);
    }

    private void Awake()
    {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelMangaer>();
        HitSprite= gameObject.transform.parent.parent.GetChild(5).GetChild(0).GetChild(0).GetChild(0).gameObject;

        if (GameObject.Find("PracticeSettings"))
        {
            ignorThis |= (1 << LayerMask.NameToLayer("HitBox"));
        }
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity){
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, smallTrail0.position, Quaternion.identity);
        bullet.tracer.AddPosition(smallTrail0.position);
        return bullet;
    }

    public void StartFiring() {
        isFiring = true;
        accumulatedTime = 0.0f;
        //recoil.Reset();
    }

    public void UpdateFiring(float deltaTime){
        accumulatedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while(accumulatedTime >= accumulatedTimeLimit){
            FireBullet();
            accumulatedTime -= fireInterval;
        }
    }

    public void UpdateBullets(float deltaTime){
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    void DestroyBullets(){
        bullets.RemoveAll(bullet => bullet.time >= maxLifetime);
    }

    void SimulateBullets(float deltaTime){
        bullets.ForEach(bullet => {
            Vector3 p0 = bullet.initialPosition;
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet){

        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;

        if (Physics.Raycast(ray, out hitInfo, distance, ~ignorThis)){

            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 2.0f);
            //Debug.Log("Object = " + hitInfo.transform + ", Distance = " + distance + hitInfo.collider);

            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal; 
            hitEffect.Emit(1);  

            if (hitInfo.rigidbody != null)
            {
                hitInfo.rigidbody.AddForce(-hitInfo.normal * bullet.impactForce);
            }

            // Damage HitBox //
            var hitBox = hitInfo.collider.GetComponent<HitBox>();
            if (hitBox)
            {
                hitBox.OnRaycastHit(this, ray.direction);
                StartCoroutine(EnableHitImageForCertainTime(0.1f));
            }

            if (GameObject.Find("PracticeSettings"))
            {
                var target = hitInfo.collider.GetComponent<Target>();
                if (target)
                {
                    target.OnRaycastHit(this);
                    StartCoroutine(EnableHitImageForCertainTime(0.1f));
                }
            }

            if (bullet.tracer != null)
            {
                bullet.tracer.transform.position = hitInfo.point;
                bullet.time = maxLifetime;
            }
            
        }else{
            if (bullet.tracer != null)
                bullet.tracer.transform.position = end;
        }
    }

    IEnumerator EnableHitImageForCertainTime(float seconds)
    {
        var HitImage = gameObject.transform.parent.parent.GetChild(5).GetChild(0).GetChild(0).GetChild(0).gameObject;
        HitSprite = HitImage;
        HitImage.SetActive(true);

        yield return new WaitForSeconds(seconds);

        HitImage.SetActive(false);
    }

    private void FireBullet(){
        if (ammoCount <= 0){
            return;
        }
        ammoCount--;

        levelManager.audioManager.PlaySFX(3);

        muzzleFlash.Emit(1);

        Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(bullet);

        recoil.GenerateRecoil();

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;

        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        if (Physics.Raycast(ray, out hitInfo, ~ignorThis))
        {
            //hitEffect.transform.position = hitInfo.point;
            //hitEffect.transform.forward = hitInfo.normal;
            //hitEffect.Emit(1);

            tracer.transform.position = hitInfo.point;
        }
    }

    public void StopFiring() {
        isFiring = false;
    }

    public bool ShouldReload()
    {
        return ammoCount == 0 && clipCount > 0;
    }

    internal void RefillAmmo()
    {
        ammoCount = clipSize;
        clipCount--;
    }
}
