using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xmlimport
{
    interface ICola
    {
        void Insertar(Contribuyente d);
        Contribuyente GetFrente();
        Contribuyente Eliminar();
        void Clear();
        bool IsEmpty();
    }
    class Cola : ICola
    {
        public Nodo frente;
        public Nodo final;
        public Cola()
        {
            frente = final = null;
        }
        public void Insertar(Contribuyente d)
        {
            if (!IsEmpty())
            {
                final.sig = new Nodo(d);
                final = final.sig;
            }
            else
            {
                frente = new Nodo(d);
                final = frente;
            }
        }
        public Contribuyente GetFrente()
        {
            if (!IsEmpty())
            {
                return frente.dato;
            }
            return null;
        }
        public Contribuyente Eliminar()
        {
            if (!IsEmpty())
            {
                Contribuyente a = frente.dato;
                frente = frente.sig;
                return a;
            }
            return null;
        }
        public void Clear()
        {
            frente = final = null;
        }
        public bool IsEmpty()
        {
            if (frente == null) return true;
            return false;
        }
    }
    class Nodo : Contribuyente
    {
        //public string dato;
        public Contribuyente dato;
        public Nodo sig;
        public Nodo()
        {
            dato = null;
            sig = null;
        }
        public Nodo(Contribuyente Item)
        {
            dato = Item;
            sig = null;
        }
        public Nodo(Contribuyente Item, Nodo n)
        {
            dato = Item;
            sig = n;
        }
    }

}
