using Abilitiesforme;
using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace SyrHarpy
{
    public static class Toils_Harpy
    {
        public static Toil GoToNearUnroofedCell(TargetIndex ind, float range)
        {
            Toil toil = new Toil();
            toil.initAction = delegate ()
            {
                Pawn pawn = toil.actor;
                Map map = pawn.Map;
                IntVec3 moveCell = IntVec3.Invalid;
                Predicate<IntVec3> passCheck = delegate (IntVec3 c)
                {
                    bool flag2 = !c.Walkable(map);
                    return !flag2;
                };
                Func<IntVec3, bool> processor = delegate (IntVec3 v)
                {
                    bool flag2 = v.Roofed(map) || v.DistanceTo(pawn.jobs.curJob.GetTarget(ind).Cell) > range;
                    bool result;
                    if (flag2)
                    {
                        result = false;
                    }
                    else
                    {
                        moveCell = v;
                        result = true;
                    }
                    return result;
                };
                map.floodFiller.FloodFill(pawn.Position, passCheck, processor, 2000, false, null);
                bool flag = moveCell != IntVec3.Invalid;
                if (flag)
                {
                    pawn.pather.StartPath(moveCell, PathEndMode.OnCell);
                }
                else
                {
                    pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
                    Messages.Message("HarpyFly_RoofCaster".Translate(), pawn, MessageTypeDefOf.RejectInput, false);
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }

        public static Toil CastFlyAbility(TargetIndex ind)
        {
            Toil toil = new Toil();
            toil.initAction = delegate ()
            {
                Pawn actor = toil.actor;
                HarpyComp harpyComp = actor.TryGetComp<HarpyComp>();
                bool flag = harpyComp != null;
                if (flag)
                {
                    harpyComp.FlyAbility(actor, actor.jobs.curJob.GetTarget(ind).Cell);
                }
                else
                {
                    actor.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }
    }
}