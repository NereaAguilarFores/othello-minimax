# Guion completo para el vídeo

Duración aproximada: 6 minutos.

No hace falta grabarlo todo seguido. Puedes grabar primero la pantalla en varios
clips y añadir la voz después. Los textos entre corchetes son instrucciones y no
se narran.

## Antes de grabar

1. Abre `SampleScene` en Unity.
2. Deja abierta también la consola, sin errores rojos.
3. Abre `Player.cs` en Visual Studio.
4. Aumenta el tamaño de letra para que el código se pueda leer.
5. Prepara `RESULTADOS_HEURISTICAS.md`.
6. Graba clips cortos de Unity usando H1, H2 y H3.

Para cambiar la heurística:

1. Detén el modo Play.
2. Selecciona `Player2` en la jerarquía.
3. En el componente `Player`, cambia `Heuristic Type`:
   - `1` para H1.
   - `2` para H2.
   - `3` para H3.
4. Pulsa Play y realiza varios movimientos.

No cambies el valor durante una partida.

---

## Escena 1. Presentación general

**Duración:** 35 segundos.

**En pantalla:** título “IA para Othello con Minimax” y después el tablero de
Unity funcionando.

**Voiceover:**

> En esta práctica hemos implementado una inteligencia artificial para jugar a
> Othello. El proyecto original ya permitía jugar, pero la máquina elegía sus
> movimientos de forma aleatoria. Nuestro objetivo ha sido sustituir esa
> decisión por el algoritmo Minimax.
>
> El agente analiza posibles movimientos hasta una profundidad de cuatro
> niveles. También hemos implementado tres heurísticas diferentes y la poda
> alfa-beta para reducir el número de ramas que se tienen que explorar.

**Rótulo recomendado:** `Minimax | Profundidad 4 | H1, H2 y H3 | Alfa-beta`

---

## Escena 2. Representación del árbol

**Duración:** 40 segundos.

**En pantalla:** `Player.cs`, líneas 5 a 25, mostrando la clase `Node`.

**Voiceover:**

> Cada nodo del árbol representa un posible estado del tablero. Dentro de la
> clase Node guardamos una copia del tablero, una referencia al padre, una lista
> de hijos, el tipo de nodo y su utilidad.
>
> La copia del tablero es importante porque cada rama tiene que simular sus
> movimientos sin modificar el tablero real ni las otras ramas.
>
> La raíz del árbol es un nodo MAX porque representa el turno de la inteligencia
> artificial. Sus hijos representan los movimientos que puede realizar.

**Señala en pantalla:** `board`, `parent`, `childList`, `type`, `utility`.

---

## Escena 3. Código principal de SelectTile

**Duración:** 55 segundos.

**En pantalla:** líneas 46 a 88 de `Player.cs`.

**Voiceover:**

> La función SelectTile recibe el tablero actual y tiene que devolver una
> posición válida. Primero creamos el nodo raíz y buscamos todos los movimientos
> disponibles para la IA.
>
> Por cada movimiento creamos un nodo hijo con una copia del tablero. Después
> aplicamos el movimiento sobre esa copia y llamamos a Minimax. Como a
> continuación juega el rival, el siguiente nodo será MIN.
>
> Guardamos la utilidad de cada opción y finalmente devolvemos el movimiento que
> tenga el valor más alto. De esta manera ya no se elige una casilla aleatoria.

**Señala en pantalla:** `FindSelectableTiles`, `new Node`, `Move`, llamada a
`Minimax` y el `return`.

---

## Escena 4. Funcionamiento de MIN y MAX

**Duración:** 55 segundos.

**En pantalla:** primero líneas 233 a 265 y después líneas 267 a 299.

**Voiceover:**

> Dentro de Minimax alternamos entre nodos MAX y MIN. En los nodos MAX suponemos
> que juega la IA, por lo que conservamos la utilidad más alta de todos los
> hijos.
>
> En los nodos MIN suponemos que el rival elegirá la respuesta más perjudicial
> para la IA. Por eso se conserva el valor más bajo.
>
> En cada llamada se cambia el turno y se reduce la profundidad. Cuando llegamos
> a profundidad cero, el tablero se evalúa con la heurística seleccionada y el
> resultado se propaga hacia la raíz.

**Rótulo recomendado:** `MAX = mayor valor` y después `MIN = menor valor`.

---

## Escena 5. Pase y final de partida

**Duración:** 35 segundos.

**En pantalla:** líneas 196 a 230.

**Voiceover:**

> También hemos tenido en cuenta los pases. Si un jugador no puede mover,
> comprobamos si el contrario tiene movimientos.
>
> Si el contrario sí puede jugar, creamos un hijo con el mismo tablero y
> cambiamos el turno. Si ninguno de los dos puede mover, la partida ha terminado
> y calculamos una utilidad definitiva según quién tenga más fichas.
>
> Las victorias reciben un valor muy alto y las derrotas uno muy bajo, para que
> siempre tengan prioridad sobre una valoración intermedia.

---

## Escena 6. Poda alfa-beta

**Duración:** 45 segundos.

**En pantalla:** líneas 192 a 196, 246 a 257 y 280 a 291.

**Voiceover:**

> Para optimizar la búsqueda hemos añadido poda alfa-beta. Alpha guarda el mejor
> valor que MAX ha encontrado hasta el momento y beta guarda el mejor valor para
> MIN.
>
> Después de evaluar cada hijo actualizamos uno de estos límites. Cuando beta es
> menor o igual que alpha, dejamos de explorar esa rama porque ya sabemos que no
> puede cambiar la decisión final.
>
> La poda reduce el trabajo del algoritmo, pero no modifica el movimiento que
> escogería Minimax sin poda.

**Señala en pantalla:** `alpha`, `beta` y los dos bloques `if (beta <= alpha)`.

---

## Escena 7. Heurística H1

**Duración:** 25 segundos.

**En pantalla:** líneas 105 a 112 y después un clip corto de Unity con
`Heuristic Type = 1`.

**Voiceover:**

> La primera heurística es la más básica. Calcula la diferencia entre las fichas
> de la IA y las del rival. Su ventaja es que es sencilla y favorece conseguir
> más fichas, aunque al principio de la partida una ventaja inmediata no siempre
> produce una buena posición final.

**Rótulo:** `H1 = fichas IA - fichas rival`

---

## Escena 8. Heurística H2

**Duración:** 25 segundos.

**En pantalla:** líneas 114 a 121 y después un clip con `Heuristic Type = 2`.

**Voiceover:**

> La segunda heurística utiliza la movilidad. Compara cuántos movimientos tiene
> disponibles la IA y cuántos tiene el rival. El objetivo es conservar opciones
> para los siguientes turnos y limitar las posibilidades del oponente.
>
> Esta heurística no tiene en cuenta directamente las fichas ni las posiciones
> del tablero.

**Rótulo:** `H2 = movimientos IA - movimientos rival`

---

## Escena 9. Heurística H3

**Duración:** 40 segundos.

**En pantalla:** líneas 123 a 170 y después un clip con `Heuristic Type = 3`.

**Voiceover:**

> La tercera heurística es posicional. Las esquinas tienen un valor muy alto
> porque, una vez capturadas, no se pueden perder.
>
> Mientras una esquina está vacía, también penalizamos las tres casillas que la
> rodean. Ocuparlas puede facilitar que el rival consiga esa esquina. Cuando la
> esquina ya está ocupada, esas casillas dejan de considerarse peligrosas.
>
> H3 utiliza la diferencia de fichas solo como desempate. Por eso no es una
> combinación lineal de H1 y H2, sino una estrategia distinta basada en la
> posición de las fichas.

**Rótulo:** `H3 = esquinas + riesgo posicional`

---

## Escena 10. Pruebas y resultados

**Duración:** 55 segundos.

**En pantalla:** `RESULTADOS_HEURISTICAS.md`, desplazándote primero por las
partidas y después por la tabla resumen.

**Voiceover:**

> Para comparar las heurísticas usamos siempre el mismo oponente de referencia:
> un agente aleatorio que elegía entre sus movimientos válidos.
>
> Cada heurística jugó cuatro partidas, dos como negras y dos como blancas.
> Utilizamos semillas registradas para que las pruebas se pudieran repetir.
> También comprobamos que todos los movimientos devueltos por la IA fueran
> válidos.
>
> Las tres heurísticas consiguieron tres victorias. H1 obtuvo una media de 43,50
> fichas, H2 obtuvo 39,75 y H3 consiguió la mejor media con 44,50.
>
> H2 tuvo una media inferior porque solo valora la movilidad. H3 consiguió el
> mejor resultado medio al tener en cuenta las esquinas y las posiciones
> peligrosas. De todas formas, como solo se han realizado cuatro partidas por
> heurística, harían falta más pruebas para sacar conclusiones más generales.

---

## Escena 11. Cierre y uso de herramientas

**Duración:** 30 segundos.

**En pantalla:** Unity funcionando, consola sin errores y título final.

**Voiceover:**

> Como resultado final, la máquina utiliza Minimax con profundidad cuatro,
> gestiona los pases, permite elegir entre tres heurísticas y aplica poda
> alfa-beta.
>
> Hemos utilizado ChatGPT como apoyo puntual para resolver algunas dudas y
> organizar la explicación del proyecto. La implementación, las pruebas y la
> revisión final las hemos realizado nosotras.

**Rótulo final:** `Othello Minimax - práctica finalizada`

---

## Consejos para que suene natural

- No intentes memorizar todo. Lee una o dos frases cada vez.
- Haz pausas entre secciones para poder cortar el audio.
- Cambia alguna palabra si no la dirías normalmente.
- Habla un poco más despacio de lo habitual.
- No muestres todo el código a la vez: acerca únicamente el fragmento explicado.
- Evita música alta; la voz debe entenderse con claridad.
- Comprueba antes de exportar que no aparece información personal en pantalla.
