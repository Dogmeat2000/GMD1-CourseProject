using UnityEngine;

namespace _01_Scripts.Core.Entities
{
    /// <summary>
    /// Holds static identifying data for a ship, allowing UI elements 
    /// to extract its name and icon natively without hardcoding.
    /// </summary>
    public class ShipIdentity : MonoBehaviour
    {
        [Tooltip("The tactical designation of this vessel (e.g., CVN-73 WASP)")]
        [field: SerializeField] 
        public string DisplayName { get; private set; } = "CVN-00 NoName";

        [Tooltip("The 2D sprite representing this ship on the HUD")]
        [field: SerializeField] 
        public Sprite HudIcon { get; private set; }
    }
}
