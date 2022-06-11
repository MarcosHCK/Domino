public interface IValidador
{
    bool EsValida(Jugada jugada, Estado estado, List<Equipo> equipos, List<Ficha> mano);
}