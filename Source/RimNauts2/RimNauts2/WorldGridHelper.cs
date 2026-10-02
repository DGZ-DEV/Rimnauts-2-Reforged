using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RimNauts2 {
    /// <summary>
    /// PORT 1.6. En 1.6 las casillas del mundo dejaron de estar en WorldGrid.tiles: viven en
    /// WorldGrid.Surface (SurfaceLayer).tiles, y el indice de casilla paso a ser la estructura
    /// PlanetTile. Se lee por reflexion para no depender de los tipos, y al escribir se DEVUELVE
    /// LA COPIA a la lista, porque el elemento es un struct.
    /// </summary>
    internal static class WorldGridHelper {
        private static readonly System.Reflection.MethodInfo surfaceGetter = AccessTools.PropertyGetter(typeof(WorldGrid), "Surface");
        private static readonly System.Reflection.FieldInfo tilesField = AccessTools.Field(typeof(PlanetLayer), "tiles");
        private static readonly System.Reflection.BindingFlags Banderas = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        private static bool aviso;

        private static System.Collections.IList Tiles() {
            if (Find.World == null || tilesField == null || surfaceGetter == null) return null;
            object capa = surfaceGetter.Invoke(Find.World.grid, null);
            if (capa == null) return null;
            return tilesField.GetValue(capa) as System.Collections.IList;
        }

        private static System.Reflection.MemberInfo Miembro(Type tipo, string nombre) {
            // PORT 1.6: los campos heredados de una clase base (biome vive en Tile, y las casillas de
            // la superficie son SurfaceTile) NO los encuentra GetField sobre el tipo concreto.
            for (Type t = tipo; t != null; t = t.BaseType) {
                System.Reflection.FieldInfo f = t.GetField(nombre, Banderas | System.Reflection.BindingFlags.DeclaredOnly);
                if (f != null) return f;
                System.Reflection.PropertyInfo p = t.GetProperty(nombre, Banderas | System.Reflection.BindingFlags.DeclaredOnly);
                if (p != null) return p;
            }
            return null;
        }

        private static void Avisar(Type tipo, string nombre) {
            if (aviso) return;
            aviso = true;
            Verse.Log.Warning("[RimNauts2 PORT 1.6] No se encontro el miembro '" + nombre + "' en " + (tipo == null ? "la casilla" : tipo.FullName) + ": hay que revisar la API de 1.6.");
        }

        // --- lectura y escritura genericas sobre la casilla ---
        private static object Leer(int index, string nombre) {
            try {
                System.Collections.IList t = Tiles();
                if (t == null || index < 0 || index >= t.Count) return null;
                object casilla = t[index];
                if (casilla == null) return null;
                System.Reflection.MemberInfo m = Miembro(casilla.GetType(), nombre);
                if (m == null) { Avisar(casilla.GetType(), nombre); return null; }
                System.Reflection.FieldInfo f = m as System.Reflection.FieldInfo;
                return f != null ? f.GetValue(casilla) : ((System.Reflection.PropertyInfo)m).GetValue(casilla);
            } catch (Exception e) { Verse.Log.Warning("[RimNauts2 PORT 1.6] Fallo leyendo " + nombre + " de la casilla " + index + ": " + e.Message); return null; }
        }

        private static void Escribir(int index, string nombre, object valor) {
            try {
                System.Collections.IList t = Tiles();
                if (t == null || index < 0 || index >= t.Count) return;
                object casilla = t[index];
                if (casilla == null) return;
                System.Reflection.MemberInfo m = Miembro(casilla.GetType(), nombre);
                if (m == null) { Avisar(casilla.GetType(), nombre); return; }
                System.Reflection.FieldInfo f = m as System.Reflection.FieldInfo;
                if (f != null) { f.SetValue(casilla, valor); } else { ((System.Reflection.PropertyInfo)m).SetValue(casilla, valor); }
                t[index] = casilla;
            } catch (Exception e) { Verse.Log.Warning("[RimNauts2 PORT 1.6] Fallo escribiendo " + nombre + " en la casilla " + index + ": " + e.Message); }
        }

        public static BiomeDef GetBiome(int index) { return (BiomeDef)Leer(index, "biome"); }
        public static BiomeDef GetBiome(PlanetTile t) { return GetBiome(t.tileId); }
        public static void SetBiome(int index, BiomeDef biome) { Escribir(index, "biome", biome); }
        public static void SetBiome(PlanetTile t, BiomeDef biome) { SetBiome(t.tileId, biome); }
        public static void SetHilliness(int index, Hilliness hilliness) { Escribir(index, "hilliness", hilliness); }
        public static void SetHilliness(PlanetTile t, Hilliness hilliness) { SetHilliness(t.tileId, hilliness); }
    }

    /// <summary>
    /// PORT 1.6. Miembros de vanilla que pasaron a ser privados pero siguen existiendo.
    /// </summary>
    internal static class MirrorVanilla {
        public static readonly AccessTools.FieldRef<RoofGrid, RoofDef[]> roofGrid = AccessTools.FieldRefAccess<RoofGrid, RoofDef[]>("roofGrid");

        private static readonly System.Reflection.MethodInfo getTransportersInGroup = AccessTools.PropertyGetter(typeof(CompLaunchable), "TransportersInGroup");

        public static List<CompTransporter> TransportersInGroup(CompLaunchable c) {
            if (getTransportersInGroup == null) return null;
            return (List<CompTransporter>)getTransportersInGroup.Invoke(c, null);
        }
    }
}