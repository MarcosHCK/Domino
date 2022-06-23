public class NoSeHaRepartido : IPredicado
{
    public bool Evaluar(Estado estado, List<Ficha> mano)
    {
        if(estado.YaSeHaJugado)return false;
        foreach(var tupla in estado.fichas_por_mano)
            if(tupla.Value == 0)return true;
        return false;
    }
}
public class Nunca : IPredicado
{
    public bool Evaluar(Estado estado, List<Ficha> mano)
    {
        return false;
    }
}
public class Siempre : IPredicado
{
    public bool Evaluar(Estado estado, List<Ficha> mano)
    {
        return true;
    }
}
public class JuegoNoIniciado : IPredicado
{
    public bool Evaluar(Estado estado, List<Ficha> mano)
    {
        return !estado.YaSeHaJugado;
    }
}
public class Tranque : IPredicado
{
    public bool Evaluar(Estado estado, List<Ficha> mano)
    {
        string ultimo_en_jugar = null;
        bool listo = false;
        foreach(Jugada jugada in estado.Jugadas_en_Reversa)
        {
            if(!jugada.EsPase)return false;
            if(ultimo_en_jugar == null){ultimo_en_jugar = jugada.autor; continue;}
                if(ultimo_en_jugar != jugada.autor)listo = true;
                else if(listo)return true;
        }
        return false;
    }
}
public class Pegada : IPredicado
{
    public bool Evaluar(Estado estado, List<Ficha> mano)
    {
        foreach(var tupla in estado.fichas_por_mano)
            if(tupla.Value == 0)return true;
        return false;
    }
}
public class NoFichasFuera : IPredicado
{
    public bool Evaluar(Estado estado, List<Ficha> mano)
    {
        return (estado.fichas_fuera == 0);
    }
}
public class TodosDoblesJugados : IPredicado
{
    List<Ficha> DoblesFaltantes;
    int index;
    public TodosDoblesJugados(List<Ficha> dobles)
    {
        this.DoblesFaltantes = dobles;
        this.index = 0;
    }
    public bool Evaluar(Estado estado, List<Ficha> mano)
    {
        for(List<Action> acciones = estado.acciones; index < acciones.Count; index++)
            if(acciones[index] is Jugada)
            {
                Jugada jugada = (Jugada)acciones[index];
                if(jugada.EsPase)continue;
                if(jugada.ficha.EsDoble)this.DoblesFaltantes.Remove(jugada.ficha);
            }
        return (this.DoblesFaltantes.Count == 0);
    }
    void Iniciar(Reglas_del_Juego reglas)
    {
        foreach(Ficha ficha in reglas.fichas)
            if(ficha.EsDoble)this.DoblesFaltantes.Add(ficha);
    }
}
public class Pases_Consecutivos : IPredicado
{
    protected int pases_consecutivos;
    protected bool MismoJugador;
    protected bool SinRepartir;
    protected bool SinRefrescar;
    public Pases_Consecutivos(int pases_consecutivos, bool MismoJugador = false, bool SinRepartir = true, bool SinRefrescar = true)
    {
        this.pases_consecutivos = pases_consecutivos;
        this.MismoJugador = MismoJugador;
        this.SinRepartir = SinRepartir;
        this.SinRefrescar = SinRefrescar;
    }
    public virtual bool Evaluar(Estado estado, List<Ficha> mano)
    {
        int pases_consecutivos = 0;
        string autor = null;
        foreach(Action accion in estado.Acciones_en_Reversa)
            if(accion is Jugada)
            {
                if(!(((Jugada)accion).EsPase))break;
                if(autor == null)autor = accion.autor;
                else if((accion.autor != autor) && MismoJugador)break;
                if((++pases_consecutivos) >= this.pases_consecutivos)return true;
            }else
            {
                if((accion is Intercambio)&&this.SinRefrescar)break;
                if((accion is Reparticion)&&this.SinRepartir)break;
            }
        return false;
    }
}
public class FaseFinal : IPredicado
{
    int fichas_lider;
    public FaseFinal(int fichas_lider)
    {
        this.fichas_lider = fichas_lider;
    }
    public bool Evaluar(Estado estado, List<Ficha> mano)
    {
        return (estado.fichas_por_mano.Values.Min() <= fichas_lider);
    }
}
public abstract class Jugador_Mas : IPredicado
{
    public bool Evaluar(Estado estado, List<Ficha> mano)
    {
        if(!estado.YaSeHaJugado)return false;
        int[] fichas_por_mano = (estado.fichas_por_mano).Values.ToArray();
        bool contado = false;
        foreach(int i in fichas_por_mano)
           if (Comparar(i, mano.Count) || (i == mano.Count))
           {
               if(Comparar(i, mano.Count) || contado)return false;
               contado = true;
           }
        return true;
    }
    protected abstract bool Comparar(int i, int x);
}
public class Jugador_Mas_Atrasado : Jugador_Mas
{
    protected override bool Comparar(int i, int x){return (i > x);}
}
public class Jugador_Mas_Adelantado : Jugador_Mas
{
    protected override bool Comparar(int i, int x){return (i < x);}
}