public class Puntuador_Usual : IPuntuador
{
    public int Puntuar (Ficha ficha)
    {
        return ficha.cabezas.Sum();
    }
    public int Puntuar (List<Ficha> fichas)
    {
        int retorno = 0;
        foreach(Ficha ficha in fichas)
            retorno += this.Puntuar(ficha);
        return retorno;
    }
    public int Puntuar (Equipo equipo, Portal_del_Banquero portal)
    {
        int retorno = int.MaxValue;
        foreach(string jugador in equipo.miembros)
        {
            retorno = Math.Min(retorno, this.Puntuar(portal[jugador]));
            if(retorno == 0)return 0;
        }
        return retorno;
    }  
}