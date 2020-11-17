using RimWorld;
using Verse;

namespace Abilitiesforme
{
    [DefOf]
    public static class GitThingDefOf
    {
        static GitThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GitThingDefOf));
        }

        public static BodyPartDef MechWing;

        public static ThingDef FlyAbilitySkyfaller;

        public static JobDef HarpyJob_Flyability;

        public static HediffDef AF_NoBulletDamage;
    }
}