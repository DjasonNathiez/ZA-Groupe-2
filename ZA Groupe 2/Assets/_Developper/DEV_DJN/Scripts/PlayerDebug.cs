using TMPro;
using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
   private PlayerManager m_playerManager;

   [Header("Player Stats Debug Informations")]
   public TextMeshProUGUI speedTxt;

   public TextMeshProUGUI healthTxt;

   [Header("Input Callback Debug Informations")]
   public TextMeshProUGUI leftStickXTxt;

   public TextMeshProUGUI rightStickTxt;

   private void Awake()
   {
      m_playerManager = FindObjectOfType<PlayerManager>();
   }

}
