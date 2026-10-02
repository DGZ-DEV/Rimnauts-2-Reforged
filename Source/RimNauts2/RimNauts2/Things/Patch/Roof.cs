using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimNauts2.Things.Patch {
    [HarmonyPatch(typeof(RoofGrid), "GetCellExtraColor")]
    public static class RoofGrid_GetCellExtraColor {
        public static bool Prefix(ref RoofGrid __instance, ref Color __result, int index) {
            if (MirrorVanilla.roofGrid(__instance)[index] != Defs.Loader.roof_magnetic_field) return true;
            __result = Color.blue;
            return false;
        }

    // PORT 1.6: el juego revienta con NullReferenceException DENTRO de
    // RimWorld.CompTransporter::CompGetGizmosExtra al seleccionar un pod del mod, siempre en el
    // mismo punto. Ese metodo es vanilla, asi que el nulo depende del estado del edificio. Este
    // prefijo escribe ese estado ANTES de que reviente, para saber que falta exactamente.
    // PORT 1.6: el juego revienta con NullReferenceException DENTRO de
    // RimWorld.CompTransporter::CompGetGizmosExtra al seleccionar un pod del mod, siempre en el
    // mismo punto (IL 0x00569). Ese metodo es vanilla, asi que el nulo depende del estado del
    // edificio. Esta sonda lo escribe ANTES de que reviente. Escribe una sola vez por sesion.
    [HarmonyPatch(typeof(RimWorld.CompTransporter), "CompGetGizmosExtra")]
    class CompTransporter_CompGetGizmosExtra {
        private static bool __sonda_hecha = false;

        public static void Prefix(RimWorld.CompTransporter __instance) {
            if (__sonda_hecha) return;
            __sonda_hecha = true;
            try {
                ThingWithComps p = __instance == null ? null : __instance.parent as ThingWithComps;
                RimWorld.CompLaunchable lanz = p == null ? null : p.GetComp<RimWorld.CompLaunchable>();
                string def = (p == null || p.def == null) ? "NULO" : p.def.defName;
                string grupo = "sin lanzador";
                if (lanz != null) {
                    List<RimWorld.CompTransporter> g = MirrorVanilla.TransportersInGroup(lanz);
                    grupo = g == null ? "NULO" : g.Count.ToString();
                }
                string linea = "[RimNauts2 PORT 1.6] Pod: def=" + def
                    + " | spawned=" + (p != null && p.Spawned)
                    + " | map=" + (p != null && p.Map != null)
                    + " | launchable=" + (lanz != null)
                    + " | props=" + (lanz != null && lanz.Props != null)
                    + " | compsDef=" + (p == null || p.def == null || p.def.comps == null ? "NULO" : p.def.comps.Count.ToString())
                    + " | compsEdificio=" + (p == null ? "NULO" : p.AllComps.Count.ToString())
                    + " | grupo=" + grupo
                    + " | cosasDentro=" + (__instance != null && __instance.innerContainer != null ? __instance.innerContainer.Count.ToString() : "NULO");
                Verse.Log.Message(linea);
            } catch (System.Exception e) { Verse.Log.Message("[RimNauts2 PORT 1.6] Fallo la sonda del pod: " + e.Message); }
        }
    }

    [HarmonyPatch(typeof(RimWorld.PlaceWorker_NotUnderRoof), "AllowsPlacing")]
    class PlaceWorker_NotUnderRoof_AllowsPlacing {
        public static bool Prefix(ref AcceptanceReport __result, BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore, Thing thing) {
            if (map.roofGrid.RoofAt(loc) != Defs.Loader.roof_magnetic_field) return true;
            __result = (AcceptanceReport) true;
            return false;
        }
    }

    [HarmonyPatch(typeof(RimWorld.CompLaunchable), "AnyInGroupIsUnderRoof", MethodType.Getter)]
    class CompLaunchable_AnyInGroupIsUnderRoof {
        public static void Postfix(ref RimWorld.CompLaunchable __instance, ref bool __result) {
            if (!__result) return;
            // PORT 1.6: este postfijo no comprobaba NADA. Si la lista venia nula (que es lo que
            // pasaba), el juego lanzaba NullReferenceException al dibujar los botones del pod, cada
            // vez que se seleccionaba. Ahora se comprueba cada paso.
            if (__instance == null || __instance.parent == null || __instance.parent.Map == null) return;
            List<RimWorld.CompTransporter> transportersInGroup = MirrorVanilla.TransportersInGroup(__instance);
            if (transportersInGroup == null) return;
            for (int index = 0; index < transportersInGroup.Count; ++index) {
                RimWorld.CompTransporter objetivo = transportersInGroup[index];
                if (objetivo == null || objetivo.parent == null || !objetivo.parent.Spawned) continue;
                if (objetivo.parent.Position.Roofed(__instance.parent.Map) && objetivo.parent.Position.GetRoof(__instance.parent.Map) != Defs.Loader.roof_magnetic_field) {
                    __result = true;
                    return;
                }
            }
            __result = false;
            return;
        }
    }

    [HarmonyPatch(typeof(RoofCollapserImmediate), "DropRoofInCellPhaseOne")]
    class RoofCollapserImmediate_DropRoofInCellPhaseOne {
        public static bool Prefix(IntVec3 c, Map map, List<Thing> outCrushedThings) {
            if (map.roofGrid.RoofAt(c) != Defs.Loader.roof_magnetic_field) return true;
            return false;
        }
    }

    [HarmonyPatch(typeof(RoofCollapserImmediate), "DropRoofInCellPhaseTwo")]
    class RoofCollapserImmediate_DropRoofInCellPhaseTwo {
        public static bool Prefix(IntVec3 c, Map map) {
            if (map.roofGrid.RoofAt(c) != Defs.Loader.roof_magnetic_field) return true;
            return false;
        }
    }
}
}
