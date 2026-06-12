# Resultados experimentales

Partidas ejecutadas en Unity contra el mismo agente aleatorio.
Cada heurística juega dos veces con negras y dos con blancas.
Las semillas se fijan para que la prueba sea reproducible.

| Heurística | Partida | Color | Semilla | Resultado | Fichas IA | Fichas rival |
|---|---:|---|---:|---|---:|---:|
| H1 | 1 | Negras | 1000 | Victoria | 50 | 14 |
| H1 | 2 | Negras | 1001 | Victoria | 48 | 16 |
| H1 | 3 | Blancas | 1002 | Victoria | 55 | 9 |
| H1 | 4 | Blancas | 1003 | Derrota | 21 | 43 |
| H2 | 1 | Negras | 2000 | Victoria | 47 | 17 |
| H2 | 2 | Negras | 2001 | Victoria | 40 | 24 |
| H2 | 3 | Blancas | 2002 | Derrota | 19 | 45 |
| H2 | 4 | Blancas | 2003 | Victoria | 53 | 8 |
| H3 | 1 | Negras | 3000 | Victoria | 49 | 15 |
| H3 | 2 | Negras | 3001 | Victoria | 55 | 9 |
| H3 | 3 | Blancas | 3002 | Victoria | 51 | 13 |
| H3 | 4 | Blancas | 3003 | Derrota | 23 | 41 |

| Heurística | Victorias | Fichas medias |
|---|---:|---:|
| H1 | 3 | 43.50 |
| H2 | 3 | 39.75 |
| H3 | 3 | 44.50 |

## Interpretación

Las tres heurísticas han conseguido 3 victorias de 4 partidas, así que en
victorias han obtenido el mismo resultado. La diferencia se ve en la media de
fichas. H3 ha conseguido la mejor media con 44,50 fichas, H1 se ha quedado cerca
con 43,50 y H2 ha obtenido la media más baja con 39,75.

H1 ha funcionado bastante bien contra el jugador aleatorio porque intenta tener
más fichas que el rival. El problema es que conseguir muchas fichas al principio
no siempre significa estar en una buena posición para el final de la partida.

H2 intenta que la IA tenga más movimientos disponibles y que el rival tenga
menos opciones. Esta estrategia puede ser útil para controlar la partida, pero
no tiene en cuenta directamente las fichas ni las casillas importantes. Esto
puede explicar que haya terminado con una media de fichas inferior.

H3 ha conseguido la mejor media porque da mucha importancia a las esquinas, que
son posiciones que ya no se pueden perder. También evita las casillas cercanas a
una esquina vacía, ya que pueden facilitar que el rival capture esa esquina.

En las tres heurísticas, la única derrota se ha producido jugando con blancas.
Sin embargo, como solo se han realizado cuatro partidas por heurística, no se
puede asegurar que jugar con blancas sea siempre peor. Para obtener conclusiones
más fiables sería necesario jugar más partidas.
