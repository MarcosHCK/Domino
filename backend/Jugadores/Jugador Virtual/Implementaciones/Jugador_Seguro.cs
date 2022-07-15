public class Jugador_Seguro : Jugador_con_Memoria
{
    public Jugador_Seguro(string nombre, Memoria memoria, IDescartador descartador = null)
    : base(nombre, memoria, descartador){}
    protected override double Valorar(Jugada jugada, Estado estado, List<Ficha> mano)
    {
        double valoracion = 0;
        foreach(int cabeza in jugada.ficha.cabezas)
            if(jugada.cabeza_usada != cabeza)//O sea, queremos que la data que pongamos sea aquella que mas tenemos, o la que matamos, por ende hay que descartar la que matamos
                valoracion += (double)Contar(cabeza, mano)*0.1;
        if(jugada.ficha.EsDoble)valoracion += 1;
        return valoracion;
    }
    protected override Jugada Apertura(List<Ficha> mano)
    {
        Jugada Salida = null, current;
        double best_score = -1, actual;
        foreach(Ficha ficha in mano)
        {
            actual = this.Valorar(current = new Jugada(this.nombre, ficha), null, mano);
            if(actual > best_score)
                Salida = current;
        }
        if(Salida == null)throw new Exception("Fallo en la Apertura");
        return Salida;
    }
    protected override double[] Valorar_Datas()
    {
        double[] respuesta = new double[this.reglas.data_tope];
        for(int i = 0; i < respuesta.Count(); i++)respuesta[i] = (double)Contar(i, this.memoria.mano)*0.1;
        return respuesta;
    }
    int Contar(int data, List<Ficha> mano)
    {
        int cant = 0;
        foreach(Ficha ficha in mano)
            foreach(int cabeza in ficha.cabezas)
                if(cabeza == data)cant++;
        return cant;
    }
}