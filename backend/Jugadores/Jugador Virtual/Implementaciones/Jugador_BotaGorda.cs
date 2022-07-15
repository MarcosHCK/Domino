public class Jugador_BotaGorda : Jugador_Virtual
{
    public Jugador_BotaGorda(string nombre, IDescartador descartador = null)
    : base(nombre, descartador){}
    protected override double Valorar(Jugada jugada, Estado estado, List<Ficha> mano)
    {
        return this.reglas.Puntuar(jugada.ficha) * 0.01;
    }
    protected override double[] Valorar_Datas()
    {
        //El jugador como es bota gorda le da menos valor a las datas mas gordas 
        //No esta preparado para ser fiable cuando El Creador del Juego es muyyy Wild, provocando que las datas no cubran todo el rango desde 0 hasta data tope
        //O quizas incluso en tal caso works bien
        double[] retorno = new double[this.reglas.data_tope];
        int[] aux = new int[this.reglas.cabezas_por_ficha];//Con este invento garantizo que para fichas de n caras el Botagorda aun pueda caracterizar bien a cada data
        for (int i = 0; i < retorno.Length; i++)
        {
            aux[0] = i;
            retorno[i] = this.reglas.Puntuar(new Ficha(aux[i])) * 0.01;
        }
        return retorno;
    }
    protected override Jugada Apertura(List<Ficha> mano)
    {
        List<Ficha> dobles = new List<Ficha>();
        foreach(Ficha ficha in mano)
            if(ficha.EsDoble)dobles.Add(ficha);
        Ficha mejor = null;
        int mayor = int.MinValue;
        int actual;
        foreach(Ficha ficha in dobles)
        {
            actual = this.reglas.Puntuar(ficha);
            if((mayor == null) || (mayor < actual))
            {
                mayor = actual;
                mejor = ficha;
            }
        }
        if(mejor == null)
        {
            foreach(Ficha ficha in mano)
            {
                actual = this.reglas.Puntuar(ficha);
                if((mayor == null) || (mayor < actual))
                {
                    mayor = actual;
                    mejor = ficha;
                }
            }
        }
        return new Jugada(this.nombre, mejor);
    }
}