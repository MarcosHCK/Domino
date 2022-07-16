public class Domino
{
    Dictionary<int, Jugador> people;
    public Domino()
    {
        this.people = new Dictionary<int, Jugador>();
        people.Add(1, new Jugador_Random("Yosvany"));
        people.Add(2, new Jugador_Random("Yusimy"));
        people.Add(3, new Jugador_Random("El_Brayan"));
        people.Add(4, new Jugador_Random("Yuniela"));
        people.Add(5, new Jugador_BotaGorda("Lezama"));
        people.Add(6, new Jugador_BotaGorda("Hemingway"));
        people.Add(7, new Jugador_Pasador("Yariny", new Memoria_de_Pases()));
        people.Add(8, new Jugador_Pasador("Marilyn", new Memoria_de_Pases()));
        people.Add(9, new Jugador_Seguro("Amy", new Memoria()));
        people.Add(10, new Jugador_Seguro("Tatiana", new Memoria()));
    }
    public void Partida()
    {
        List<Jugador> jugadores;
        List<Equipo> equipos;
        string partida = null;
        GetJugadores(out jugadores, out equipos);
        for (int i = 0; i < 1000; i--)
        {
            if(partida == null)Console.WriteLine("Introduzca el nombre del juego que desea jugar");
            Reglas_del_Juego Reglas = Prueba.Reglas(((partida != null)?partida:Console.ReadLine()));
            Juego juego = new Juego(jugadores, Reglas, new Ordenador_Usual(), equipos.ToArray());
            Prueba.MostrarJuego(juego);
            Console.WriteLine();
            Console.WriteLine();
        }
    }
    void GetJugadores(out List<Jugador> jugadores, out List<Equipo> equipos)
    {
        StreamReader Sr = new StreamReader("./backend/Partidas/Jugadores.txt");
        equipos = new List<Equipo>();
        jugadores = new List<Jugador>();
        Equipo aux;
        for(string entrada = Sr.ReadLine(); entrada != "break"; entrada = Sr.ReadLine())
        {
            string nombre = entrada;
            entrada = Sr.ReadLine();
            List<string> miembros = new List<string>();
            for(int cont = int.Parse(entrada.Substring(0, 1)); cont > 0; cont--)
            {
                entrada = entrada.Substring(entrada.IndexOf(' ') + 1);
                jugadores.Add(new Jugador_Humano(GetNombre()));
                miembros.Add(jugadores.Last().nombre);
                string GetNombre()
                {
                    string retorno = "";
                    for(int i = 0; (i < entrada.Length) && (entrada[i] != ' '); i++)
                        retorno += entrada[i];
                    return retorno;
                }
            }
            int[] indices;
            Util.Diseccionar_Entrada(Sr.ReadLine(), out indices);
            foreach(int i in indices)
            {
                jugadores.Add(this.people[i]);
                miembros.Add(jugadores.Last().nombre);
            }
            equipos.Add(new Equipo(nombre, miembros.ToArray()));
        }
    }
}
