public class Reglas_del_Juego
{
    public readonly int cabezas_por_ficha;
    public readonly int data_tope;
    public readonly int fichas_por_mano;
    ICreador Creador;
    DaCriterio<bool> Finisher;
    public readonly Puntuador Puntuador;
    DaCriterio<IEmparejador> Emparejador;
    DaCriterio<IValidador> Validador;
    DaCriterio<IMoverTurno> MoverTurno;
    MoverFichas MoverFichas;//Que este metodo, dado una lista de criterios de cambios, le asigne a las propiedades en Estado referidas al Refresque y a la Reparticion un ICriterio_de_Intercambio
    public Reglas_del_Juego(ICreador Creador, DaCriterio<bool> Finisher, Puntuador Puntuador,
    DaCriterio<IEmparejador> Emparejador, DaCriterio<IValidador> Validador, DaCriterio<IMoverTurno> MoverTurno, MoverFichas MoverFichas,
    int data_tope = 7, int fichas_por_mano = 7, int cabezas_por_ficha = 2)
    {
        this.cabezas_por_ficha = cabezas_por_ficha;
        this.data_tope = data_tope;
        this.fichas_por_mano = fichas_por_mano;
        this.Creador = Creador;
        this.Finisher = Finisher;
        this.Puntuador = Puntuador;
        this.Emparejador = Emparejador;
        this.Validador = Validador;
        this.MoverTurno = MoverTurno;
        this.MoverFichas = MoverFichas;
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
        return this.Finisher.Criterio(Estado, mano_del_ultimo_en_jugar);
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
    public bool EsValidoColocar(Estado estado, List<Ficha> mano, Ficha ficha, int cabeza_de_la_ficha, int cara_de_la_mesa)
    {
        return this.Emparejador.Criterio(estado, mano).EsValidoColocar(ficha, cabeza_de_la_ficha, cara_de_la_mesa);
    }
    public bool EsValida(Jugada jugada, Estado estado, List<Ficha> mano)
    {
        return 
        this.EsValidoColocar(estado, mano, jugada.ficha, jugada.cabeza_usada, jugada.cara_de_la_mesa)
                         &&
        this.Validador.Criterio(estado, mano).EsValida(jugada, this, estado, mano);
    }
    public string MoveNext(Estado estado, List<Ficha> mano)
    {
        return this.MoverTurno.Criterio(estado, mano).SgteJugador(this, estado, mano);
    }
    public Cambiador Get_Cambiador_de_Refrescar(Estado estado, List<Ficha> mano)
    {
        return this.MoverFichas.Get_Cambiador_de_Refrescar(estado, mano);
    }
    public Cambiador Get_Cambiador_de_Repartir(Estado estado, List<Ficha> mano)
    {
        return this.MoverFichas.Get_Cambiador_de_Repartir(estado, mano);
    }
}