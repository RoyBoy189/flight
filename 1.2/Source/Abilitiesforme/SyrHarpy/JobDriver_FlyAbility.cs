using Abilitiesforme;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    public class JobDriver_FlyAbility : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            HarpyComp comp = this.pawn.TryGetComp<HarpyComp>();
            bool flag = comp != null;
            if (flag)
            {
                bool flag2 = this.pawn.Position.Roofed(this.pawn.Map);
                if (flag2)
                {
                    yield return Toils_Harpy.GoToNearUnroofedCell(TargetIndex.A, comp.AdjustedRange);
                }
                yield return Toils_Harpy.CastFlyAbility(TargetIndex.A);
            }
            yield break;
        }
    }
}