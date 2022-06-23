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
        this.reglas = reglas;
        this.organizador = new Organizador(jugadores, ordenador, equipos.ToList());
        this.estado = new Estado(reglas, organizador.jugadores, organizador.equipos, reglas.fichas_fuera(jugadores.Count));
        this.banquero = new Banquero(this.estado, this.organizador, reglas.fichas, reglas.Puntuador);
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
                for (; estado.Cambiador_de_Repartir != null; estado.Actualizar_Cambiadores(reglas, (mano = portal[estado.Jugador_en_Turno])))
                    banquero.Repartir();
                for (; estado.Cambiador_de_Refrescar != null; estado.Actualizar_Cambiadores(reglas, (mano = portal[estado.Jugador_en_Turno])))
                    banquero.Refrescar();
                for (List<Action> acciones = estado.acciones; index < acciones.Count;)yield return acciones[index++];
                mano = portal[estado.Jugador_en_Turno];
                if(reglas.GameOver(this.estado, mano))yield break;
                Jugada jugada = jugador.Jugar(new Estado(estado), mano);
                mano = banquero.Actualizar(jugada);
                estado.Actualizar(jugada);
                yield return jugada;
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
}
