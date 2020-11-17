using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Abilitiesforme
{
    public class FlyAbilitySkyfaller : Skyfaller, IActiveDropPod, IThingHolder
    {
        public override void Tick()
        {
            this.innerContainer.ThingOwnerTick(true);
            this.ticksToImpact--;
            Vector3 drawPos = base.DrawPos;
            Pawn pawn = this.GetThingForGraphic() as Pawn;
            drawPos.z += (Mathf.Pow(2f * this.TimeInAnimation - 1f, 2f) * -1f + 1f) * (float)(this.ticksToImpactMax / 30);
            List<IntVec3> list = GenAdjFast.AdjacentCells8Way(drawPos.ToIntVec3());
            foreach (IntVec3 c in list)
            {
                base.Map.fogGrid.Unfog(c);
            }
            base.Map.fogGrid.Unfog(drawPos.ToIntVec3());
            bool flag = this.ticksToImpact == 15;
            if (flag)
            {
                base.HitRoof();
            }
            bool flag2 = !this.anticipationSoundPlayed && this.def.skyfaller.anticipationSound != null && this.ticksToImpact < this.def.skyfaller.anticipationSoundTicks;
            if (flag2)
            {
                this.anticipationSoundPlayed = true;
                this.def.skyfaller.anticipationSound.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            }
            bool flag3 = this.ticksToImpact == 3;
            if (flag3)
            {
                this.EjectPilot();
            }
            bool flag4 = this.ticksToImpact == 0;
            if (flag4)
            {
                base.Impact();
            }
            else
            {
                bool flag5 = this.ticksToImpact < 0;
                if (flag5)
                {
                    Log.Error("ticksToImpact < 0. Was there an exception? Destroying skyfaller.", false);
                    this.EjectPilot();
                    this.Destroy(DestroyMode.Vanish);
                }
            }
        }

        private void EjectPilot()
        {
            Thing thingForGraphic = this.GetThingForGraphic();
            HarpyComp comp = thingForGraphic.TryGetComp<HarpyComp>();
            bool flag = thingForGraphic != null;
            if (flag)
            {
                GenPlace.TryPlaceThing(thingForGraphic, base.Position, base.Map, ThingPlaceMode.Near, delegate (Thing t, int count)
                {
                    PawnUtility.RecoverFromUnwalkablePositionOrKill(t.Position, t.Map);
                    bool flag2 = t.def.Fillage == FillCategory.Full && this.def.skyfaller.CausesExplosion && this.def.skyfaller.explosionDamage.isExplosive && t.Position.InHorDistOf(this.Position, this.def.skyfaller.explosionRadius);
                    if (flag2)
                    {
                        this.Map.terrainGrid.Notify_TerrainDestroyed(t.Position);
                    }
                    this.CheckDrafting(t);
                    comp.TriggerCooldown();
                }, null, default(Rot4));
            }
        }

        internal void CheckDrafting(Thing thing)
        {
            bool flag = thing != null && thing is Pawn;
            if (flag)
            {
                HarpyComp harpyComp = thing.TryGetComp<HarpyComp>();
                bool flag2 = harpyComp != null && harpyComp.drafted;
                if (flag2)
                {
                    harpyComp.drafted = false;
                    (thing as Pawn).drafter.Drafted = true;
                }
            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Skyfaller.DrawDropSpotShadow(drawLoc, base.Rotation, this.ShadowMaterial, this.def.skyfaller.shadowSize, this.ticksToImpact);
            drawLoc.z += (Mathf.Pow(2f * this.TimeInAnimation - 1f, 2f) * -1f + 1f) * (float)(this.ticksToImpactMax / 30);
            bool flag = this.skyfallerPawn != null;
            if (flag)
            {
                this.skyfallerPawn.RenderPawnAt(drawLoc);
            }
        }

        public ActiveDropPodInfo Contents
        {
            get
            {
                return ((ActiveDropPod)this.innerContainer[0]).Contents;
            }
            set
            {
                ((ActiveDropPod)this.innerContainer[0]).Contents = value;
            }
        }

        private new float TimeInAnimation
        {
            get
            {
                return 1f - (float)this.ticksToImpact / (float)this.ticksToImpactMax;
            }
        }

        private Material ShadowMaterial
        {
            get
            {
                bool flag = this.cachedShadowMaterial == null && !this.def.skyfaller.shadow.NullOrEmpty();
                if (flag)
                {
                    this.cachedShadowMaterial = MaterialPool.MatFrom(this.def.skyfaller.shadow, ShaderDatabase.Transparent);
                }
                return this.cachedShadowMaterial;
            }
        }

        private Thing GetThingForGraphic()
        {
            Thing thing = null;
            bool flag = this.innerContainer.Any && this.innerContainer.Count > 0;
            if (flag)
            {
                for (int i = 0; i < this.innerContainer.Count; i++)
                {
                    Thing thing2 = this.innerContainer[i];
                    bool flag2 = thing2 is Pawn;
                    if (flag2)
                    {
                        thing = thing2;
                    }
                }
            }
            return thing as Pawn;
        }

        private bool anticipationSoundPlayed;

        public int ticksToImpactMax;

        private Material cachedShadowMaterial;

        public PawnRenderer skyfallerPawn;
    }
}