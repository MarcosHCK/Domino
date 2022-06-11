public abstract class Jugador_Virtual : Jugador
{
    public Jugador_Virtual(string nombre) : base(nombre){}
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
    protected abstract double Valorar(Jugada jugada, Estado estado, List<Ficha> mano);
    protected abstract Jugada Apertura(List<Ficha> mano);
}