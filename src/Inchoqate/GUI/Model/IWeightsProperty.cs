using Inchoqate.GUI.View;
using OpenTK.Mathematics;

namespace Inchoqate.GUI.Model;

[ViewProperty]
public interface IWeightsProperty
{
    public const float PerWeightMin = 0; 

    public const float PerWeightMax = 1; 

    public const float TotalWeightSum = 1;

    public Vector3 Weights { get; set; }

    /// <summary>
    ///     Checks if the weights are valid.
    ///     Weights must sum up to 1.
    ///     Each weight must be between 0 and 1.
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    public bool IsValid(Vector3 oldValue, Vector3 newValue)
    {
        return newValue.X is >= PerWeightMin and <= PerWeightMax
               && newValue.Y is >= PerWeightMin and <= PerWeightMax
               && newValue.Z is >= PerWeightMin and <= PerWeightMax
               && Math.Abs(newValue.Sum() - TotalWeightSum) < 0.00001;
    }
}