public class Validador_Usual : IValidador
{
    public virtual bool EsValida(Jugada jugada, Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        return (mano.Contains(jugada.ficha) && estado.caras_de_la_mesa.Contains(jugada.cara_de_la_mesa) && jugada.ficha.cabezas.Contains(jugada.cabeza_usada));
    }
}
public class Validador_Paridad_Diferente : Validador_Usual
{
    public override bool EsValida(Jugada jugada, Reglas_del_Juego reglas, Estado estado, List<Ficha> mano)
    {
        return (base.EsValida(jugada, reglas, estado, mano)&&(Diferente_Paridad()));
        bool Diferente_Paridad()
        {
            Jugada ultima = estado.Jugadas_en_Reversa.First();
            if((!estado.YaSeHaJugado) || (ultima.ficha == null))return true;//Si la ultima jugada fue pase tira lo que sea
            return (((jugada.ficha.cabezas.Sum() + ultima.ficha.cabezas.Sum())&1) != 0);//Si suman impar tienen diferente paridad
        }
    }
}