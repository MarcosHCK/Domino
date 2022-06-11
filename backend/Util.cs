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
}