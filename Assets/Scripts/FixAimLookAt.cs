using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FixAimLookAt : MonoBehaviour
{
    public MultiAimConstraint AimSpine1;
    public MultiAimConstraint AimSpine2;
    public MultiAimConstraint AimHead;
    public MultiAimConstraint WeaponPose;
    public Transform Aim;
    public RigBuilder rig;
    // Start is called before the first frame update
    void Start()
    {
        Aim = GameObject.Find("AimLookAt").GetComponent<Transform>();

        AimSpine1 = GameObject.Find("AimSpine1").GetComponent<MultiAimConstraint>();
        AimSpine2 = GameObject.Find("AimSpine2").GetComponent<MultiAimConstraint>();
        AimHead = GameObject.Find("AimHead").GetComponent<MultiAimConstraint>();
        WeaponPose = GameObject.Find("WeaponPose").GetComponent<MultiAimConstraint>();

        AimSpine1.data.sourceObjects = new WeightedTransformArray { new WeightedTransform(Aim.transform, 1) };
        AimSpine2.data.sourceObjects = new WeightedTransformArray { new WeightedTransform(Aim.transform, 1) };
        AimHead.data.sourceObjects = new WeightedTransformArray { new WeightedTransform(Aim.transform, 1) };
        WeaponPose.data.sourceObjects = new WeightedTransformArray { new WeightedTransform(Aim.transform, 1) };

        rig = GameObject.Find("Character_NewInputSystem_SplitSceen").GetComponent<RigBuilder>();
        rig.Build();
    }

}
