using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace RimNauts2.Things {
    public class TransportPodArrivalAction : RimWorld.Planet.TransportersArrivalAction {
        public RimWorld.Planet.MapParent map_parent;
        public IntVec3 cell;

        public TransportPodArrivalAction(RimWorld.Planet.MapParent map_parent, IntVec3 cell) {
            this.map_parent = map_parent;
            this.cell = cell;
        }

        public override void ExposeData() {
            base.ExposeData();
            Scribe_References.Look(ref map_parent, "mapParent");
            Scribe_Values.Look(ref cell, "cell");
        }

        public override RimWorld.FloatMenuAcceptanceReport StillValid(IEnumerable<IThingHolder> pods, RimWorld.Planet.PlanetTile destinationTile) {
            RimWorld.FloatMenuAcceptanceReport floatMenuAcceptanceReport = base.StillValid(pods, destinationTile);
            if (!floatMenuAcceptanceReport) return floatMenuAcceptanceReport;
            if (map_parent != null && map_parent.Tile != destinationTile) return false;
            return CanLandInSpecificCell(pods, map_parent);
        }

        // PORT 1.6: la clase base tiene ahora una propiedad abstracta GeneratesMap. Esta accion
        // aterriza en un mapa QUE YA EXISTE (CanLandInSpecificCell exige mapParent.HasMap y el
        // skyfaller se genera en map_parent.Map), asi que NO genera mapa.
        public override bool GeneratesMap { get { return false; } }

        public override void Arrived(List<RimWorld.ActiveTransporterInfo> pods, RimWorld.Planet.PlanetTile tile) {
            RimWorld.Planet.TransportersArrivalActionUtility.RemovePawnsFromWorldPawns(pods);
            RimWorld.ActiveTransporterInfo pod = new RimWorld.ActiveTransporterInfo();
            for (int i = 0; i < pods.Count; i++) {
                pod.innerContainer.TryAddRangeOrTransfer(pods[i].innerContainer, destroyLeftover: true);
            }
            pod.openDelay = 0;
            Thing activeDropPod_thing = ThingMaker.MakeThing(Defs.Loader.thing_delivery_cannon_active);
            RimWorld.ActiveTransporter activeDropPod = (RimWorld.ActiveTransporter) activeDropPod_thing;
            activeDropPod.Contents = pod;
            RimWorld.SkyfallerMaker.SpawnSkyfaller(Defs.Loader.thing_delivery_cannon_incoming, activeDropPod, cell, map_parent.Map);
        }

        public static bool CanLandInSpecificCell(IEnumerable<IThingHolder> pods, RimWorld.Planet.MapParent mapParent) {
            if (mapParent == null || !mapParent.Spawned || !mapParent.HasMap) return false;
            if (mapParent.EnterCooldownBlocksEntering()) {
                return RimWorld.FloatMenuAcceptanceReport.WithFailMessage("MessageEnterCooldownBlocksEntering".Translate(mapParent.EnterCooldownTicksLeft().ToStringTicksToPeriod()));
            }
            return true;
        }
    }
}
