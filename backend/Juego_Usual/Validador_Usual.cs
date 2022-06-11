public class Validador_Usual : IValidador
{
    public bool EsValida(Jugada jugada, Estado estado, List<Equipo> equipos, List<Ficha> mano)
    {
        if(!mano.Contains(jugada.ficha))return false;
        return (jugada.cara_de_la_mesa == jugada.cabeza_usada);
    }
}