public class Mover_Turno_Usual : IMoverTurno
{
    int index;
    public Mover_Turno_Usual()
    {
        this.index = 0;
    }
    public string SgteJugador(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano, List<string> jugadores, List<Equipo> equipos)
    {
        index = (index + 1)%jugadores.Count;
        return jugadores[index];
    } 
}