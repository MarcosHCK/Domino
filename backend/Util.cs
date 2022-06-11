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
                    retorno.Add(new Jugada(estado.Jugador_en_Turno, ficha, cabeza, cara));
        return retorno; 
    }
}
        /*for (int i = 0; i < dp.GetLength(0); i++, Console.WriteLine())
            for(int j = 0; j < dp.GetLength(1); j++)
                Console.Write(dp[i, j].ToString() + " ");*/