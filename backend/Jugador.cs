public abstract class Jugador
{
    public string nombre{get; private set;}
    protected Reglas_del_Juego reglas;
    IDescartador descartador;
    protected List<string> jugadores;
    protected List<Equipo> equipos;
    public Jugador(string nombre, IDescartador descartador = null)
    {
        this.nombre = nombre;
        this.descartador = descartador;
        if (this.descartador == null)this.descartador = new Descartador_Vacio();
    }
    public abstract Jugada Jugar(Estado estado, List<Ficha> mano);
    public void Iniciar(Reglas_del_Juego reglas, List<string> jugadores, List<Equipo> equipos)
    {
        this.reglas = reglas;
        this.jugadores = jugadores;
        this.equipos = equipos;
    }
    public List<Ficha> Descartar(ICriterio_de_cambio criterio, List<Ficha> mano, int cambios_restantes, int rondas_de_cambios_restantes)
    {//Esto es mejorable
        return descartador.Descartar(criterio, mano, cambios_restantes, rondas_de_cambios_restantes, reglas, this.Valorar_Datas);
    }
    protected virtual int[] Valorar_Datas
    {
        get
        {
            return new int[0];
        }
    }
}