using System.Reflection;
using Verse;

namespace RimNauts2 {
    [StaticConstructorOnStartup]
    public static class RimNauts2 {
        static RimNauts2() {
            // PORT 1.6: todo el parcheo va en un try/catch. Si un parche falla (una firma de 1.5,
            // por ejemplo), Harmony lanza y ANTES se llevaba por delante el resto del constructor:
            // ni ajustes, ni definiciones, ni assets. Asi el mod carga igual y el fallo se registra.
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("sindre0830.rimnauts2");
            try {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            } catch (System.Exception e) {
                Verse.Log.Error("[RimNauts2 PORT 1.6] Fallo aplicando parches. El mod SIGUE cargando, pero puede quedar incompleto: " + e.Message);
            }
            int __parcheados = 0;
            try { foreach (System.Reflection.MethodBase __m in harmony.GetPatchedMethods()) { __parcheados++; } } catch (System.Exception) { }
            Verse.Log.Message("[RimNauts2 PORT 1.6] Harmony ha parcheado " + __parcheados + " metodos de este mod.");
            // print mod info
            Logger.print(
                Logger.Importance.Info,
                key: "RimNauts.Info.mod_loaded",
                args: new NamedArgument[] { RimNauts2_ModContent.instance.Content.ModMetaData.Name, RimNauts2_ModContent.instance.Content.ModMetaData.ModVersion }
            );
            Defs.Loader.init();
        }
    }

    public class RimNauts2_ModContent : Mod {
        public static RimNauts2_ModContent instance { get; private set; }

        public RimNauts2_ModContent(ModContentPack content) : base(content) {
            instance = this;
        }
    }
}
