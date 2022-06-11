public abstract class Jugador_Virtual : Jugador
{
    public Jugador_Virtual(string nombre) : base(nombre){}
    public override Jugada Jugar(Estado estado, List<Ficha> mano)
    {
        if(estado.jugadas.Count == 0)return this.Apertura(mano);
        (Jugada?, double) MejorJugada = (null, 0);
        (Jugada, double) Actual;
        Jugada jugada;
        foreach(Ficha ficha in mano)
            foreach(int cabeza in ficha.cabezas)
                foreach(int cara in estado.caras_de_la_mesa)
                {
                    jugada = new Jugada(this.nombre, ficha, cabeza, cara);
                    if(this.reglas.EsValida(jugada, estado, this.equipos, mano))
                    {
                        Actual = (jugada, this.Valorar(jugada, estado, mano));
                        if((MejorJugada.Item1 == null) || (Actual.Item2 > MejorJugada.Item2))
                            MejorJugada = Actual;
                    }
                }
        if(MejorJugada.Item1 == null)return new Jugada(this.nombre);
        return MejorJugada.Item1;
    }
    protected abstract double Valorar(Jugada jugada, Estado estado, List<Ficha> mano);
    protected abstract Jugada Apertura(List<Ficha> mano);
}