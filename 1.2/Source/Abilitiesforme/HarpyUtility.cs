using System.Linq;
using Verse;

namespace Abilitiesforme
{
    public static class HarpyUtility
    {
        public static bool FlightCapabable(Pawn pawn)
        {
            bool flag = pawn.RaceProps.body.GetPartsWithDef(GitThingDefOf.MechWing).Any<BodyPartRecord>();
            return flag && HarpyUtility.FlightCapability(pawn) > 1f;
        }

        public static float FlightCapability(Pawn pawn)
        {
            bool flag = pawn.RaceProps.body.GetPartsWithDef(GitThingDefOf.MechWing).Any<BodyPartRecord>();
            float result;
            if (flag)
            {
                BodyDef body = pawn.RaceProps.body;
                float num = 0f;
                foreach (BodyPartRecord part in body.GetPartsWithDef(GitThingDefOf.MechWing))
                {
                    num += PawnCapacityUtility.CalculatePartEfficiency(pawn.health.hediffSet, part, false, null);
                }
                result = num;
            }
            else
            {
                result = 0f;
            }
            return result;
        }
    }
}