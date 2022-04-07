using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStatesData;
using static GameMgr;

public class PlayerEffectController : MonoBehaviour
{
    PlayerController player;

    bool darkDebuff;
    public bool DarkDebuff { get { return darkDebuff; } set { darkDebuff = value; } }
    bool slowDebuff;
    public bool SlowDebuff { get { return slowDebuff; } set { slowDebuff = value; } }

    private void Awake()
    {
        darkDebuff = false;
        slowDebuff = false;
    }
    private void Update()
    {
        if (GM.CurRoomMgr != null)
        {
            if (GM.CurRoomMgr.clear)
            {
                darkDebuff = false;
                slowDebuff = false;
            }
        }
    }
}
