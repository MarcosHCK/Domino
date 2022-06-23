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
}