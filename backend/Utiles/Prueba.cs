public static class Prueba
{
    public static void MostrarJuego(Juego juego)
    {
        foreach(Action accion in juego.Jugar)
        {
            Console.WriteLine(accion);
        }
        foreach(var tupla in juego.Puntuaciones)
            Console.WriteLine(tupla.Key + " " + tupla.Value.ToString());
    }
    public static string SimularJuego(Juego juego)
    {
        foreach(Action accion in juego.Jugar);
        return Ganador();
        string Ganador()
        {
            string respuesta = null;
            int mejor = int.MaxValue;
            foreach(var tupla in juego.Puntuaciones)
                if(tupla.Value < int.MaxValue)
                {
                    respuesta = tupla.Key;
                    mejor = tupla.Value;
                }
            return respuesta;
        }
    }
    public static void Mostrar_Juego_No_Humano()
    {

    }
    public static Reglas_del_Juego Reglas(string nombre_de_partida)
    {
        StreamReader Sr = new StreamReader("./backend/Partidas/" + nombre_de_partida + "/Numeros.txt");
        int data_tope = int.Parse(Sr.ReadLine());
        int cabezas_por_ficha = int.Parse(Sr.ReadLine());
        int fichas_por_mano = int.Parse(Sr.ReadLine());
        Creador_Usual Creador = Get_Creador_Usual();
        DaCriterio<bool> Finisher = Get_Finisher();
        Puntuador Puntuador = Get_Puntuador();
        DaCriterio<IMoverTurno> MoverTurno = Get_MoverTurno();
        MoverFichas MoverFichas = Get_MoverFichas();
        DaCriterio<IEmparejador> Emparejador = Get_Emparejador();
        DaCriterio<IValidador> Validador = Get_Validador();
        Reglas_del_Juego Reglas = new Reglas_del_Juego
        (Creador, Finisher, Puntuador,Emparejador, Validador, MoverTurno, MoverFichas, 
        data_tope, fichas_por_mano, cabezas_por_ficha);
        return Reglas; 
        Creador_Usual Get_Creador_Usual()
        {
            StreamReader Sr = new StreamReader("./backend/Partidas/" + nombre_de_partida + "/Creador.txt");
            bool dobles = (Sr.ReadLine() == "Si");
            return new Creador_Usual(dobles, int.Parse(Sr.ReadLine()));
        }
        DaCriterio<bool> Get_Finisher()
        {
            StreamReader Sr = new StreamReader("./backend/Partidas/" + nombre_de_partida + "/Finisher.txt");
            List<IPredicado> Predicados_de_Finalizacion = new List<IPredicado>()
            {new Tranque(), new Pegada(), new NoFichasFuera(), new FaseFinal(3), new TodosDoblesJugados(Creador.fichas(data_tope, cabezas_por_ficha).FindAll(x => x.EsDoble)), new Pases_Consecutivos(1), new Pases_Consecutivos(3, true, SinRefrescar : false),new Pases_Consecutivos(9, true, false, false)};
            return new DaCriterio<bool>(Predicados_de_Finalizacion, Sr, new List<bool>{true, false});
        }
        Puntuador Get_Puntuador()
        {
            StreamReader Sr = new StreamReader("./backend/Partidas/" + nombre_de_partida + "/Puntuador.txt");
            IPuntuador_de_Fichas Puntuador_de_Fichas = ((Sr.ReadLine() == "1")?(new Puntuador_de_Fichas_Usual()) : (new Puntuador_de_Fichas_de_una_Cara()));
            IPuntuador_de_Manos Puntuador_de_Manos = ((Sr.ReadLine() == "1")?(new Puntuador_de_Manos_Usual()):(new Puntuador_de_Manos_Cant_Fichas()));
            IPuntuador_de_Equipos Puntuador_De_Equipos = ((Sr.ReadLine() == "1")?(new Puntuador_De_Equipos_Total()):(new Puntuador_De_Equipos_Mejor_Parcial()));
            return new Puntuador(Puntuador_de_Fichas, Puntuador_de_Manos, Puntuador_De_Equipos);
        }
        DaCriterio<IMoverTurno> Get_MoverTurno()
        {
            StreamReader Sr = new StreamReader("./backend/Partidas/" + nombre_de_partida + "/MoverTurno.txt");
            //A continuacion una lista de los predicados a tener en cuenta para determinar a quien toca el sgte turno
            List<IPredicado> Predicados = new List<IPredicado>()
            {new Siempre(), new JuegoNoIniciado(), new NoFichasFuera(), new FaseFinal(3), new TodosDoblesJugados(Creador.fichas(data_tope, cabezas_por_ficha).FindAll(x => x.EsDoble)),
            new Pases_Consecutivos(1), new Pases_Consecutivos(2, true, SinRefrescar : false)};
            List<IMoverTurno> Criterios = new List<IMoverTurno>()
            {new MoverDerecha(), new MismoJugador()/*new MoverJusto(), new MoverPorParidad()*/};
            return new DaCriterio<IMoverTurno>(Predicados, Sr, Criterios);
        }
        MoverFichas Get_MoverFichas()
        {
            DaCriterio<(Cambiador, Cambiador)> Repartidor = Get_Repartidores();
            DaCriterio<Cambiador> Refrescador = Get_Refrescador();
            return new MoverFichas(Refrescador, Repartidor);
            DaCriterio<(Cambiador, Cambiador)> Get_Repartidores()
            {
                StreamReader Sr = new StreamReader("./backend/Partidas/" + nombre_de_partida + "/Repartidor.txt"); 
                List<ICriterio_de_Intercambio> Criterios = new List<ICriterio_de_Intercambio>()//No puedo hacer con la jerarquia actual que los jugadores seleccionen fichas a dedo. Wait for cambios
                {new Intercambio_Random(), new Intercambio_Sobrevalor(90), new Intercambio_Variado()};
                List<IPredicado> Predicados = new List<IPredicado>()
                {new NoSeHaRepartido(), new Tranque(), new FaseFinal(3), new Pases_Consecutivos(3, true, SinRefrescar : false)};
                return new DaCriterio<(Cambiador, Cambiador)>(Predicados, Sr, ATuplas(Get_Cambiadores(Criterios, Sr)));
                List<(Cambiador, Cambiador)> ATuplas(List<Cambiador> lista)
                {
                    if((lista.Count&1) == 1)throw new System.Exception("Cantidad impar");
                    List<(Cambiador, Cambiador)> retorno = new List<(Cambiador, Cambiador)>();
                    for(int i = 0; i < lista.Count; i += 2)
                        retorno.Add((lista[i], lista[i + 1]));
                    return retorno;
                }
            }
            DaCriterio<Cambiador> Get_Refrescador()
            {
                StreamReader Sr = new StreamReader("./backend/Partidas/" + nombre_de_partida + "/Refrescador.txt");
                List<ICriterio_de_Intercambio> Criterios = new List<ICriterio_de_Intercambio>()
                {new Intercambio_Random(), new Intercambio_DoblarFicha(), new Intercambio_Dos_Que_Sumen()};
                List<IPredicado> Predicados = new List<IPredicado>()
                {new Nunca(), new Pases_Consecutivos(1),new FaseFinal(3), new Jugador_Mas_Atrasado(), new Jugador_Mas_Adelantado(), new NoFichasFuera(), new Pases_Consecutivos(1, SinRefrescar : false), new Pases_Consecutivos(2, SinRefrescar : false), new Pases_Consecutivos(3, SinRefrescar : false)};
                return new DaCriterio<Cambiador>(Predicados, Sr, Get_Cambiadores(Criterios, Sr));
            }
            List<Cambiador> Get_Cambiadores(List<ICriterio_de_Intercambio> Criterios, StreamReader Sr)
            {
                List<Cambiador> Cambiadores = new List<Cambiador>();
                for (int[] entrada; Util.Diseccionar_Entrada(Sr.ReadLine(), out entrada); )
                    if(entrada.Length == 2)
                        Cambiadores.Add(new Cambiador_por_Cant_de_Fichas(Criterios[entrada[0] - 1], entrada[1]));
                    else Cambiadores.Add(new Cambiador_Por_Balance(Criterios[entrada[0] - 1], entrada[1], entrada[2], entrada[3]));
                return Cambiadores;
            }
        }
        DaCriterio<IEmparejador> Get_Emparejador()
        {
            StreamReader Sr = new StreamReader("./backend/Partidas/" + nombre_de_partida + "/Emparejador.txt");
            List<IEmparejador> Emparejadores = new List<IEmparejador>()
            {new Emparejador_Usual(), new Emparejador_Pro_Dobles(), new Emparejador_Sucesor(data_tope), new Emparejador_DobleModular(data_tope), new Emparejador_Permisivo()};
            List<IPredicado> Predicados = new List<IPredicado>()
            {new Siempre(), new Tranque(), new FaseFinal(3), new JuegoNoIniciado(), new Jugador_Mas_Atrasado(), new Jugador_Mas_Adelantado(), new Pases_Consecutivos(1, SinRefrescar : false), new Pases_Consecutivos(2, true, SinRefrescar : false), new Pases_Consecutivos(3, true, SinRefrescar : false)};
            return new DaCriterio<IEmparejador>(Predicados, Sr, Emparejadores);
        }
        DaCriterio<IValidador> Get_Validador()
        {
            StreamReader Sr = new StreamReader("./backend/Partidas/" + nombre_de_partida + "/Validador.txt");
            List<IValidador> Validadores = new List<IValidador>()
            {new Validador_Usual(), new Validador_Paridad_Diferente()/*, new Validador_por_Puntuacion_respecto_a_la_paridad_de_la_jugada_Anterior()*/};
            List<IPredicado> Predicados = new List<IPredicado>()
            {new Siempre(), new Tranque(), new FaseFinal(3), new TodosDoblesJugados(Creador.fichas(data_tope, cabezas_por_ficha).FindAll(x => x.EsDoble)), new Jugador_Mas_Atrasado(), new Jugador_Mas_Adelantado()};
            return new DaCriterio<IValidador>(Predicados, Sr, Validadores);
        }
    }
    public static void Prueba_Usual(string partida = null)
    {
        for (int i = 0; i < 1000; i--){
        Jugador_Random Yosvany, Yusimy, El_Brayan, Yuniela;
        Yosvany = new Jugador_Random("Yosvany");
        Yusimy = new Jugador_Random("Yusimy");
        El_Brayan = new Jugador_Random("El_Brayan");
        Yuniela = new Jugador_Random("Yuniela");
        Equipo Titis = new Equipo("Titis", "Yusimy", "Yuniela");
        Equipo Tatas = new Equipo("Tatas", "Yosvany", "El_Brayan");
        List<Jugador> jugadores = new List<Jugador>(){Yosvany, Yusimy, El_Brayan, Yuniela};
        if(partida == null)Console.WriteLine("Introduzca el nombre del juego que desea jugar");
        Reglas_del_Juego Reglas = Prueba.Reglas(((partida != null)?partida:Console.ReadLine()));
        Juego juego = new Juego(jugadores, Reglas, new Ordenador_Usual(), Titis, Tatas);
        Prueba.MostrarJuego(juego);
        Console.WriteLine();
        Console.WriteLine();
        }
    }
    public static void Prueba_Con_Humano()
    {
        Console.WriteLine("Introduzca su nombre");
        Jugador_Humano Usuario = new Jugador_Humano(Console.ReadLine());
        Console.WriteLine("Ahora usted jugara un juego en parejas de domino Usual, haciendo equipo con Yuniela, contra Yosvany y Yusimy");
        Jugador_Random Yosvany, Yusimy, El_Brayan, Yuniela;
        Yosvany = new Jugador_Random("Yosvany");
        Yusimy = new Jugador_Random("Yusimy");
        El_Brayan = new Jugador_Random("El_Brayan");
        Yuniela = new Jugador_Random("Yuniela");
        Equipo CPU = new Equipo("CPU", "Yosvany", "Yusimy");
        Equipo Tuyo = new Equipo("Tuyo", Usuario.nombre, "Yuniela");
        Reglas_del_Juego Reglas = Prueba.Reglas("Usual");
        List<Jugador> jugadores = new List<Jugador>(){Yosvany, Yusimy, Usuario, Yuniela};
        Juego juego = new Juego(jugadores, Reglas, new Ordenador_Usual(), CPU, Tuyo);
        Prueba.MostrarJuego(juego);
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Ahora usted jugara un juego de Longaniza individual contra El_Brayan y Yosvany");
        jugadores = new List<Jugador>(){Usuario, El_Brayan, Yosvany};
        juego = new Juego(jugadores, Prueba.Reglas("Longaniza"), new Ordenador_Usual());
        Prueba.MostrarJuego(juego);
        Console.WriteLine();
        Console.WriteLine();
    }
    public static void Prueba_Con_Humano_Del_Camaron()
    {
        Console.WriteLine("Ahora un juego individual de camaron entre cuatro jugadores");
        Console.WriteLine("Note que este es un juego muy raro, lea la descripcion de el para que entienda");
        Console.WriteLine("Introduzca su nombre");
        Jugador_Humano Usuario = new Jugador_Humano(Console.ReadLine());
        Jugador_Random Yusimy, El_Brayan, Yuniela;
        Yusimy = new Jugador_Random("Yusimy");
        El_Brayan = new Jugador_Random("El_Brayan");
        Yuniela = new Jugador_Random("Yuniela");
        List<Jugador> jugadores = new List<Jugador>(){Yusimy, Usuario, Yuniela, El_Brayan};
        Juego juego = new Juego(jugadores, Prueba.Reglas("Camaron"), new Ordenador_Usual());
        Prueba.MostrarJuego(juego);
        Console.WriteLine();
        Console.WriteLine();
    }
    public static void Prueba_de_Jugadores()
    {
        Jugador_BotaGorda A = new Jugador_BotaGorda("Marrero");
        Jugador_Pasador B = new Jugador_Pasador("Yariny", new Memoria_de_Pases());
        Jugador_BotaGorda C = new Jugador_BotaGorda("Hemingway");
        Jugador_Pasador D = new Jugador_Pasador("Thatcher", new Memoria_de_Pases());
        Equipo Gordo = new Equipo("Gordo", A.nombre, C.nombre);
        Equipo Pasador = new Equipo("Pasador", B.nombre, D.nombre);
        List<Jugador> jugadores = new List<Jugador>(){A, B, C, D};
        Juego juego;
        Console.WriteLine("desea que se muestren los juegos? Si o no");
        bool flag = (Console.ReadLine() == "Si");
        Dictionary<string, int> Victorias = new Dictionary<string, int>();
        Victorias.Add("Gordo", 0);
        Victorias.Add("Pasador", 0);
        for (int i = 0; i < 1000; i++)
        {
            if(flag)Console.WriteLine("Introduzca el juego que desea que se juege");
            juego = new Juego(jugadores, Prueba.Reglas(Console.ReadLine()), new Ordenador_Usual(), Gordo, Pasador);
            if(flag)Prueba.MostrarJuego(juego);
            else Victorias[SimularJuego(juego)]++;
        }
        foreach(var tupla in Victorias)Console.WriteLine(tupla.Value + " " + tupla.Key);
    }
}
