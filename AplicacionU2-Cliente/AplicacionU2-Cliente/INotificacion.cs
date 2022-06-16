using System;
using System.Collections.Generic;
using System.Text;

namespace AplicacionU2_Cliente
{
    public interface INotificacion
    {
        void ToastAlert(string mensaje);
        void SnackAlert(string mensaje);
    }
}
