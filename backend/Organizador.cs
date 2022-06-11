public class Organizador
{
    Dictionary<string, Jugador> _jugadores;
    List<string> orden;
    public bool HayEquipos{get; private set;}
    List<Equipo> _equipos;
    public Organizador(List<Jugador> jugadores, List<Equipo> _equipos, IOrdenador ordenador)
    {
        orden = ordenador.Ordenar(_equipos, jugadores.Count);
        _jugadores = new Dictionary<string, Jugador>();
        foreach(Jugador jugador in jugadores)
            _jugadores.Add(jugador.nombre, jugador);
        HayEquipos = (_equipos != null);
        this._equipos = _equipos;
    }
    public List<string> jugadores
    {
        get
        {
            return new List<string>(orden);
        }
    }
    public List<Equipo> equipos
    {
        get
        {
            List<Equipo> retorno = new List<Equipo>();
            foreach (Equipo equipo in this._equipos)
                retorno.Add((Equipo)equipo.Clone());
            return retorno;
        }
    }
    public Jugador this[string nombre]
    {
        get
        {
            return _jugadores[nombre];
        }
    }
}