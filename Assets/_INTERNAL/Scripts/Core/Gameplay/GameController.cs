using UI;
using UnityEngine;

namespace Core.Gameplay
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private MovementController _movementController;
        [SerializeField] private TrafficController _trafficController;
        [SerializeField] private UIController _uiController;
    }
}