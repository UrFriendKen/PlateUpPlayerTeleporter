using KitchenData;
using KitchenMods;
using System.Runtime.InteropServices;
using Unity.Entities;

namespace KitchenPlayerTeleporter
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CPortal : IApplianceProperty, IAttachableProperty, IComponentData, IModComponent
    {
    }
}
