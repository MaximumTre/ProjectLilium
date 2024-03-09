using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class AnimationMessage : MonoBehaviour
    {
        public PlayerDash PDash;
        public void DashForward()
        {
            PDash.ForwardDash();
        }

        public void DashBack()
        {
            PDash.BackDash();
        }

        public void DashLeft()
        {
            PDash.LeftDash();
        }

        public void DashRight()
        {
            PDash.RightDash();
        }

        public void EndDashAnimation()
        {
            //PDash.EndDashAnimation();
        }
    }
}
