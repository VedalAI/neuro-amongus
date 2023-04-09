using System.IO;

namespace Neuro.Communication.AmongUsAI;

public interface IDeserializable
{
    void Deserialize(BinaryReader reader);
}
