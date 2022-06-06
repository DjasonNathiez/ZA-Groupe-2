using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public enum ColliderMode
    {
        CollisionEnter2D = 1 << 0,
        CollisionStay2D = 1 << 1,
        CollisionExit2D = 1 << 2,
        TriggerEnter2D = 1 << 3,
        TriggerStay2D = 1 << 4,
        TriggerExit2D = 1 << 5,
        CollisionEnter3D = 1<<6,
        CollisionStay3D = 1<<7,
        CollisionExit3D = 1<<8,
        TriggerEnter3D = 1<<9,
        TriggerStay3D = 1<<10,
        TriggerExit3D = 1<<11
    }


}
