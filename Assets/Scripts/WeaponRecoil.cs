using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WeaponRecoil : MonoBehaviour
{
    public CharacterAiming characterAiming;
    [HideInInspector] public Cinemachine.CinemachineImpulseSource cameraShake;
    
    public Animator rigController;

    public Vector2[] recoilPattern;
    public float duration;
    public Camera mainCamera;

    float verticalRecoil;
    float horizontalRecoil;
    float time;
    int index;
    private void Awake(){
        cameraShake = GetComponent<CinemachineImpulseSource>();
    }

    public void Reset(){
        index = 0;
    }

    int NextIndex(int index){
        return (index + 1) % recoilPattern.Length;
    }

    public void GenerateRecoil(){
        time = duration;

        //cameraShake.GenerateImpulse(Camera.main.transform.forward);
        cameraShake.GenerateImpulse(mainCamera.transform.forward);

        horizontalRecoil = recoilPattern[index].x;
        verticalRecoil = -recoilPattern[index].y;

        index = NextIndex(index);

        rigController.Play("weapon_recoil", 1, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0)
        {
            characterAiming.yAxis.Value -= ((verticalRecoil / 4) * Time.deltaTime) / duration;
            characterAiming.xAxis.Value -= ((horizontalRecoil / 10) * Time.deltaTime) / duration;
            time = -Time.deltaTime;
        }
    }
}
