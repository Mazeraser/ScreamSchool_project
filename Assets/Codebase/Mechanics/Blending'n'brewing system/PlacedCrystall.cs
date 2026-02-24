using UnityEngine;
using Codebase.Mechanics.Data;
using Codebase.Mechanics.BlendingNBrewingSystem.BrewingStates;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    public class PlacedCrystal : MonoBehaviour
    {
        private BrewingMachine machine;
        private CrystalData data;
        private int clickCount;

        public void Initialize(BrewingMachine machine, CrystalData data)
        {
            this.machine = machine;
            this.data = data;
            clickCount = 0;
        }

        private void OnMouseDown()
        {
            if (machine == null) return;

            switch ((int)machine.CurrentStateType)
            {
                case 2:
                    clickCount++;
                    Debug.Log($"Клик по кристаллу: {clickCount}/{data.clicksToIgnite}");

                    if (clickCount >= data.clicksToIgnite)
                    {
                        machine.IgniteCrystal(); 
                    }
                    break;

                case 3:
                    machine.ClickWhileBrewing();
                    break;

                default:
                    Debug.Log("Сейчас нельзя взаимодействовать с кристаллом");
                    break;
            }
        }
    }
}