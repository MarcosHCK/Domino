public class Jugador_Pasador : Jugador_con_Memoria
{
    Random Azar;
    public Jugador_Pasador(string nombre, Memoria_de_Pases memoria, IDescartador descartador = null)
    : base(nombre, memoria, descartador)
    {
        this.Azar = new Random();
        this.memoria = memoria;
    }
    protected override double Valorar(Jugada jugada, Estado estado, List<Ficha> mano)
    {
        string sgte;
        double valoracion = 0;
        bool NoLlevaNada, IsPartner;
        Memoria_de_Pases memoria = (Memoria_de_Pases)this.memoria;
        int cont = 3;
        do
        {
            estado.Actualizar(jugada);
            sgte = this.reglas.MoveNext(estado, mano);
            IsPartner = MismoEquipo();
            Evaluar();
            jugada = new Jugada(sgte);
        }while((NoLlevaNada) && ((cont--) > 0));
        return valoracion + Azar.NextDouble()*0.01;
        void Evaluar()
        {
            NoLlevaNada = true;
            foreach(int cara in estado.caras_de_la_mesa)
                if(memoria.SePasoAData(sgte, cara))
                    valoracion += ((IsPartner)?-1:1);
                else NoLlevaNada = false;
            valoracion += (NoLlevaNada?estado.caras_de_la_mesa.Count:0);
            if(TodasCarasIguales())valoracion *= 3;//para q asi prefiera llevar la dos caras de la mesa al mismo numero si sto le da una minima ventaja, y en cambio lo repudie si esto le da una minima desventaja
            bool TodasCarasIguales()
            {
                return (estado.caras_de_la_mesa.Aggregate(((a, b) => ((a == b)?(a):(-1)))) != -1);
            }
        }
        bool MismoEquipo()
        {
            foreach(Equipo equipo in estado.equipos)
                if(equipo.miembros.Contains(sgte))
                    return equipo.miembros.Contains(this.nombre);
            throw new Exception(sgte + " no pertenece a ningun equipo");
        }
    }
    protected override double[] Valorar_Datas()
    {
        //En este caso el jugador valora una data de acuerdo a la cantidad de elementos de ella que posee
        double[] retorno = new double[this.reglas.data_tope];
        foreach(Ficha ficha in this.memoria.mano)
            foreach(int cabeza in ficha.cabezas)
            {
                try{
                retorno[cabeza] += 1;
                }catch{throw new Exception("Cabeza " + cabeza);}
            }
        return retorno;
    }
    protected override Jugada Apertura(List<Ficha> mano)
    {
        double[] datas = this.Valorar_Datas();
        double mejor = 0, actual;
        Ficha Salida = null;
        foreach(Ficha ficha in mano)
            if(ficha.EsDoble)
            {
                actual = datas[ficha.cabezas[0]];//Notar que esto solo funciona para dos fichas. No seria nada dificil cambiarlo todo para tres
                if(actual > mejor)
                {
                    Salida = ficha;
                    mejor = actual;
                }
            }
        if(Salida != null)return new Jugada(this.nombre, Salida);
        mejor = 0;
        Salida = null;
        foreach(Ficha ficha in mano)
        {
            actual = 0;
            foreach(int cabeza in ficha.cabezas)
                actual += datas[cabeza];
            if(actual > mejor)
            {
                Salida = ficha;
                mejor = actual;
            }
        }
        return new Jugada(this.nombre, Salida);
    }
}