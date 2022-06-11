public class Ordenador_Usual : IOrdenador
{
    public List<string> Ordenar(List<Equipo> equipos, int cant_de_jugadores)
    {
        List<string> orden = new List<string>();
        Util.DarAgua(equipos);
        for(int i = 0; orden.Count < cant_de_jugadores; i++)
            foreach(Equipo equipo in equipos)
                if(equipo.miembros.Count > i)
                    orden.Add(equipo.miembros[i]);
        return orden;
    }
}