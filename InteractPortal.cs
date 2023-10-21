using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenPlayerTeleporter
{
    public class InteractPortal : ItemInteractionSystem, IModSystem
    {
        CPosition Position;

        private CPlayerColour Colour;

        protected override InteractionType RequiredType => InteractionType.Act;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Has<CPortal>(data.Target) || !Require(data.Target, out CConveyTeleport conveyTeleport) || conveyTeleport.Target == default)
            {
                return false;
            }

            if (!Has<CPortal>(conveyTeleport.Target) || !Require(conveyTeleport.Target, out Position))
            {
                return false;
            }

            if (!Has<CPlayer>(data.Interactor) || !Has<CPosition>(data.Interactor))
            {
                return false;
            }

            if (!Require<CPlayerColour>(data.Interactor, out Colour))
            {
                return false;
            }

            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            Position.ForceSnap = true;
            data.Context.Set(data.Interactor, Position);
            MakePing(data);
        }

        private void MakePing(in InteractionData data)
        {
            Entity entity = data.Context.CreateEntity();
            data.Context.Set(entity, new CRequiresView
            {
                Type = Main.PlayerTeleportBeamView
            });
            data.Context.Set(entity, Position);
            data.Context.Set(entity, new CLifetime
            {
                RemainingLife = 0.25f
            });
            data.Context.Set(entity, new CPlayerPing
            {
                Colour = Colour
            });
        }
    }
}
