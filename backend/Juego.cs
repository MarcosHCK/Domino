public class Juego
{
    Organizador organizador;
    Estado estado;
    Reglas_del_Juego reglas;
    Banquero banquero;
    Portal_del_Banquero portal;
    public Juego(List<Jugador> jugadores, Reglas_del_Juego reglas, IOrdenador ordenador, params Equipo[] equipos)
    {
        this.reglas = reglas;
        this.organizador = new Organizador(jugadores, ordenador, equipos.ToList());
        this.estado = new Estado(reglas, organizador.jugadores, organizador.equipos, reglas.fichas_fuera(jugadores.Count));
        this.banquero = new Banquero(this.estado, this.organizador, reglas.fichas);
        this.portal = new Portal_del_Banquero(banquero);
        foreach(Jugador jugador in jugadores)
            jugador.Iniciar(reglas, organizador.jugadores, organizador.equipos);
    }

    public IEnumerable<Action> Jugar
    {
        get
        {
            int index = 0;
            for (List<Ficha>? mano = null; !reglas.GameOver(this.estado, mano);)
            {
                estado.PasarTurno(reglas, mano, organizador.jugadores, organizador.equipos);
                Jugador jugador = organizador[estado.Jugador_en_Turno];
                for (; estado.Criterio_De_Cambio_Repartir != null; estado.Actualizar_Criterios_de_Cambio(reglas, (mano = portal[estado.Jugador_en_Turno])))
                    banquero.Repartir();
                for (estado.Actualizar_Criterios_de_Cambio(reglas, (mano = portal[estado.Jugador_en_Turno])); estado.Criterio_De_Cambio_Refrescar != null; estado.Actualizar_Criterios_de_Cambio(reglas, (mano = portal[estado.Jugador_en_Turno])))
                    banquero.Refrescar();
                mano = portal[estado.Jugador_en_Turno];
                Jugada jugada = jugador.Jugar(new Estado(estado), mano);
                mano = banquero.Actualizar(jugada);
                estado.Actualizar(jugada);
                for (List<Action> acciones = estado.acciones; index < acciones.Count;)yield return acciones[index++];
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
