public class Organizador
{
    Dictionary<string, Jugador> _jugadores;
    List<string> orden;
    List<Equipo> _equipos;
    public Organizador(List<Jugador> jugadores, IOrdenador ordenador, List<Equipo> _equipos)
    {
        if(_equipos.Count == 0)
        {
            _equipos = new List<Equipo>();
            foreach(Jugador jugador in jugadores)
                _equipos.Add(new Equipo(jugador.nombre, jugador.nombre));
        }
        orden = ordenador.Ordenar(_equipos, jugadores.Count);
        _jugadores = new Dictionary<string, Jugador>();
        foreach(Jugador jugador in jugadores)
            _jugadores.Add(jugador.nombre, jugador);
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
            return new List<Equipo>(_equipos);
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