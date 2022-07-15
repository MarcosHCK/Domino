public class Memoria
{
    //protected string owner;
    protected List<int> caras_de_la_mesa;
    public List<Ficha> mano;
    public Memoria()
    {
        //this.owner = null;
        this.caras_de_la_mesa = null;
        this.mano = null;
    }
    public void Actualizar(Estado estado, List<Ficha> mano, int index_of_actualization)
    {
        //if(this.owner == null)this.owner = estado.Jugador_en_Turno;
        this.mano = mano;
        List<Action> acciones = estado.acciones;
        for (; index_of_actualization < acciones.Count; index_of_actualization++)
            Actualizar(acciones[index_of_actualization]);   
    }
    public virtual void Actualizar(Action accion)
    {
        if(accion is Jugada)
        {
            Jugada jugada = (Jugada)accion;
            if(jugada.EsPase)return;
            if(this.caras_de_la_mesa == null)
            {
                this.caras_de_la_mesa = jugada.ficha.cabezas.ToList();
                return;
            }
            this.caras_de_la_mesa.Remove(jugada.cara_de_la_mesa);
            bool flag = true;
            foreach(int cabeza in jugada.ficha.cabezas)
                if((cabeza == jugada.cabeza_usada)&&(flag))
                {
                    flag = false;
                    this.caras_de_la_mesa.Remove(cabeza);
                }else this.caras_de_la_mesa.Add(cabeza);
        }
        return;
    }
}