public class Estado
{
    List<Action> _acciones;
    List<int> _caras_de_la_mesa;
    Dictionary<string, int> _fichas_por_mano;
    public ICriterio_de_cambio Criterio_De_Cambio_Refrescar{get; private set;}
    public ICriterio_de_cambio Criterio_De_Cambio_Repartir{get; private set;}
    public string Jugador_en_Turno{get; private set;}
    public bool YaSeHaJugado{get; private set;}
    public List<Equipo> _equipos;
    public int fichas_fuera{get; private set;}
    public Estado(Reglas_del_Juego reglas, List<string> jugadores, List<Equipo> _equipos, int fichas_fuera)
    {
        this.YaSeHaJugado = false;
        this._acciones = new List<Action>();
        this._caras_de_la_mesa = new List<int>();
        this._equipos = _equipos;
        this.fichas_fuera = fichas_fuera;
        this._fichas_por_mano = new Dictionary<string, int>();
        foreach(string nombre in jugadores)this._fichas_por_mano.Add(nombre, 0);
        this.PasarTurno(reglas, null, jugadores, equipos);
    }
    public Estado(Estado otro)
    {
        this.YaSeHaJugado = false;
        this._acciones = otro.acciones;
        this._caras_de_la_mesa = otro.caras_de_la_mesa;
        this.Criterio_De_Cambio_Refrescar = otro.Criterio_De_Cambio_Refrescar;
        this.Criterio_De_Cambio_Repartir = otro.Criterio_De_Cambio_Repartir;
        this.Jugador_en_Turno = otro.Jugador_en_Turno;
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
        foreach(Action accion in hechos){
            this._acciones.Add(accion);
            if(accion is Jugada)
            {
                Jugada jugada = (Jugada)accion;
                if(jugada.EsPase)return;
                if(this.YaSeHaJugado)this._caras_de_la_mesa.Remove(jugada.cara_de_la_mesa);
                foreach (int cabeza in jugada.ficha.cabezas)
                    this._caras_de_la_mesa.Add(cabeza);
                if(this.YaSeHaJugado)this._caras_de_la_mesa.Remove(jugada.cabeza_usada);
                this.YaSeHaJugado = true;
                return;
            }
            int balance = ((Intercambio)accion).fichas_devueltas - ((Intercambio)accion).fichas_tomadas;
            this.fichas_fuera += balance;
            this._fichas_por_mano[accion.autor] += balance;
        }
    }
    public void PasarTurno(Reglas_del_Juego reglas, List<Ficha> mano, List<string> jugadores, List<Equipo> equipos)
    {
        this.Jugador_en_Turno = reglas.MoveNext(this, mano, jugadores, equipos);
        this.Actualizar_Criterios_de_Cambio(reglas, mano);
    }
    public void Actualizar_Criterios_de_Cambio(Reglas_del_Juego reglas, List<Ficha> mano)
    {
        this.Criterio_De_Cambio_Refrescar = reglas.Get_Criterios_de_Refrescar(this, mano);
        this.Criterio_De_Cambio_Repartir = reglas.Get_Criterios_de_Repartir(this, mano);
    }
}