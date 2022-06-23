public class Puntuador_de_Fichas_Usual : IPuntuador_de_Fichas
{
    public int Puntuar(Ficha ficha)
    {
        return ficha.cabezas.Sum();
    }
}
public class Puntuador_de_Fichas_de_una_Cara : IPuntuador_de_Fichas
{
    public int Puntuar(Ficha ficha)
    {
        return ficha.cabezas.Max();
    }
}
public class Puntuador_de_Manos_Usual : IPuntuador_de_Manos
{
    public int Puntuar (IPuntuador_de_Fichas Puntuador_De_Fichas, List<Ficha> fichas)
    {
        if(fichas.Count == 0)return - 1; 
        int retorno = 0;
        foreach(Ficha ficha in fichas)
            retorno += Puntuador_De_Fichas.Puntuar(ficha);
        return retorno;
    }
}
public class Puntuador_de_Manos_Cant_Fichas : IPuntuador_de_Manos
{
    public int Puntuar (IPuntuador_de_Fichas Puntuador_De_Fichas, List<Ficha> fichas)
    {
        if(fichas.Count == 0)return - 1; 
        return fichas.Count;
    }
}
public class Puntuador_De_Equipos_Total : IPuntuador_de_Equipos
{
    public int Puntuar (IPuntuador_de_Fichas Puntuador_De_Fichas, IPuntuador_de_Manos Puntuador_De_Manos, Equipo equipo, Portal_del_Banquero banquero)
    {
        int retorno = 0, aux;
        foreach(string miembro in equipo.miembros)
        {
            aux = Puntuador_De_Manos.Puntuar(Puntuador_De_Fichas, banquero[miembro]);
            if(aux == -1)return -1;
            retorno += aux;
        }
        return retorno;
    }
}
public class Puntuador_De_Equipos_Mejor_Parcial : IPuntuador_de_Equipos
{
    public int Puntuar (IPuntuador_de_Fichas Puntuador_De_Fichas, IPuntuador_de_Manos Puntuador_De_Manos, Equipo equipo, Portal_del_Banquero banquero)
    {
        int retorno = int.MaxValue;
        foreach(string miembro in equipo.miembros)
            retorno = Math.Min(Puntuador_De_Manos.Puntuar(Puntuador_De_Fichas, banquero[miembro]), retorno);
        return retorno;
    }
}