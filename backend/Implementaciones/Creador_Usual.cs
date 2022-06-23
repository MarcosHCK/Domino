public class Creador_Usual : ICreador
{
    bool dobles;
    int repeticiones;
    List<Ficha> _fichas;//Notar que no estoy permitiendo que se creen dos tipos de fichas diferentes en el juego. De querer esto debo cambiar some stuff
    public Creador_Usual(bool dobles = true, int repeticiones = 1)
    {
        this.dobles = dobles;
        this.repeticiones = repeticiones;
        this._fichas = new List<Ficha>();
    }
    public List<Ficha> fichas(int data_tope, int cabezas_por_ficha)
    {
        if(this._fichas.Count != 0)return new List<Ficha>(this._fichas);
        int[] cabezas = new int[cabezas_por_ficha];
        for(; cabezas[0] < data_tope; Incrementar(dobles))
            for(int i = 0; i < repeticiones; i++)
                _fichas.Add(new Ficha((int[])(cabezas.Clone())));
        return this.fichas(data_tope, cabezas_por_ficha);//dejado sin el guion bajo a proposito
        void Incrementar(bool dobles)
        {
            for (int index = cabezas_por_ficha - 1; index >= 0; index--)
            {
                if((++cabezas[index]) < data_tope)break;
                if(index == 0)break;
                cabezas[index] = 0; 
            }
            for(int i = 1; i < cabezas_por_ficha; i++)
               cabezas[i] = Math.Max(cabezas[i - 1] + ((dobles)?0:1), cabezas[i]);//Ineficiente, but, who cares?
        }
    }
    public int cant_de_fichas(int data_tope, int cabezas_por_ficha)
    {
        return Util.cant_de_fichas(data_tope, cabezas_por_ficha);
    }
}