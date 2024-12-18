using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMenuManager : MonoBehaviour
{
    public GameObject mainMenu;      // Main Menu Panel
    public GameObject slide1;        // Tutorial Slide 1
    public GameObject slide2;        // Tutorial Slide 2

    // Show Slide 1 (from Main Menu)
    public void ShowSlide1()
    {
        mainMenu.SetActive(false);   // Hide Main Menu
        slide1.SetActive(true);      // Show Slide 1
    }

    // Show Slide 2 (from Slide 1)
    public void ShowSlide2()
    {
        slide1.SetActive(false);     // Hide Slide 1
        slide2.SetActive(true);      // Show Slide 2
    }

    // Return to Slide 1 (from Slide 2)
    public void ReturnToSlide1()
    {
        slide2.SetActive(false);     // Hide Slide 2
        slide1.SetActive(true);      // Show Slide 1
    }

    // Return to Main Menu (from Slide 2)
    public void ReturnToMainMenu()
    {
        slide2.SetActive(false);     // Hide Slide 2
        mainMenu.SetActive(true);    // Show Main Menu
    }
}