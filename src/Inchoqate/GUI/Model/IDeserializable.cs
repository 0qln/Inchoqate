namespace Inchoqate.GUI.Model;

// ReSharper disable once UnusedTypeParameter
// ReSharper disable once InconsistentNaming
/// <summary>
///     Every deserializable object should implement this interface.
/// </summary>
/// <typeparam name="ImplementingType">
///     Issue: https://github.com/JamesNK/Newtonsoft.Json/issues/1284
///     Json deserialization messes up default values in constructors.
///     We need to explicitly assign default values on usage or use overloads.
///     The implementing type is in this interface forced to have a default constructor.
/// </typeparam>
public interface IDeserializable<ImplementingType>
    where ImplementingType : new()
{
}