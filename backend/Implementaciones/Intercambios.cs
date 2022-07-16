public class Intercambio_Random : ICriterio_de_Intercambio
{
    protected Random Azar = new Random();
    public Intercambio_Random(){this.Azar = new Random();}
    public virtual List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, int fichas_a_tomar, Puntuador puntuador)
    {
        List<Ficha> retorno = new List<Ficha>();
        for(int index; (fichas_a_tomar--) > 0; fichas_fuera.RemoveAt(index))
        {
            index = Azar.Next(fichas_fuera.Count);
            retorno.Add(fichas_fuera[index]);
        }
        return retorno;
    }
}
public class Intercambio_DoblarFicha : Intercambio_Random
{
    public Intercambio_DoblarFicha() : base(){}
    public override List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, int fichas_a_tomar, Puntuador puntuador)
    {
        List<Ficha> retorno = new List<Ficha>(); 
        foreach(Ficha ficha_descartada in descartes)
        {
            int actual = puntuador.Puntuar(ficha_descartada);
            bool flag = true;
            foreach(Ficha ficha in fichas_fuera)
                if(puntuador.Puntuar(ficha) == 2*actual)
                {
                    fichas_fuera.Remove(ficha);
                    retorno.Add(ficha);
                    flag = false;
                    break;
                }
            if(flag)retorno.Add((base.Reemplazar(fichas_fuera, new List<Ficha>(){ficha_descartada}, 1, puntuador))[0]);
        }
        while(retorno.Count < fichas_a_tomar)retorno.Add((base.Reemplazar(fichas_fuera, new List<Ficha>(), 1, puntuador))[0]);
        return retorno;
    }
}
public class Intercambio_Dos_Que_Sumen : Intercambio_Random
{
    public override List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, int fichas_a_tomar, Puntuador puntuador)
    {
        List<Ficha> retorno = new List<Ficha>();
        if(descartes.Count > 1)throw new Exception("Not Implemented");
        int punt = puntuador.Puntuar(descartes[0]);
        //fichas_fuera.Add(descartes[0]);
        foreach(Ficha a in fichas_fuera)
            foreach(Ficha b in fichas_fuera)
                if(puntuador.Puntuar(a) + puntuador.Puntuar(b) == punt)
                {
                    retorno.Add(a);
                    retorno.Add(b);
                    fichas_fuera.Remove(a);
                    fichas_fuera.Remove(b);
                    return retorno;
                }
        return base.Reemplazar(fichas_fuera, descartes, 2, puntuador);
    }
}
public class Intercambio_Sobrevalor : Intercambio_Random
{
    readonly int valor;
    public Intercambio_Sobrevalor(int valor)
    {
        this.valor = valor;
    }
    public override List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, int fichas_a_tomar, Puntuador puntuador)
    {
        List<Ficha> retorno = base.Reemplazar(fichas_fuera, descartes, fichas_a_tomar, puntuador);
        int diferencia = Suma() - this.valor;
        int aux;
        for (int i = 0; ((i < retorno.Count) && (diferencia != 0)); i++)
            for(int j = 0; j < fichas_fuera.Count; j++)
                {
                    aux = puntuador.Puntuar(retorno[i]) - puntuador.Puntuar(fichas_fuera[j]);
                    if(Math.Abs(diferencia + aux) < Math.Abs(diferencia))
                    {
                        Ficha temp = fichas_fuera[j];
                        fichas_fuera[j] = retorno[i];
                        retorno[i] = temp;
                        diferencia = diferencia + aux;
                    }
                }
        return retorno;
        int Suma()
        {
            int suma = 0;
            foreach(Ficha ficha in retorno)suma += puntuador.Puntuar(ficha);
            return suma;
        }
    }
}
public class Intercambio_Variado : Intercambio_Random
{
    public override List<Ficha> Reemplazar(List<Ficha> fichas_fuera, List<Ficha> descartes, int fichas_a_tomar, Puntuador puntuador)
    {
        List<Ficha> retorno = new List<Ficha>();
        HashSet<int> datas = new HashSet<int>();
        for(int index = Azar.Next(fichas_fuera.Count); retorno.Count < fichas_a_tomar; index = GetIndex())
            Anadir(index);
        return retorno;
        void Anadir(int index)
        {
            retorno.Add(fichas_fuera[index]);
            foreach(int cabeza in fichas_fuera[index].cabezas)
                datas.Add(cabeza);
            fichas_fuera.RemoveAt(index);
        }
        int GetIndex()
        {
            for(int i = 0; i < fichas_fuera.Count; i++)
            {
                bool flag = true;
                foreach(int cabeza in fichas_fuera[i].cabezas)
                    if(datas.Contains(cabeza))flag = false;
                if(flag)return i;
            }
            return Azar.Next(fichas_fuera.Count);
        }
    }
}
