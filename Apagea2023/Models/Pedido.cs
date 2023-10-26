using System;
using System.Collections.Generic;

namespace Apagea2023.Models
{
	public class Pedido
	{
        #region ... propiedades de clase ...

        public String IdPedido { get; set; } = Guid.NewGuid().ToString();
        public DateTime FechaPedido { get; set; } = DateTime.Now;
        public List<ItemPedido> ElementosPedido { get; set; } = new List<ItemPedido>();

        public Direccion DireccionEnvio { get; set; }
        public Direccion DireccionFacturacion { get; set; }

        public Decimal Subtotal { get; set; } = 0;
        public Decimal GastosEnvio { get; set; } = 2;
        public Decimal Total { get; set; } = 0;
        public String EstadoPedido { get; set; } = "en preparacion";

        #endregion

        #region ... metodos de clase ...

        public Decimal CalculoSubTotalPedido()
        {
            return this.ElementosPedido.Sum((ItemPedido item) => item.LibroItem.Precio * item.CantidadItem);
        }

        #endregion
    }
}

