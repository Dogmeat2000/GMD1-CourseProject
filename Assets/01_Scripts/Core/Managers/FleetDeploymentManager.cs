using System;
using System.Collections.Generic;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Settings;
using _01_Scripts.Turrets;
using UnityEngine;

namespace _01_Scripts.Core.Managers
{
    /// <summary>
    /// Evaluates the GlobalManager's GameMode and dynamically populates the fleet's hardpoints.
    /// </summary>
    public class FleetDeploymentManager : MonoBehaviour, IService
    {
        [Header("Possible Player controlled ships")]
        [SerializeField] private GameObject centerPlayerShip;
        [SerializeField] private GameObject leftPlayerShip;
        [SerializeField] private GameObject rightPlayerShip;
        
        
        [Header("Single Player turret")]
        [Tooltip("Player1s PlayerTurret GameObject to enable in Singleplayer mode")]
        [SerializeField] private GameObject singlePlayerP1Turret;
        [Tooltip("The Ship Hardpoint that Player1s PlayerTurret is obscuring (will be deactivated)")]
        [SerializeField] private GameObject singlePlayerP1ShipHardPoint;
        
        
        [Header("Coop - Same Ship Player turrets")]
        [Tooltip("Player1s PlayerTurret GameObject to enable in Coop Same Ship mode")]
        [SerializeField] private GameObject coopSameShipP1Turret;
        
        [Tooltip("The Ship Hardpoint that Player1s PlayerTurret is obscuring (will be deactivated)")]
        [SerializeField] private GameObject coopSameShipP1ShipHardPoint;
        
        [Tooltip("Player2s PlayerTurret GameObject to enable in Coop Same Ship mode")]
        [SerializeField] private GameObject coopSameShipP2Turret;
        
        [Tooltip("The Ship Hardpoint that Player2s PlayerTurret is obscuring (will be deactivated)")]
        [SerializeField] private GameObject coopSameShipP2ShipHardPoint;
        
        
        [Header("Coop - Separate Ships Player turrets")]
        [Tooltip("Player1s PlayerTurret GameObject to enable in Coop Separate Ships mode")]
        [SerializeField] private GameObject coopSeparateShipsP1Turret;
        
        [Tooltip("The Ship Hardpoint that Player1s PlayerTurret is obscuring (will be deactivated)")]
        [SerializeField] private GameObject coopSeparateShipP1ShipHardPoint;
        
        [Tooltip("Player2s PlayerTurret GameObject to enable in Coop Separate Ships mode")]
        [SerializeField] private GameObject coopSeparateShipsP2Turret;
        
        [Tooltip("The Ship Hardpoint that Player2s PlayerTurret is obscuring (will be deactivated)")]
        [SerializeField] private GameObject coopSeparateShipP2ShipHardPoint;
        
        
        [Header("General Fleet (AI)")]
        [Tooltip("All other ships in the fleet, that should be equipped with AI turrets")]
        [SerializeField] private GameObject[] alliedFleetShips;
        
        [Header("Turret Prefabs")]
        [SerializeField] private GameObject aiAuxTurretPrefab;

        private void Start() {
            DeployFleet();
        }

        private void DeployFleet() {
            GameMode mode = GlobalManager.Instance ? GlobalManager.Instance.GlobalSettings.ActiveGameMode : GameMode.SinglePlayer;
            Debug.Log($"Selected Game mode is: {mode}");
            
            DisableAllPlayerTurrets();
            EnableAllHardpoints();
            
            switch (mode) {
                case GameMode.SinglePlayer:
                    singlePlayerP1Turret.SetActive(true);
                    singlePlayerP1ShipHardPoint.SetActive(false);
                    break;

                case GameMode.CoopTwoShips:
                    coopSeparateShipsP1Turret.SetActive(true);
                    coopSeparateShipP1ShipHardPoint.SetActive(false);
                    coopSeparateShipsP2Turret.SetActive(true);
                    coopSeparateShipP2ShipHardPoint.SetActive(false);
                    break;

                case GameMode.CoopOneShip:
                    coopSameShipP1Turret.SetActive(true);
                    coopSameShipP1ShipHardPoint.SetActive(false);
                    coopSameShipP2Turret.SetActive(true);
                    coopSameShipP2ShipHardPoint.SetActive(false);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ArmAIShips();
        }
        
        /// <summary>
        /// Scans the ship's hierarchy for valid hardpoints and deploys the requested number of AI controlled weapons.
        /// </summary>
        private void ArmAIShips() {
            HashSet<GameObject> allShips = new HashSet<GameObject>();
            
            if (alliedFleetShips != null) {
                foreach (var s in alliedFleetShips) if (s) allShips.Add(s);
            }

            if (aiAuxTurretPrefab) {
                foreach (GameObject ship in allShips) {
                    foreach (TurretHardpoint socket in ship.GetComponentsInChildren<TurretHardpoint>()) {
                        if (socket.transform.childCount == 0) {
                            socket.EquipTurret(aiAuxTurretPrefab, TurretClass.Auxiliary);
                        }
                    }
                }
            }
        }

        private void DisableAllPlayerTurrets() {
            if (singlePlayerP1Turret) 
                singlePlayerP1Turret.SetActive(false);
            
            if (coopSameShipP1Turret) 
                coopSameShipP1Turret.SetActive(false);
            
            if (coopSameShipP2Turret) 
                coopSameShipP2Turret.SetActive(false);
            
            if (coopSeparateShipsP1Turret) 
                coopSeparateShipsP1Turret.SetActive(false);
            
            if (coopSeparateShipsP2Turret) 
                coopSeparateShipsP2Turret.SetActive(false);
        }

        private void EnableAllHardpoints() {
            if (singlePlayerP1ShipHardPoint) 
                singlePlayerP1ShipHardPoint.SetActive(true);
            
            if (coopSameShipP1ShipHardPoint) 
                coopSameShipP1ShipHardPoint.SetActive(true);
            
            if (coopSameShipP2ShipHardPoint) 
                coopSameShipP2ShipHardPoint.SetActive(true);
            
            if (coopSeparateShipP1ShipHardPoint) 
                coopSeparateShipP1ShipHardPoint.SetActive(true);
            
            if (coopSeparateShipP2ShipHardPoint) 
                coopSeparateShipP2ShipHardPoint.SetActive(true);
        }
    }
}
