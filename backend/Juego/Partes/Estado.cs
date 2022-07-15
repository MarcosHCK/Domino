public class Estado
{
    List<Action> _acciones;
    List<int> _caras_de_la_mesa;
    Dictionary<string, int> _fichas_por_mano;
    List<Equipo> _equipos;
    List<string> _jugadores;
    public (Cambiador, Cambiador) Cambiadores_de_Repartir{get; private set;}
    public Cambiador Cambiador_de_Refrescar{get; private set;}
    public string Jugador_en_Turno{get; private set;}//Aqui las propiedades si deben ser get private set
    public bool YaSeHaJugado{get; private set;}
    public int fichas_fuera{get; private set;}
    public Estado(Reglas_del_Juego reglas, List<string> jugadores, List<Equipo> _equipos, int fichas_fuera)
    {
        this.YaSeHaJugado = false;
        this._acciones = new List<Action>();
        this._caras_de_la_mesa = new List<int>();
        this._equipos = _equipos;
        this.fichas_fuera = fichas_fuera;
        this._fichas_por_mano = new Dictionary<string, int>();
        this._jugadores = new List<string>(jugadores);
        foreach(string nombre in jugadores)this._fichas_por_mano.Add(nombre, 0);
        this.PasarTurno(reglas, null);
    }
    public Estado(Estado otro)
    {
        this.YaSeHaJugado = false;
        this._acciones = otro.acciones;
        this._caras_de_la_mesa = otro.caras_de_la_mesa;
        this.Jugador_en_Turno = new string(otro.Jugador_en_Turno);
        this._jugadores = otro.jugadores;
        this._equipos = otro.equipos;
        this.fichas_fuera = otro.fichas_fuera;
        this._fichas_por_mano = otro.fichas_por_mano;
        this.YaSeHaJugado = otro.YaSeHaJugado;
    }
    public List<Action> acciones
    {
        get
        {
            return new List<Action>(_acciones);
        }
    }
    public List<int> caras_de_la_mesa
    {
        get
        {
            return new List<int>(_caras_de_la_mesa);
        }
    }
    public List<string> jugadores
    {
        get
        {
            return new List<string>(_jugadores);
        }
    }
    public List<Equipo> equipos
    {
        get
        {
            return new List<Equipo>(_equipos);
        }
    }
    public Dictionary<string, int> fichas_por_mano
    {
        get
        {
            return new Dictionary<string, int>(this._fichas_por_mano);
        }
    }
    public void Actualizar(params Action[] hechos)
    {
        foreach(Action accion in hechos)
        {
            this._acciones.Add(accion);
            if(accion is Jugada)
            {
                Jugada jugada = (Jugada)accion;
                if(jugada.EsPase)return;
                this._fichas_por_mano[jugada.autor]--;
                if(this.YaSeHaJugado)this._caras_de_la_mesa.Remove(jugada.cara_de_la_mesa);
                foreach (int cabeza in jugada.ficha.cabezas)
                    this._caras_de_la_mesa.Add(cabeza);
                if(this.YaSeHaJugado)this._caras_de_la_mesa.Remove(jugada.cabeza_usada);
                this.YaSeHaJugado = true;
                return;
            }
            if(accion is Intercambio)
            {
                int balance = (((Intercambio)accion).fichas_tomadas - ((Intercambio)accion).fichas_devueltas);
                this.fichas_fuera -= balance;
                this._fichas_por_mano[accion.autor] += balance;
            }
        }
    }
    public void Actualizar_Cambiadores(Reglas_del_Juego reglas, List<Ficha> mano)
    {
        this.Cambiadores_de_Repartir = this.Get_Cambiadores_de_Repartir(reglas, mano);
        this.Cambiador_de_Refrescar = this.Get_Cambiador_de_Refrescar(reglas, mano);
    }
    public void PasarTurno(Reglas_del_Juego reglas, List<Ficha> mano)
    {
        this.Jugador_en_Turno = reglas.MoveNext(this, mano);
        this.Actualizar_Cambiadores(reglas, mano);
    }
    public Cambiador Get_Cambiador_de_Refrescar(Reglas_del_Juego reglas, List<Ficha> mano)
    {
        return reglas.Get_Cambiador_de_Refrescar(this, mano);
    }
    public (Cambiador, Cambiador) Get_Cambiadores_de_Repartir(Reglas_del_Juego reglas, List<Ficha> mano)
    {
        return reglas.Get_Cambiadores_de_Repartir(this, mano);
    }
    public IEnumerable<Action> Acciones_en_Reversa
    {
        get
        {
            if(this.YaSeHaJugado)
                foreach(Action accion in this._acciones.Reverse<Action>())
                    yield return accion;
            yield break;
        }
    }
    public IEnumerable<Jugada> Jugadas_en_Reversa
    {
        get
        {
            if(this.YaSeHaJugado)
                foreach(Action accion in this._acciones.Reverse<Action>())
                    if(accion is Jugada)yield return (Jugada)accion;
            yield break;
        }
    }
}