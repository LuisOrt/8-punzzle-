using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1{
    public class Estado{
        public Estado Padre{ get; set; }
        public int accion{ get; set; }
        public int profundidad{ get; set; }
        public int costo { get; set; }
        public int[] estado { get; set; }
        public int costoH { get; set; }

        public Estado(Estado pa, int acc, int prof, int cost, int[] est,int ch) {
            Padre = pa;
            accion = acc;
            profundidad = prof;
            costo = cost;
            estado = est;
            costoH = ch;
        }
    }
}
