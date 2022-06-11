public class Reglas_del_Juego
{
    public int cabezas_por_ficha{get; private set;}
    public int data_tope{get; private set;}
    public int fichas_por_mano{get; private set;}
    ICreador Creador;
    IGameOver _GameOver;
    IPuntuador Puntuador;
    IValidador Validador;
    IMoverTurno MoverTurno;
    IRefrescador Refrescador;//Que este metodo, dado una lista de criterios de cambios, le asigne a las propiedades en Estado referidas al Refresque y a la Reparticion un ICriterio_de_Cambio
    public Reglas_del_Juego(ICreador Creador, IGameOver _GameOver, IPuntuador Puntuador,
    IValidador Validador, IMoverTurno MoverTurno, IRefrescador Refrescador,
    int data_tope = 7, int fichas_por_mano = 7, int cabezas_por_ficha = 2)
    {
        this.cabezas_por_ficha = cabezas_por_ficha;
        this.data_tope = data_tope;
        this.fichas_por_mano = fichas_por_mano;
        this.Creador = Creador;
        this._GameOver = _GameOver;
        this.Puntuador = Puntuador;
        this.Validador = Validador;
        this.MoverTurno = MoverTurno;
        this.Refrescador = Refrescador;
    }
    public List<Ficha> fichas
    {
        get
        {
            return this.Creador.fichas(data_tope, cabezas_por_ficha);
        }
    }
    public int fichas_fuera(int cant_de_jugadores)
    {
        return this.Creador.cant_de_fichas(this.data_tope, this.cabezas_por_ficha);
    }
    public bool GameOver(Estado Estado, List<Ficha> mano_del_ultimo_en_jugar)
    {
        return this._GameOver.GameOver(Estado, mano_del_ultimo_en_jugar);
    }
    public int Puntuar (Ficha ficha)
    {
        return this.Puntuador.Puntuar(ficha);
    }
    public int Puntuar (List<Ficha> fichas)
    {
        return this.Puntuador.Puntuar(fichas);
    }
    public int Puntuar (Equipo equipo, Portal_del_Banquero banquero)
    {
        return this.Puntuador.Puntuar(equipo, banquero);
    }
    public bool EsValida(Jugada jugada, Estado estado, List<Ficha> mano)
    {
        return this.Validador.EsValida(jugada, estado, estado.equipos, mano);
    }
    public string MoveNext(Estado estado, List<Ficha> mano, List<string> jugadores, List<Equipo> equipos)
    {
        return this.MoverTurno.SgteJugador(this, estado, mano, jugadores, equipos);
    }
    public ICriterio_de_cambio Get_Criterios_de_Refrescar(Estado estado, List<Ficha> mano)
    {
        return this.Refrescador.Get_Criterios_de_Refrescar(this, estado, mano);
    }
    public ICriterio_de_cambio Get_Criterios_de_Repartir(Estado estado, List<Ficha> mano)
    {
        return this.Refrescador.Get_Criterios_de_Repartir(this, estado, mano);
    }
}