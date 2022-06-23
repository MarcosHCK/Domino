public class Puntuador
{
    IPuntuador_de_Fichas Puntuador_De_Fichas;
    IPuntuador_de_Manos Puntuador_De_Manos;
    IPuntuador_de_Equipos Puntuador_De_Equipos;
    public Puntuador(IPuntuador_de_Fichas Puntuador_De_Fichas, IPuntuador_de_Manos Puntuador_De_Manos, IPuntuador_de_Equipos Puntuador_De_Equipos)
    {
        this.Puntuador_De_Fichas = Puntuador_De_Fichas;
        this.Puntuador_De_Manos = Puntuador_De_Manos;
        this.Puntuador_De_Equipos = Puntuador_De_Equipos;
    }
    public int Puntuar (Ficha ficha)
    {
        return this.Puntuador_De_Fichas.Puntuar(ficha);
    }
    public int Puntuar (List<Ficha> fichas)
    {
        return this.Puntuador_De_Manos.Puntuar(this.Puntuador_De_Fichas, fichas); 
    }
    public int Puntuar (Equipo equipo, Portal_del_Banquero banquero)
    {
        return this.Puntuador_De_Equipos.Puntuar(this.Puntuador_De_Fichas, this.Puntuador_De_Manos, equipo, banquero);
    }
}