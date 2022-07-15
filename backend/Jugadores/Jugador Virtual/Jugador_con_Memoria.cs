public abstract class Jugador_con_Memoria : Jugador_Virtual
{
    protected Memoria memoria;
    int index_of_actualization;
    public Jugador_con_Memoria(string nombre, Memoria memoria, IDescartador descartador = null)
    : base(nombre, descartador)
    {
        this.memoria = memoria;
        this.index_of_actualization = 0;
    }
    public override Jugada Jugar(Estado estado, List<Ficha> mano)
    {
        memoria.Actualizar(estado, mano, index_of_actualization);
        index_of_actualization = estado.acciones.Count();
        if(!estado.YaSeHaJugado)return this.Apertura(mano);
        (Jugada?, double) MejorJugada = (null, 0);
        (Jugada, double) Actual;
        foreach(Jugada jugada in Util.PosiblesJugadas(estado, mano))
            if(this.reglas.EsValida(jugada, estado, mano))
            {
                Actual = (jugada, this.Valorar(jugada, new Estado(estado), mano));//para que asi pueda modificar el estado a su gusto 
                if((MejorJugada.Item1 == null) || (Actual.Item2 > MejorJugada.Item2))
                   MejorJugada = Actual;
            }
        if(MejorJugada.Item1 == null)return new Jugada(this.nombre);
        return MejorJugada.Item1;
    }
}