//El unico proposito de esta interface es representar el criterio por el cual es ordenado el nombre de los jugadores en la lista de participantes, lo cual luego puede influir gruesamente (o no) en el orden de los turnos
public interface IOrdenador
{
    List<string> Ordenar(List<Equipo> equipos, int cant_de_jugadores);
}