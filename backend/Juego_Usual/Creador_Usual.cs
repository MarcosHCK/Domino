public class Creador_Usual : ICreador
{
    public List<Ficha> fichas(int data_tope, int cabezas_por_ficha)
    {
        List<Ficha> fichas = new List<Ficha>();
        int[] cabezas = new int[cabezas_por_ficha];
        for(; cabezas[0] < data_tope; Incrementar())
            fichas.Add(new Ficha((int[])(cabezas.Clone())));
        return fichas;
        void Incrementar()
        {
            for (int index = cabezas_por_ficha - 1; index >= 0; index--)
            {
                if((++cabezas[index]) < data_tope)break;
                if(index == 0)break;
                cabezas[index] = 0; 
            }
            for(int i = 1; i < cabezas_por_ficha; i++)
               cabezas[i] = Math.Max(cabezas[i - 1], cabezas[i]);//Ineficiente, but, who cares?
        }
    }
}