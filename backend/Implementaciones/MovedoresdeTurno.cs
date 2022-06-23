public class MoverDerecha : IMoverTurno
{
    int index;
    public MoverDerecha()
    {
        this.index = 0;
    }
    public virtual string SgteJugador(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        List<string> jugadores = estado.jugadores;
        index = (index + 1)%jugadores.Count;
        return jugadores[index];
    }
}
public class MismoJugador : IMoverTurno
{
    public virtual string SgteJugador(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        if(!estado.YaSeHaJugado)throw new System.Exception("No puede jugar el mismo si nadie ha jugado");
        return estado.acciones.Last().autor;
    }
}
/*public class MoverLonganiza : MoverDerecha
{
    public MoverLonganiza() : base(){}
    public override string SgteJugador(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {//Usar el predicado Jugador no lleva
        foreach(Jugada jugada in Util.PosiblesJugadas(estado, mano))
            if(reglas.EsValida(jugada, estado, mano))return estado.Jugador_en_Turno;
        return base.SgteJugador(reglas, estado, mano);
    }
}
public class MoverBurrito : MoverDerecha
{
    public MoverBurrito() : base(){}
    public override string SgteJugador(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        if((estado.fichas_fuera > 0) && (estado.YaSeHaJugado))
        {
            Action accion = estado.acciones.Last();
            if(accion is Jugada)
            {
                Jugada jugada = (Jugada)accion;
                if(jugada.EsPase)return jugada.autor;
            }
        }
        return base.SgteJugador(reglas, estado, mano);
    }
}*/
/*public class MoverJusto : IMoverTurno
{
    public MoverJusto(){};
    public string SgteJugador(Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        throw new System.Exception("No implementado");
    }
}*/