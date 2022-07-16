public class Jugador_Humano : Jugador
{
    public Jugador_Humano(string nombre) : base(nombre){}
    public override Jugada Jugar(Estado estado, List<Ficha> mano)
    {
        foreach(Jugada X in Util.PosiblesJugadas(estado, mano))
            if(this.reglas.EsValida(X, estado, mano))
            {
                Jugada jugada;
                for(jugada = null; ; jugada = null)
                {
                    Console.WriteLine("Ficha_a_Jugar:");
                    int index = int.Parse(Console.ReadLine());
                    Console.WriteLine("Cabeza:");
                    int cabeza = int.Parse(Console.ReadLine());
                    Console.WriteLine("Cara:");
                    int cara = int.Parse(Console.ReadLine());
                    jugada = new Jugada(this.nombre, mano[index], cabeza, cara, mano.Count - 1);
                    if(this.reglas.EsValida(jugada, estado, mano))return jugada;
                    Console.WriteLine("Jugada No Valida");
                }
            }
        return new Jugada(this.nombre);
    }
    public override List<Ficha> Descartar(Cambiador cambiador, Estado Estado, List<Ficha> mano)
    {
        if(cambiador is Cambiador_Por_Balance)
        {
            Cambiador_Por_Balance Cambiador = (Cambiador_Por_Balance)cambiador;
            if(Cambiador.Descartes_Permitidos == 0)return new List<Ficha>();
            List<Ficha> descartes = new List<Ficha>();
            Console.WriteLine("Descartes Obligatorios " + Cambiador.Descartes_Obligatorios);
            Console.WriteLine("Descartes Permitidos " + Cambiador.Descartes_Permitidos);
            Console.WriteLine("Fichas_a_Descartar:");
            string entrada = Console.ReadLine();
            foreach(string numero in entrada.Split(' '))
            {
                Ficha ficha = mano[int.Parse(numero)];
                descartes.Add(ficha);
            }
            return descartes;
        }else
        {
            Cambiador_por_Cant_de_Fichas Cambiador = (Cambiador_por_Cant_de_Fichas)cambiador;
            int num_d_descartes = mano.Count - Cambiador.cant_de_fichas;
            return this.Descartar(new Cambiador_Por_Balance(Cambiador.Criterio_de_Intercambio, num_d_descartes, num_d_descartes, num_d_descartes), Estado, mano);
        }
    }
}