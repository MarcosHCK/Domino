public interface IPuntuador
{
    int Puntuar (Ficha ficha);
    int Puntuar (List<Ficha> fichas);
    int Puntuar (Equipo equipo, Portal_del_Banquero banquero);
}