public static class Util
{
    public static void DarAgua<T> (List<T> cosas)
    {
        Random Azar = new Random();
        T[] A = cosas.ToArray();
        double[] B = new double[cosas.Count];
        for(int i = 0; i < cosas.Count; B[i++] = Azar.NextDouble());
        System.Array.Sort(B, A);
        cosas = A.ToList();
    }
    public static int cant_de_fichas(int data_tope, int cabezas_por_ficha)
    {
        int[,] dp = new int[data_tope, cabezas_por_ficha + 1];
        for(int i = 0; i < dp.GetLength(0); dp[i++, 0] = 1);
        for(int j = 1; j < dp.GetLength(1); j++)
            for(int i = dp.GetLength(0) - 1, sum = 0; i >= 0; i--)
            {
                sum += dp[i, j - 1];
                dp[i, j] = sum;
            }
        return dp[0, cabezas_por_ficha];
    }
    public static List<Jugada> PosiblesJugadas(Estado estado, List<Ficha> mano)
    {
        List<Jugada> retorno = new List<Jugada>();
        foreach(Ficha ficha in mano)
            foreach(int cabeza in ficha.cabezas)
                foreach(int cara in estado.caras_de_la_mesa)
                    retorno.Add(new Jugada(estado.Jugador_en_Turno, ficha, cabeza, cara, estado.fichas_por_mano[estado.Jugador_en_Turno] - 1));
        return retorno; 
    }
    public static bool Diseccionar_Entrada(string entrada, out int[] retorno)
    {
        int index = 0;
        retorno = null;
        if((entrada == "break") || (entrada.Length == 0))return false;
        string[] numeros = entrada.Split(' ');
        retorno = new int[numeros.Length];
        foreach(string s in numeros)retorno[index++] = int.Parse(s);
        return true; 
    }
    public static double ID()
    {
        Random Azar = new Random();
        return Azar.NextDouble()*Math.Pow(10, 12) + Azar.NextDouble()*Math.Pow(10, 6) + Azar.NextDouble();
    }
}