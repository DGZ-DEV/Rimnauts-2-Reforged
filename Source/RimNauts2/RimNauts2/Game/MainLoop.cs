using RimNauts2.Things;
using System.Collections.Generic;
using Verse;

namespace RimNauts2.Game {
    public class MainLoop : GameComponent {
        public static MainLoop instance;

        // PORT 1.6: campo heredado del original que en 1.6 ya no se usa. Se conserva (no se
        // borra nada) y se silencia el aviso para que la compilacion salga limpia.
        #pragma warning disable 0169
        private TickManager _tickManager;
        #pragma warning restore 0169

        public List<SatelliteInformationCommand> satelliteCommands;

        public MainLoop(Verse.Game game) : base() {
            instance = this;

            satelliteCommands = new List<SatelliteInformationCommand> {
                new Relay(),
                new EnergyRelay(),
                new EnergyBooster(),
                new CosmicSurveillance(),
            };
        }

        public override void GameComponentTick() {
            /*if (_tickManager == null) {
                _tickManager = Find.TickManager;
                return;
            }

            if (_tickManager.TicksGame % 250 != 0) return;

            foreach (var satelliteCommand in satelliteCommands) satelliteCommand.UpdateData();

            foreach (var satelliteCommand in satelliteCommands) {
                satelliteCommand.UpdateText();
                satelliteCommand.UpdateCmd();
            }*/
        }
    }
}
