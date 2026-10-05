using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonPatch
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            new Harmony("ifchen0.prisonpatch").PatchAll();
        }
    }

    /// <summary>
    /// Vanilla drops the food policy whenever the eater is in a mental state, regardless of who is fetching the food.
    /// That is meant for a breaking pawn feeding itself, but it also lets a warden deliver restricted food to a
    /// prisoner who is mid-break. Keep the policy when someone else is the getter; self-feeding stays vanilla.
    /// </summary>
    [HarmonyPatch(typeof(Pawn_FoodRestrictionTracker), nameof(Pawn_FoodRestrictionTracker.GetCurrentRespectedRestriction))]
    public static class Patch_GetCurrentRespectedRestriction
    {
        public static void Postfix(Pawn_FoodRestrictionTracker __instance, Pawn getter, ref FoodPolicy __result)
        {
            if (__result != null || getter == null)
                return;
            Pawn pawn = __instance.pawn;
            if (getter == pawn || !pawn.InMentalState || !__instance.Configurable)
                return;
            // Same faction gate vanilla applies before its mental state check.
            if (pawn.Faction != Faction.OfPlayer && getter.Faction != Faction.OfPlayer)
                return;
            __result = __instance.CurrentFoodPolicy;
        }
    }
}
