## Descripcion
En una consola la parte Logica imprime todo lo importante para la representacion visual del juego, asi como recibe todos los datos necesarios en caso de haber jugadores humanos.

## Acciones
Toda accion comienza escribiendo el tipo de accion que es
Cuando se reparten fichas se imprime en pantalla Reparticion
De otra forma, luego de su tipo, toda accion comienza imprimiendo el nombre del jugador que la llevo a cabo 

### Intercambio de Fichas
Seran representados de la sgte forma
Intercambio Yosvany -5 +7
significa que el jugador Yosvany descarto 5 fichas y robo 7

### Jugadas
Si ocurre un pase aparecera esto:
Jugada Yuniela pase 
significa que el jugador Yuniela se paso
De otra forma aparecera esto
Jugada Yuniela ( 2, 3 ) 3 5 6
Que quiere decir que Yuniela coloco la ficha 3:2 usando la cabeza 3 por la cara de la mesa donde aparece un 5, quedandole seis fichas en la mano. Notar que el formato de la fichas es ( c1, c2, .... , cn ) donde la ficha puede tener hasta n cabezas.
Al final de la jugada puse la cantidad de fichas con la que quedo ekl jugador para que el implementador del frontend no tenga que llevar por su cuenta cuantas fichas tiene cada jugador, sino que le baste tener almacenadas las jugadas y revisar cuantas jugadas tenia el jugador la ultima vez que jugo

## Transcurso de los turnos

### Jugador Virtual
Al comenzar un turno se imprime Turno seguido del nombre del jugador en turno. A continuacion se imprime Mano FUlano, aclarando que se imprimira la mano de Fulano. Luego entre saltos de linea se imprime cada ficha en la mano del jugador en turno. Al terminar de imprimir las fichas se coloca en pantalla break. Luego se imprimen las caras activas en la mesa. Luego nuevamente un salto de linea. Ahora, cuantas veces toque refrescar o repartir fichas, se imprime la accion y nuevamente la mano del jugador en turno. Si el jugador en turno es virtual, se mostraran todas las acciones que realice, para luego imprimirse en pantalla EndTurn.
#### Por ejemplo:
Turno Yusimy
Mano Yusimy
( 1, 1 )
( 7, 3 )
( 4, 0 )
break
Caras: 8 8
Jugada Yusimy se pasa
EndTurn

#### Otro ejemplo:
Turno El_Brayan
Mano El_Brayan
( 2, 2 )
( 3, 3 )
break
Caras: 6 9 5 
Reparticion
Intercambio El_Brayan -2
Intercambio Yuniela -2
Intercambio Yusimy -3
Intercambio Yosvany -4
Intercambio El_Brayan +2
Intercambio Yuniela +2
Intercambio Yusimy +3
Intercambio Yosvany +4
Mano El_Brayan
( 3, 3 )
( 7, 7 )
break
Reparticion
Intercambio El_Brayan -2
Intercambio Yuniela -2
Intercambio Yusimy -3
Intercambio Yosvany -4
Intercambio El_Brayan +2
Intercambio Yuniela +2
Intercambio Yusimy +3
Intercambio Yosvany +4
Mano El_Brayan
( 4, 3 )
( 4, 5 )
break
Intercambio EL_Brayan -1 +2
Mano El_Brayan
( 4, 3 )
( 0, 3 )
( 1, 5 )
Jugada Yosvany (1, 5) 5 5 2
EndTurn

### Jugador Humano
En el caso del jugador humano el proceder es similar, pero la ejecucion del programa se detendra a la espera de que el humano decida las fichas que desea descartar, o la jugada que a realizar.
En el proceso de intercambio de fichas que involucre al jugador humano se imprimira en pantalla el numero de descartes obligatorios y el numero de descartes permitidos, de la forma que vera usted en el ejemplo. El Sistema esperara a que le pasen en una linea todos los descartes. Del implementador del frontend depende que la cantidad de descartes sea correcta, por algo le pase los datos.
Cuando el Sistema este esperando por las fichas a descartar imprimira Fichas_a_Descartar:
Cuando el Sistema este esperando por una ficha para realizar una jugada imprimira Ficha_a_Jugar:
Cuando el Sistema este esperando por el valor de la cabeza de la ficha que se usara para la jugada imprimira Cabeza:
Cuando el Sistema este esperando por el valor de la cara de la mesa donde se realizara la jugada imprimira Cara:
En caso de introducirse una jugada no valida, imprimira Jugada No Valida y volvera a pedir los datos
Los datos de cual ficha usar, cual cabeza de esa ficha, y cual cara de la mesa sera muy sencillo para el que programe el frontend evitar que sean introducidos datos incorrectos, por tal razon no cubro algun Out_of_Range Exception que pueda surgir de que se introduzca un indice de ficha q no es, o una cara de la mesa que no existe. Asumo la responsabilidad de si una jugada no es valida volver a solicitar los datos again. Notar que tras cada aparicion del : el programa se detiene a esperar que un dato sea introducido
#### Por Ejemplo
Turno Davier
Mano Davier
( 5, 4 )
( 9, 9 )
( 9, 0 )
( 6, 6 )
break
Caras: 2 4
Reparticion
Intercambio Yusimy -3
Descartes Obligatorios 4
Descartes Permitidos 4
Ficha_a_Descartar:                        //indices 0-indexed de las fichas a dscartar en la mano mostrada
Intercambio Davier -4
Intercambio Yosvany -1
Intercambio Yuniela -5
Intercambio Yusimy +3
Intercambio Davier +4
Intercambio Yosvany +1
Intercambio Yuniela +5
Mano Davier
( 2, 2 )
( 1, 7 )
( 1, 0 )
( 5, 7 )
break
Descartes Obligatorios 1
Descartes Permitidos 1
Ficha_a_Descartar:
Intercambio Davier -1 +2
Mano Davier
( 2, 2 )
( 1, 0 )
( 5, 7 )
( 3, 0 )
( 1, 4 )
break
Ficha_a_Jugar:                          //indice 0-indexed de las fichas a decartar en la mano mostrada
Cabeza:                                 //valor de la cabeza de la ficha usada
Cara:                                   //valor de la cara de la mesa a usar
Jugada No Valida
Ficha_a_Jugar:
Cabeza:
Cara:
Davier ( 1, 4 ) 4 4 4                   //hipotetica jugada correcta
EndTurn

####Otro Ejemplo:
Turno Davier
Mano Davier
( 2, 8 )
( 1, 5 )
break
Caras: 3 3
Intercambio Davier +1
Mano Davier
( 2, 8 )
( 1, 5 )
( 0, 0 )
break
Jugada Davier pase
EndTurn

En este ejemplo no hubo que esperar ningun dato ya que el jugador no debio descartar fichas ni tenia jugada posible a realizar
AL final del juego aparecen las puntuaciones
Al pricipio del juego aparecen los equipos