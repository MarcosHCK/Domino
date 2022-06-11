public interface IMoverTurno
{
    string SgteJugador(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano, List<string> jugadores, List<Equipo> equipos);
}