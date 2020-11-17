using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using RimWorld;

namespace Abilitiesforme
{
    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {

            new Harmony("harmony.Abilitiesforme").PatchAll();
        }
    }

    [HarmonyPatch(typeof(Bullet))]
    class PatchBullet
    {
        [HarmonyPrefix]
        [HarmonyPatch("Impact")]
        [HarmonyPatch(new Type[] { typeof(Thing) })]
        static bool PostFix(Thing hitThing, ref Bullet __instance)
        {
            if (hitThing != null)
            {
                Pawn pawn = hitThing as Pawn;
                if (pawn != null && pawn.health.hediffSet.HasHediff(GitThingDefOf.AF_NoBulletDamage))
                {
                    __instance.Destroy();
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}
