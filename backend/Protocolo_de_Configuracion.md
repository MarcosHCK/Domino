## Descripcion
El juego consta de una serie de reglas configurables. A continuacion las listare todas y la forma en que el desarrollador del Frontend debe escribir los txt en la carpeta Configurado para obtener un juego configurado con las caracteristicas requeridas.
En un inicio la carpeta configurado contiene el juego Usual, pero el usuario del codigo puede reescribir los txt que considere en dependencia de las caracteristicas del juego que se desee llevar a cabo
A continuacion, listo cada uno de los txt alli presentes, cada uno que se refiere a una propiedad diferente del juego que puede ser configurada

## Numeros.txt
En este txt usted debera introducir tres datos, en la primera linea el tope de la data, en la segunda la cantidad de cabezas por ficha y en la tercera la cantidad de fichas por mano. Sencillo verdad?

## Puntuador.txt
En este txt usted debera escribir en tres lineas el indice del criterio bajo el cual usted desea que se puntue una ficha, un jugador, o un equipo respectivamente.
En la primera linea escoja entre:

    1- Puntuar la ficha por la suma de sus caras
    2- Puntuar la ficha por la cara mas pesada 
En la segunda linea escoja entre:

    1- Puntuar la mano por la suma de sus fichas
    2- Puntuar la mano por su cantidad de fichas
En la tercera linea escoja entre:

    1- Puntuar a un equipo por la suma de sus miembros
    2- Puntuar a un equipo por aquel de sus miembros que mas bajo este 

## Creador.txt
En este txt se configuran las fichas que estaran en juego. En el futuro pondre mas creadores si quiero, como un creador de cartas y cosas asi, pero por ahora limitemonos al creador usual del domino.
Luego, lo unico que usted tiene que escribir en este txt son en la primera linea si desa que aparezcan dobles o no. Lo escribe con un simple Si o un simple No. En la segunda linea debe escribir la cantidad de veces que desea que aparezca cada ficha
#### Ejemplo
Si 

1

## Finisher.txt
El Finisher como su nombre indica es la parte del juego que se encarga de determinar cuando el juego acabo
Los predicados con los cual usted puede formar combinaciones que expresen estados en los que desea declarar el juego como terminado son:

    1- Tranque
    2- Pegada
    3- NoFichasFuera
    4- Al lider quedan menos de tres fichas
    5- Todos los dobles han sido jugados
    6- El jugador anterior se paso
    7- Han ocurrido tres pases consecutivos del mismo jugador desde la ultima vez que se repartio
    8- Se ha repartido tres veces consecutivas sin realizar jugada
Usted para configurar cuando termina el juego debera en cada linea del txt escribir una combinacion de predicados tal que provoquen la finalizacion del juego. Asi, comience cada linea con un 1 OBLIGATORIO, y luego separados por espacios coloque el indice de los predicados que conforman la combinacion, con valor negativo en caso que usted lo q desea expresar es la negacion de tal predicado
Cuando haya introduciod todas las combinaciones que finalizan el juego, escriba break
#### Ejemplo
1 1

1 2

break

Asi el juego termina cuando haya tranque o el jjuego termina cuando halla pegada

## Emparejador.txt
El emparejador es la parte del juego que se encarga de determina en cada momento del partido cual es el criterio a usar para decidir si una ficha puede ser colocada por un extremo de la mesa o no.
Los criterios que pueden ser usados son los sgtes:

(por ahora; crear nuevos criterios es tan sencillo como implementar una interfaz)

    1- Usual
    2- Puedes poner un doble cuando quieras
    3- Sucesor
    4- Doble Modular
    5- Cualquier ficha
Los predicados con los cual usted puede formar comibnaciones tales que expresen si el juego se encuentra en una situacion determinada o no son los sgtes:

    1- Siempre
    2- Tranque
    3- Al lider quedan menos de tres fichas
    4- Juego No Iniciado
    5- El jugador en turno es el mas atrasado
    6- El jugador en turno es el mas adelantado
    7- Este mismo jugador se acaba de pasar una vez despues de repartir
    8- Han ocurrido dos pases consecutivos de un mismo jugador despues de repartir
    9- Han ocurrido tres pases consecutivos de un mismo jugador despues de repartir
Usted en el txt en cada linea debera introducir primero el criterio para emparejar a ser usado y luego en esa misma linea la combiunacion de predicados que da lugar al uso de ese criterio. Si en tal combinacion un predicado debe cumplirse usted coloca su indice positivo, de otra forma, su indice negativo.
Luego de haber introducido para cada situacion de juego el emparejador a usar, escriba break
##### Ejemplo
1 1

break

En este ejemplo el criterio 1 (El usual de que la cabeza de la ficha usada debe valer lo mismo que la cara de la mesa por donde es colocada) es usado cuando se cumple el predicado 1, o sea Siempre.

#### Otro Ejemplo
1 -7

3 7 -8

4 8 -9

break

En este caso el criterio emparejador usual se usa solo cuando el jugado en turno no se paso en el turno inmediato anterior; el criterio emparejador Sucesor(por una cara de la mesa solo puede seer colocada una ficha usando una cabeza que sea el sucesor natural de tal cara de la mesa) es usado cuando el jugador se acaba de pasar una pero no dos veces, mientras que el criterio emparejador Doble(la ficha a colocar debe usar una cabeza que sea el doble modulo data_tope del valor de la cara de la mesa por donde es colocada)

## Validador.txt
El Validador es la parte del juego que dada una situacion de juego determinada se encarga de emitir un criterio sobre si una jugada es valida o no, independientemente de que la ficha jugada pueda ser colocada por una cara determinada de la mesa.
Los criterios que pueden ser usados son los sgtes:

    1- Usual
    2- Tu ficha debe tener paridad diferente a la ficha jugada anteriormente
    3- La ficha jugada debe puntuar mas alto que la jugada anteriormente
    4- La ficha jugada debe puntuar mas bajo que la jugada anteriormente
A continuacion, le presentamos los predicados que podran ser usados para formar combinaciones que expresen el estado del juego y por tanto el criterio validador a usar:

    1- Siempre
    2- Tranque
    3- NoFichasFuera
    4- Al lider quedan menos de tres fichas
    5- Todos los dobles han sido jugados
    6- Juega el jugador mas atrasado
    7- Juega el jugador mas adelantado
En cada linea del txt el implementador del frontend debera escribir primeramente el indice de un criterio y luego una combinacion de predicados que llevan a la seleccion de tal criterio
Finalmente break
#### Por ejemplo
1 -5

2 5

break

Expresaria que en caso de que no se hayan jugado todos los dobles, una jugada sera valida si cumple con el criterio emparejador; mientras que si ya se jugaron todos los dobles, una jugada sera valida si la ficha en cuestion tiene diferente paridad de la ultima ficha jugada.

## MoverTurno.txt
MoverTurno se encarga de determinar en cada estado del juego a quien le toca el sgte turno
Estos son los criterios entre los que usted puede escoger para determinar a quien toca el sgte turno en un estado determinado del juego:

    1- Mover a la derecha
    2- Le toca al mismo jugador
    3- Le toca al jugador que mas fichas tenga y que tenga jugada
    4- Si la suma de las fichas de la mesa es impar le toca al de la izquierda, sino al de la derecha
Estos son los predicados con cuyas combinaciones usted puede expresar el estado del juego en el que desea aplicar un determinado criterion para determinar el sgte jugador:

    1- Siempre
    2- El juego no ha iniciado
    3- No quedan fichas fuera
    4- Al lider le quedan menos de tres fichas
    5- Todos los dobles han sido jugados
    6- La ultima jugada fue un pase
    7- El mismo jugador se ha pasado dos veces consecutivos
No olvide el break al final
#### Ejemplo
1 2

1 -6

1 3

2 -3 6

break

Este es el MoverTurno del Burrito y quiere decir que Si el juego no ha iniciado se mueva turno de la forma usual, si la ultima jugada no fue un pase tambien, asi como si no quedan fichas fuera.(Notar que la forma usual es hacia la derecha). Mientras, si quedan fichas afuera y la jugada anterior fue un pase le toca al mismo jugador que estaba en turno

## Cambiador
No existe ningun txt con este nombre, pero vale la pena presentar la forma en que debe ser introducido un cambiador para luego explicar su uso en el Repartidor.txt y el Refrescador.txt
El cambiador es una estructura que expresa como realizar un intercambio de fichas. Hay dos tipos, el tipo de cambiador que obliga al jugador que lo realiza a quedarse con una cantidad fija de fichas y el tipo que lo obliga a realizar una cantidad determinada de descartes, pudiendo realizar quizas algunos mas, pero forzando el balance en cuanto a fichas ganadas en la mano del jugador antes y despues de la operacion.
Para construir un cambiador usted debe en el txt donde se necesite tal cambiador, escribir en cada linea un cambiador, primeramente usando el criterio de Intercambio que usara tal cambiador y luego introduciendo la cantidad de descartes obligatorios y cantidad de descartes permitidos, o la cantidad de fichas fijas.
#### Por Ejemplo
1 0 0 1

expresa un cambiador que usa el criterio numero 1 de los expresados en el archivo donde este cambiador es requerido, y tal cambiador ademas cumple que no se pueden descartare fichas, o sea no hay descartes obligatorios ni permitidos; y el balance de la operacion tiene q ser de 1 ficha mas en la mano tras concluir el intercambio

4 2 3 -1

expresa un cambiador que usa el criterio 4 del txt donde es requerido, obligando a descartar dos fichas permitiendo descartar hasta tres y forzando que tras la operacion el jugador debe tener una ficha menos en la mano.

1 8

expresa un cambiador que usa el criterio de Intercambio 1 de el txt dond es requerido y fuerza que tras la operacion de intercambiar fichas, al jugador que la realizo debe quedarle una sola

## Refrescador.txt
Para expresar la accion de refrescar existen los cambiadores, pero a lo largo del partido podemos querer usar diferentes cambiadores. Por ello, en este txt primero usted debe introducir todos los cambiadores que usara, que podran ampararse en los sgtes criterios de Intercambio:

    1- Azar
    2- Una ficha es reemplazada por otra que valga el doble de ser posible 
    3- Una ficha es reemplazada por dos que sumen lo que ella de ser posible
Los predicados con los que usted podra armar las combinaciones que expresen cuando usar cada cambiador son los sgtes:

    1- Nunca
    2- El jugador anterior se paso
    3- Al lider le quedan menos de tres fichas
    4- Le toca al jugador mas atrasado
    5- Le toca al jugador mas adelantado
    6- No quedan fichas fuera
    7- Este mismo jugador se acaba de pasar una vez despues de repartir
    8- Han ocurrido dos pases consecutivos de un mismo jugador despues de repartir
    9- Han ocurrido tres pases consecutivos de un mismo jugador despues de repartir
Luego de haber introducido los cambiadores y haber colocado un break, usted debera introducir en cada una de las sgtes lineas primeramente el numero correspondiente al cambiador a usar y luego la combinacion de predicados que expresa el momento del juego en que usted desea usarlo. Finalmente introduzca break.
#### Ejemplo
1 0 0 1

3 1 1 1

break

1 2 -6 -8

2 2 -6 -9

break

Este es el refrescador del Camaron
Usa dos cambiadores, uno que obliga al jugador en turno a robar una ficha y otro que obliga al jugador en turno a descartar una ficha, mientras le son devueltas dos que sumen lo mismo que el valor de la ficha que descarto, o sea, el tercer criterio de intercambio.
El primer cambiador lo usa mientras haya ocurrido un pase y queden fichas fuera pero no hallan ocurrido dos pases consecutivos del mismo jugador
El segundo se usa cuando hayan ocurrido dos pases consecutivos del mismo jugador desde la ultima vez que se repartio, pero no tres

## Repartidor.txt
La accion de repartir fichas no es otra sino la aplicacion a cada jugador de dos acciones, una donde descartan fichas y la otra donde roban fichas. Asi una forma de describir una reparticion es mediante dos cambiadores, uno que se encarga de la parte en que cada juagdor descarta fichas y otro que se encarga de la parte en que cada jugador roba fichas
En este archivo usted debera primeramente armar las parejas de cambiadores que expresan la accion de repartir los cambiadores. Para esto puede seleccionar los sgtes criterios de intercambio:

    1- Azar
    2- La mano a entregar al jugador tendra un valor lo mas cercano posible a un valor dado
    3- La mano que se le otorgara al jugador tendra la mayor variedad de datas posibles
Luego de haber armado todas las parejas de cambiadores a usar, usted debe armar las combinaciones que expresan los momentos del juego en que se debe repartir y la pareja a usar en cada momento. Los predicados entre los que usted puede armar las combinaciones que describan cada circunstancia que usted desee del juego son:

    1- No se ha repartido aun
    2- Tranque
    3- Al lider le quedan menos de tres fichas
    4- Han ocurrido tres pases consecutivos de un mismo jugador despues de repartir
El formato sera:
Primeramente introduzca una a una las parejas de cambiadores.
Para ello sencillamente introduzca los dos cambiadores que la conforman de manera consecutiva
Luego escriba break.
Luego en cada linea coloque el indice de una pareja y luego la combiancion de predicados que da lugar al uso de tal pareja
Finalmente escriba break

## Por Ejemplo
1 0

1 8

1 0

1 -1

break

1 1

2 4

break

Aqui se expresan dos parejas de cambiadores
En la primera tenemos un cambiador que fuerza a todos los jugadores a quedarse con 0 fichas
y otro que los fuerza a todos a robar hasta tener 8
En ambos casos el criterio empleado para sustituir una ficha por otra o robar es el Azar.
En la segunda pareja el primer cambiador fuerza a todos los jugadores a quedarse con 0 fichas
mientras el segundo los fuerza a tobar hasta tener tantas fichas como tenian antes de comenzar la reparticion, (NOTAR EL -1)
Luego un break
Debajo tenemos la situacion de juego en que usaremos cada pareja para realizar la reparticion
La pareja uno la usaremos si no se ha repartido aun
La pareja dos la usaremos si han ocurrido tres pases consecutivos del mismo jugador.