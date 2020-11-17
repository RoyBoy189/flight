using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Abilitiesforme
{
    public class HarpyComp : ThingComp
    {
        public CompProperties_HarpyComp Props
        {
            get
            {
                return (CompProperties_HarpyComp)this.props;
            }
        }

        public float AdjustedRange
        {
            get
            {
                Pawn pawn = this.parent as Pawn;
                bool flag = pawn != null;
                float result;
                if (flag)
                {
                    result = this.Props.range * HarpyUtility.FlightCapability(pawn) / 2f;
                }
                else
                {
                    result = this.Props.range;
                }
                return result;
            }
        }

        public void TriggerCooldown()
        {
            this.cooldownTicks = this.Props.cooldown * 60f;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Pawn pawn = this.parent as Pawn;
            Pawn pawn2 = pawn;
            bool flag = ((pawn2 != null) ? pawn2.Map : null) != null && pawn.IsColonistPlayerControlled && !pawn.Downed && pawn.Spawned;
            if (flag)
            {
                bool flag2 = Find.Selector.SingleSelectedThing == pawn;
                if (flag2)
                {
                    string label = "FlyLabel".Translate();
                    string desc = "FlyDescription".Translate();
                    Texture2D FlyIcon = ContentFinder<Texture2D>.Get(Command_HarpyFly.IconPath, true);
                    yield return new Command_HarpyFly
                    {
                        defaultLabel = label,
                        defaultDesc = desc,
                        icon = FlyIcon,
                        pawn = pawn,
                        abilityRange = this.AdjustedRange,
                        hotKey = KeyBindingDefOf.Misc3
                    };
                    label = null;
                    desc = null;
                    FlyIcon = null;
                }
            }
            yield break;
        }

        public void FlyAbility(Pawn pawn, IntVec3 targetCell)
        {
            bool flag = this.cooldownTicks > 0f;
            if (flag)
            {
                Messages.Message("Fly_Cooldown".Translate(), MessageTypeDefOf.RejectInput, false);
            }
            else
            {
                this.soundQueued = true;
                float num;
                this.RotateFlyer(pawn, targetCell, out num);
                float speed = GitThingDefOf.FlyAbilitySkyfaller.skyfaller.speed;
                float num2 = pawn.Position.DistanceTo(targetCell);
                int num3 = (int)(num2 / speed);
                num += 270f;
                bool flag2 = num >= 360f;
                if (flag2)
                {
                    num -= 360f;
                }
                Skyfaller skyfaller = SkyfallerMaker.SpawnSkyfaller(GitThingDefOf.FlyAbilitySkyfaller, targetCell, pawn.Map);
                FlyAbilitySkyfaller flyAbilitySkyfaller = skyfaller as FlyAbilitySkyfaller;
                skyfaller.ticksToImpact = num3;
                flyAbilitySkyfaller.ticksToImpactMax = num3;
                flyAbilitySkyfaller.skyfallerPawn = new PawnRenderer(pawn);
                skyfaller.angle = num;
                this.drafted = pawn.Drafted;
                pawn.DeSpawn(DestroyMode.Vanish);
                skyfaller.innerContainer.TryAdd(pawn, false);
            }
        }

        internal void RotateFlyer(Pawn pawn, IntVec3 targetCell, out float angle)
        {
            angle = pawn.Position.ToVector3().AngleToFlat(targetCell.ToVector3());
            float num = angle + 90f;
            bool flag = num > 360f;
            if (flag)
            {
                num -= 360f;
            }
            Rot4 rotation = Rot4.FromAngleFlat(num);
            pawn.Rotation = rotation;
        }

        public override void CompTickRare()
        {
        }

        public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
        {
            base.PrePreTraded(action, playerNegotiator, trader);
            bool flag;
            if (action == TradeAction.PlayerBuys)
            {
                Pawn pawn = this.parent as Pawn;
                flag = (pawn != null);
            }
            else
            {
                flag = false;
            }
            bool flag2 = flag;
            if (flag2)
            {
            }
        }

        public float cooldownTicks;

        public bool drafted;

        public int timeToImpactMax;

        public bool soundQueued = false;

        public Dictionary<ThingDef, int> drugsNursedToday = new Dictionary<ThingDef, int>();
    }
}