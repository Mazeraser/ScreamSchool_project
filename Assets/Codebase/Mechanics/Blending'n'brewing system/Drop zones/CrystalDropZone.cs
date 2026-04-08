using UnityEngine;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    public class CrystalDropZone : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BrewingMachine brewingMachine;
        [SerializeField] private Transform crystalSocket;

        public void DropCrystal(CrystalData data)
        {
            if (brewingMachine == null)
            {
                Debug.LogError("CrystalDropZone: BrewingMachine не назначена");
                return;
            }

            if ((int)brewingMachine.CurrentStateType != 1)
            {
                Debug.Log("Нельзя установить кристалл сейчас: машина не готова");
                return;
            }

            Transform socket = crystalSocket != null ? crystalSocket : transform;
            
            foreach (Transform child in socket)
            {
                Destroy(child.gameObject);
            }

            GameObject placedCrystal = Instantiate(data.worldPrefab, socket);

            PlacedCrystal clickHandler = placedCrystal.AddComponent<PlacedCrystal>();
            clickHandler.Initialize(brewingMachine, data);

            brewingMachine.PlaceCrystal(data);
        }
    }
}