namespace Neuro.Recording.Common;

public static class MapTypeExtensions
{
    public static MapType GetTypeForMessage(this ShipStatus shipStatus)
    {
        if (!shipStatus) return MapType.NoneMapType;
        if (shipStatus.TryCast<AirshipStatus>()) return MapType.Airship;
        switch (shipStatus.Type)
        {
            case ShipStatus.MapType.Ship: return MapType.Skeld;
            case ShipStatus.MapType.Hq: return MapType.MiraHq;
            case ShipStatus.MapType.Pb: return MapType.Polus;
            default:
                Warning($"Unkown ship status type: {shipStatus.GetIl2CppType()} with MapType {shipStatus.Type}");
                return MapType.NoneMapType;
        }
    }
}
