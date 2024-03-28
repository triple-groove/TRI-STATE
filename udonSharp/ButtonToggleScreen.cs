
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ButtonToggleScreen : UdonSharpBehaviour
{
    public GameObject toggleObject;

    private void Start()
    {
        toggleObject.SetActive(false);
    }

    public override void Interact()
    {
        toggleObject.SetActive(!toggleObject.activeSelf);
    }
}
