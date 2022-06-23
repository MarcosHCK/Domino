public abstract class Jugador_Virtual : Jugador
{
    IDescartador descartador;
    public Jugador_Virtual(string nombre, IDescartador descartador = null) : base(nombre)
    {
        this.descartador = descartador;
        if (this.descartador == null)this.descartador = new Descartador_Random();
    }
    public override Jugada Jugar(Estado estado, List<Ficha> mano)
    {
        if(!estado.YaSeHaJugado)return this.Apertura(mano);
        (Jugada?, double) MejorJugada = (null, 0);
        (Jugada, double) Actual;
        foreach(Jugada jugada in Util.PosiblesJugadas(estado, mano))
            if(this.reglas.EsValida(jugada, estado, mano))
            {
                Actual = (jugada, this.Valorar(jugada, estado, mano));
                if((MejorJugada.Item1 == null) || (Actual.Item2 > MejorJugada.Item2))
                   MejorJugada = Actual;
            }
        if(MejorJugada.Item1 == null)return new Jugada(this.nombre);
        return MejorJugada.Item1;
    }
    public override List<Ficha> Descartar(Cambiador Cambiador, Estado Estado, List<Ficha> mano)
    {
        return descartador.Descartar(Cambiador, Estado, mano, reglas, this.Valorar_Datas());
    }
    protected abstract double Valorar(Jugada jugada, Estado estado, List<Ficha> mano);
    protected abstract double[] Valorar_Datas();
    protected abstract Jugada Apertura(List<Ficha> mano);
}