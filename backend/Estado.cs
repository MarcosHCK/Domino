public class Estado
{
    List<Jugada> _jugadas;
    List<int> _caras_de_la_mesa;
    ICriterio_de_cambio _Criterio_De_Cambio_Refrescar;
    ICriterio_de_cambio _Criterio_De_Cambio_Repartir;
    public string Jugador_en_Turno{get; private set;}
    public int num_de_fichas_fuera;
    public Estado(Reglas_del_Juego reglas, List<string> jugadores, List<Equipo> equipos)
    {
        this._jugadas = new List<Jugada>();
        this._caras_de_la_mesa = new List<int>();
        this.PasarTurno(reglas, null, jugadores, equipos);
    }
    public Estado(Estado otro)
    {
        this._jugadas = otro.jugadas;
        this._caras_de_la_mesa = otro.caras_de_la_mesa;
        this._Criterio_De_Cambio_Refrescar = otro.Criterio_De_Cambio_Refrescar;
        this._Criterio_De_Cambio_Repartir = otro.Criterio_De_Cambio_Repartir;
    }
    public List<Jugada> jugadas
    {
        get
        {
            return new List<Jugada>(this._jugadas);
        }
    }
    public List<int> caras_de_la_mesa
    {
        get
        {
            return new List<int>(_caras_de_la_mesa);
        }
    }
    public ICriterio_de_cambio Criterio_De_Cambio_Repartir
    {
        get
        {
            return _Criterio_De_Cambio_Repartir;
        }
    }
    public ICriterio_de_cambio Criterio_De_Cambio_Refrescar
    {
        get
        {
            return _Criterio_De_Cambio_Refrescar;
        }
    }
    public void Actualizar(Jugada jugada)
    {
        this._jugadas.Add(jugada);
        if(jugada.EsPase)return;
        if(this.jugadas.Count > 1)this._caras_de_la_mesa.Remove(jugada.cara_de_la_mesa);
        foreach (int cabeza in jugada.ficha.cabezas)
            this._caras_de_la_mesa.Add(cabeza);
        if(this.jugadas.Count > 1)this._caras_de_la_mesa.Remove(jugada.cabeza_usada);
    }
    public void PasarTurno(Reglas_del_Juego reglas, List<Ficha> mano, List<string> jugadores, List<Equipo> equipos)
    {
        this.Jugador_en_Turno = reglas.MoveNext(this, mano, jugadores, equipos);
        this._Criterio_De_Cambio_Refrescar = reglas.Get_Criterios_de_Refrescar(this, mano);
        this._Criterio_De_Cambio_Repartir = reglas.Get_Criterios_de_Repartir(this, mano);
    }
}
