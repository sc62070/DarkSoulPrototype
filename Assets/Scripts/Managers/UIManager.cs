using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public RectTransform healthBar;
    public RectTransform healthBarFill;

    public RectTransform staminaBar;
    public RectTransform staminaBarFill;

    public GameObject interactionPanel;

    public Character character;

    void Start() {

    }

    void LateUpdate() {

        if(character != null) {
            healthBarFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (character.health / character.MaxHealth) * healthBar.rect.width);
            healthBarFill.anchoredPosition = Vector2.zero;

            staminaBarFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (character.stamina / character.maxStamina) * staminaBar.rect.width);
            staminaBarFill.anchoredPosition = Vector2.zero;
        }

        interactionPanel.SetActive(Interactable.active != null);
        if(Interactable.active != null) {
            interactionPanel.GetComponentInChildren<Text>().text = Interactable.active.Description;
        }
        
    }
}
