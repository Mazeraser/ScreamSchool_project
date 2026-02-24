using System.Collections.Generic;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    public class BlendMemento
    {
        public IReadOnlyList<IngredientData> Ingredients { get; }
        public bool IsBlendSuccessful { get; }

        public BlendMemento(IReadOnlyList<IngredientData> ingredients, bool isBlendSuccessful)
        {
            Ingredients = new List<IngredientData>(ingredients).AsReadOnly();
            IsBlendSuccessful = isBlendSuccessful;
        }
    }
}