using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class PauseMenuActivator : MonoBehaviour
    {
        public GameObject PauseMenu, OptionsMenu, QuitMenu;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        public void ToggleMenu()
        {
            if(PauseMenu.activeInHierarchy)
            {
                PauseMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                return;
            }

            else
            {
                if(!OptionsMenu.activeInHierarchy)
                {
                    PauseMenu.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }
            }
        }

        public void ActuallyQuit()
        {
            Application.Quit();
        }
    }
}
