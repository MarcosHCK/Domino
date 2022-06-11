public class Portal_del_Banquero
{
    Banquero banquero;
    public Portal_del_Banquero(Banquero banquero)
    {
        this.banquero = banquero;
    }
    public List<Ficha> this[string nombre]
    {
        get
        {
            return banquero.GetMano(nombre);
        }
    }
    public int num_de_fichas_fuera
    {
        get
        {
            return this.banquero.num_de_fichas_fuera;
        }
    }
}