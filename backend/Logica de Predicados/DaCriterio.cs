public class DaCriterio<T> 
{
    List<IPredicado> Predicados;
    List<(int[], T)> Combinaciones;
    public DaCriterio(List<IPredicado> Predicados, StreamReader Sr, List<T> Criterios)
    {
        this.Predicados = Predicados;
        this.Combinaciones = new List<(int[], T)>();
        for (int[] entrada; Util.Diseccionar_Entrada(Sr.ReadLine(), out entrada);)
        {
            T Criterio = Criterios[entrada[0] - 1];
            int[] Combinacion = entrada.Skip(1).ToArray();
            this.Combinaciones.Add((Combinacion, Criterio));
        }
    }
    public T Criterio(Estado estado, List<Ficha> mano)
    {
        bool[] evaluacion = new bool[this.Predicados.Count];
        int index = 0;
        foreach(IPredicado predicado in this.Predicados)
            evaluacion[index++] = predicado.Evaluar(estado, mano);
        index = 0;
        foreach(var tupla in this.Combinaciones)
            if(Coinciden(tupla.Item1, evaluacion))return tupla.Item2;
        return default(T);
        bool Coinciden(int[] A, bool[] B)
        {
            foreach(int index in A)
            {
                bool valor = evaluacion[Math.Abs(index) - 1];
                if((index > 0)&&(!valor))return false;
                if((index < 0)&&(valor))return false;
            }
            return true;
        }
    }
}