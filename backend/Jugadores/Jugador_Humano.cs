public class Jugador_Humano : Jugador
{
    public Jugador_Humano(string nombre) : base(nombre){}
    public override Jugada Jugar(Estado estado, List<Ficha> mano)
    {
        foreach(Jugada X in Util.PosiblesJugadas(estado, mano))
            if(this.reglas.EsValida(X, estado, mano))
            {
                Mostrar(estado, mano);
                Jugada jugada;
                for(jugada = null; ; jugada = null)
                {
                    Console.WriteLine("Introduzca el indice de la ficha que desea jugar, la cabeza de la ficha que usted usara y la cara por la que desea jugarla separados por un salto de linea");
                    int index = int.Parse(Console.ReadLine());
                    int cabeza = int.Parse(Console.ReadLine());
                    int cara = int.Parse(Console.ReadLine());
                    jugada = new Jugada(this.nombre, mano[index], cabeza, cara, mano.Count - 1);
                    if(this.reglas.EsValida(jugada, estado, mano))return jugada;
                    Console.WriteLine("Jugada No Valida");
                }
            }
        Console.WriteLine("No puedes jugar");
        return new Jugada(this.nombre);
    }
    public override List<Ficha> Descartar(Cambiador cambiador, Estado Estado, List<Ficha> mano)
    {
        if(cambiador is Cambiador_Por_Balance)
        {
            Cambiador_Por_Balance Cambiador = (Cambiador_Por_Balance)cambiador;
            if(Cambiador.Descartes_Permitidos == 0)return new List<Ficha>();
            Mostrar(Estado, mano);
            List<Ficha> descartes = new List<Ficha>();
            Console.WriteLine("Introduzca en un renglon los indices de las fichas que desea descartar. Debe descartar al menos " + Cambiador.Descartes_Obligatorios + "fichas");
            string entrada = Console.ReadLine();
            foreach(string numero in entrada.Split(' '))
            {
                Ficha ficha = mano[int.Parse(numero)];
                //if(descartes.Contains(ficha))throw new Exception("TRAMPOSO");
                //Podran hacer trampa pero una vez que cree la interfaz visul no habra problemas
                descartes.Add(ficha);
            }
            return descartes;
        }
        throw new System.Exception("No Implementado");
    }
    void Mostrar(Estado estado, List<Ficha> mano)
    {
        Console.WriteLine("\nEstos son los equipos");
        foreach(Equipo equipo in estado.equipos)
        {
            Console.Write(equipo.nombre + ": ");
            foreach(string miembro in equipo.miembros)Console.Write(miembro + ' ');
            Console.WriteLine();
        }
        Console.WriteLine("\nEstas son tus fichas");
        for(int i = 0; i < mano.Count; i++)Console.WriteLine(i.ToString() + "-- " + mano[i].ToString());
        Console.WriteLine("\nEstas son las caras de la mesa por las que puedes jugar:");
        foreach(int cara in estado.caras_de_la_mesa)Console.Write(cara.ToString() + ' ');
    }
}