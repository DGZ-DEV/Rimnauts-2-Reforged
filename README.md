# RimNauts 2 — Reforged (RimWorld 1.6)

Port a **RimWorld 1.6** de **RimNauts 2**, el mod de Sindre Eiklid que anade asteroides,
lunas, satelites y planetas al mapa del mundo.

El original se quedo en 1.5. Este port lo pone a funcionar en 1.6 **junto con Universum**,
que es su dependencia y que tiene su propio port.

## Requisitos

| Mod | Por que |
|---|---|
| **Harmony** | Lo necesitan los dos |
| **Universum** (port a 1.6) | Es la dependencia: sin el, RimNauts 2 no carga |

Tambien requiere los DLC de RimWorld que ya usaras normalmente.

## Que se arreglo respecto al original de 1.5

- **El generador coloca sus objetos.** En 1.6, heredar de `WorldGenStep` no basta: hace falta
  el def que registra la clase **y** que la capa lo enumere. Faltaba lo segundo, asi que el
  generador existia y **nadie lo llamaba nunca**: el mundo se veia normal y sin asteroides,
  sin un solo error que lo delatara.
- **El pod del jugador ya se puede usar.** Le faltaba su componente de combustible; sin el,
  el juego no podia evaluar si tenia carburante y la interfaz no mostraba **ningun boton**.
- **Las texturas cargan.** El mod reparte su contenido entre la raiz y una carpeta de version,
  y el mapa de carpetas hay que declararlo entero.
- **Un campo de definicion** que 1.6 ya no tiene (`causesNeed`) y un aviso del relic de Ideology.
- **Las mallas de los objetos celestes** ahora llevan coordenadas de textura, que antes no se
  generaban: el juego se negaba a dibujarlas.

## Limitaciones conocidas

Se documentan en vez de esconderse:

- **El efecto de ver el planeta en el cielo** ya no se dibuja. En 1.6 la geometria que lo
  alimentaba no existe; el framework ahora lo detecta y no intenta pintarlo, en lugar de
  llenar el registro de avisos.
- **Viajar a los asteroides del anillo** todavia no es posible: el lanzamiento del juego solo
  acepta destinos que ya tengan mapa generado, y los asteroides no lo tienen. Los planetas y
  satelites si funcionan.
- Queda retirado, con aviso, un parche de capas del framework pendiente de reescribir.

## Instalacion

1. Copia `Rimnauts 2 (Reforged)` y `Universum (Reforged)` a la carpeta `Mods` de RimWorld.
2. Activa **Harmony**, **Universum** y **RimNauts 2** en el menu de mods, en ese orden.
3. Genera un mundo nuevo: los objetos se colocan al generarlo.

## Creditos y licencia

**RimNauts 2** es obra de **Sindre Eiklid** y se distribuye bajo **licencia MIT**, que se
conserva en este repositorio junto a su aviso de copyright original.

**Universum**, su dependencia, es de **Rimworld: Space Project**, tambien MIT, y tiene su
propio repositorio de port.

Este port mantiene la autoria original intacta. El `README` original del autor se conserva
como `README.original.md`.
