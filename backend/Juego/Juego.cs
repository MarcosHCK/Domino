public class Juego
{
    Organizador organizador;
    Estado estado;
    Reglas_del_Juego reglas;
    Banquero banquero;
    Portal_del_Banquero portal;
    public Juego(List<Jugador> jugadores, Reglas_del_Juego reglas, IOrdenador ordenador, params Equipo[] equipos)
    {
        foreach(Jugador jugador in jugadores)
            jugador.Iniciar(reglas);
        List<Ficha> fichas = reglas.fichas;
        this.reglas = reglas;
        this.organizador = new Organizador(jugadores, ordenador, equipos.ToList());
        this.estado = new Estado(reglas, organizador.jugadores, organizador.equipos, reglas.fichas_fuera(jugadores.Count));
        this.banquero = new Banquero(this.estado, this.organizador, fichas, reglas.Puntuador);
        this.portal = new Portal_del_Banquero(banquero);
    }

    public IEnumerable<Action> Jugar
    {
        get
        {
            int index = 0;
            for (List<Ficha>? mano = null; ; index++)
            {
                estado.PasarTurno(reglas, mano);
                Jugador jugador = organizador[estado.Jugador_en_Turno];
                Console.WriteLine("Turno " + jugador.nombre);
                MostrarMano();
                if(estado.YaSeHaJugado)MostrarCaras();
                else MostrarEquipos();
                for (; estado.Cambiadores_de_Repartir != (null, null); estado.Actualizar_Cambiadores(reglas, (mano = portal[estado.Jugador_en_Turno])))
                {
                    banquero.Repartir();
                    MostrarMano();
                }
                for (; estado.Cambiador_de_Refrescar != null; estado.Actualizar_Cambiadores(reglas, (mano = portal[estado.Jugador_en_Turno])))
                {
                    banquero.Refrescar();
                    MostrarMano();
                }
                for (List<Action> acciones = estado.acciones; index < acciones.Count;)yield return acciones[index++];
                mano = portal[estado.Jugador_en_Turno];
                if(reglas.GameOver(this.estado, mano))
                {
                    Console.WriteLine("GameOver");
                    yield break;
                }
                Jugada jugada = jugador.Jugar(new Estado(estado), mano);
                mano = banquero.Actualizar(jugada);
                estado.Actualizar(jugada);
                yield return jugada;
                Console.WriteLine("EndTurn");
            }
        }
    }
    public Dictionary<string, int> Puntuaciones
    {
        get
        {
            Dictionary<string, int> retorno = new Dictionary<string, int>();
            foreach (Equipo equipo in organizador.equipos)
                retorno.Add(equipo.nombre, reglas.Puntuar(equipo, portal));
            return retorno;   
        }
    }
    void MostrarMano()
    {
        Console.WriteLine("Mano " + estado.Jugador_en_Turno);
        foreach(Ficha ficha in portal[estado.Jugador_en_Turno])Console.WriteLine(ficha);
        Console.WriteLine("break");
    }
    void MostrarCaras()
    {
        string AMostrar = "";
        foreach(int cara in estado.caras_de_la_mesa)AMostrar += (cara + " ");
        AMostrar.Substring(0, AMostrar.Length - 1);
        Console.WriteLine("Caras: " + AMostrar);
    }
    void MostrarEquipos()
    {
        Console.WriteLine("Equipos");
        foreach(Equipo equipo in estado.equipos)
        {
            Console.Write(equipo.nombre + ": " );
            foreach(string miembro in equipo.miembros)Console.Write(miembro + ' ');
            Console.WriteLine();
        }
        Console.WriteLine("break");
    }
}