/*public class Partida
{
    IEnumerable<Juego> juegos;
    Dictionary<Equipo, int> Puntuaciones;
    ICalificador calificador;
    int tope;
    public Partida(Organizador organizador, IEnumerable<Juego> juegos, int tope)
    {
        this.Puntuaciones = new Dictionary<Equipo, int>();
        this.tope = tope;
    }
    public IEnumerable<bool> Jugar
    {
        get
        {
            foreach(Juego juego in juegos)
            {
                if (Puntuaciones.Values.Max() >= tope)yield break;
                for(; juego.Jugar; juego.Mostrar());
                Console.WriteLine("\n");
                foreach (var tupla in juego.Puntuaciones)
                    Console.WriteLine(tupla.Key.nombre + ' ' + tupla.Value.ToString());
                calificador.Actualizar_Puntuaciones(juego.Puntuaciones, this.Puntuaciones);
                Console.WriteLine("\n\n\n\n");  
                foreach (var tupla in Puntuaciones)
                    Console.WriteLine(tupla.Key.nombre + ' ' + tupla.Value.ToString());
                yield return true;
            }
            yield break;
        }
    }
}*/