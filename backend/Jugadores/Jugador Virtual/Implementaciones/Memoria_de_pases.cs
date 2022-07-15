public class Memoria_de_Pases : Memoria
{
    Dictionary<string, HashSet<int>> pases;
    public Memoria_de_Pases() : base()
    {
        this.pases = new Dictionary<string, HashSet<int>>();
    }
    public override void Actualizar(Action accion)
    {
        base.Actualizar(accion);
        if(accion is Jugada)
        {
            Jugada jugada = (Jugada)accion;
            if(jugada.EsPase)
            {
                if(!pases.ContainsKey(jugada.autor))pases.Add(jugada.autor, new HashSet<int>());
                foreach(int cara in this.caras_de_la_mesa)pases[jugada.autor].Add(cara);
            }
        }
    }
    public bool SePasoAData(string nombre, int data)
    {
        if(!pases.ContainsKey(nombre))return false;
        return pases[nombre].Contains(data);
    }
}