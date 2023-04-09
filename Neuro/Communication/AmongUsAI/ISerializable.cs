using System.IO;

namespace Neuro.Communication.AmongUsAI;

public interface ISerializable
{
    void Serialize(BinaryWriter writer);
}
