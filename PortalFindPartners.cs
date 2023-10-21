using Kitchen;
using KitchenMods;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace KitchenPlayerTeleporter
{
    public class PortalFindPartners : GameSystemBase, IModSystem
    {
        private EntityQuery Portals;

        private HashSet<int> _usedPortalIDs = new HashSet<int>();

        protected override void Initialise()
        {
            base.Initialise();
            Portals = GetEntityQuery(typeof(CConveyTeleport), typeof(CPortal));
        }

        protected override void OnUpdate()
        {
            if (Portals.IsEmpty)
            {
                return;
            }
            using NativeArray<Entity> entities = Portals.ToEntityArray(Allocator.Temp);
            using NativeArray<CConveyTeleport> portals = Portals.ToComponentDataArray<CConveyTeleport>(Allocator.Temp);
            _usedPortalIDs.Clear();
            foreach (CConveyTeleport item in portals)
            {
                _usedPortalIDs.Add(item.GroupID);
            }
            Entity entity = default(Entity);

            foreach (Entity entity2 in entities)
            {
                if (!Require(entity2, out CConveyTeleport comp) || comp.Target != default(Entity))
                {
                    continue;
                }
                if (entity == default(Entity))
                {
                    entity = entity2;
                    continue;
                }
                int groupID = 1;
                for (int i = 1; i < portals.Length; i++)
                {
                    if (!_usedPortalIDs.Contains(i))
                    {
                        groupID = i;
                        break;
                    }
                }
                Set(entity2, new CConveyTeleport
                {
                    Target = entity,
                    GroupID = groupID
                });
                Set(entity, new CConveyTeleport
                {
                    Target = entity2,
                    GroupID = groupID
                });
                break;
            }
        }
    }
}
