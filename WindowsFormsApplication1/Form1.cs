

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication1{
    public partial class Form1 : Form{
       
        public static int cambios =0;

        public Form1(){
            InitializeComponent();
        }

        private void Gnera(object sender, EventArgs e){
            int[] c;
            int tam;
            textBox1.Text = "";
            try{
                cambios = Convert.ToInt32(textBox3.Text);
            }
            catch { cambios = 0;}
            try{
                tam = Convert.ToInt32(textBox2.Text);
            }catch{
                tam = 0;
            }
            
            c = CalcularNumeros(tam*tam,true);
            muestra(c);
        }

        private int[] CalcularNumeros(int c,bool mezclar){
            int[] n=new int[c];
            for (int i = 0; i <(c-1) ; i++){
                n[i] = i+1;
            }
            if (mezclar)
            {
                n = Mesclar(n);
            }
            return n;
        }
       
        private int[] Mesclar(int[] num){
            Random r = new Random();
            if (checkBox3.Checked){
                for (int i = 0; i < cambios; ){
                    int a = r.Next(1, 5);
                    if (posible(a, num)){
                        num=realiza(a, num);
                        i++;
                    }
                }
            }else{
                for (int i = 0; i < cambios; i++){
                    int pos1 = r.Next(0, num.Length);
                    int pos2 = r.Next(0, num.Length);
                    int var = num[pos1];
                    num[pos1] = num[pos2];
                    num[pos2] = var;
                }
            }
           
        return num;
        }

        private void button2_Click(object sender, EventArgs e){
            todos.Clear();
            List<Estado> lista = new List<Estado>();
            lista = busca(EstadoActual());
            int lc=0;
            try{
                lc = lista.Count;
                textBox1.AppendText("---------Eureca-----------\n");
                for (int i = (lista.Count-1); i > 0; i--)
                {
                    muestra(lista.ElementAt(i).estado);
                    textBox1.AppendText("\n");
                }
            }catch { 
             textBox1.AppendText("\n -No Exite Solucion-\n");
            }
            nodMem.Text = todos.Count.ToString();
            
        }

        private void muestra(int[] p){
            int tam = p.Length;
            double t = float.Parse(tam + "");
            t = Math.Pow(t, 0.5);
            tam = Convert.ToInt32(t);
            int x = 0;
            for (int i = 0; i < tam; i++){
                for (int j = 0; j < tam; j++){
                    if (p[x] > 9){
                        textBox1.AppendText(p[x] + " | ");
                    }
                    else{
                        textBox1.AppendText(p[x] + "  |  ");
                    }
                    x++;
                }
                textBox1.AppendText("\n");
            }
        }

        private Estado EstadoActual() {
            char[] xD = textBox1.Text.ToCharArray();
            string aux = "";
            List<int> cc = new List<int>();
            for(int i=0;i<xD.Length;i++){   
                if (xD[i] == '|'||xD[i]=='\n'){
                    try{
                        cc.Add(Convert.ToInt32(aux));
                    }
                    catch { }
                    aux = "";
                }
                else{
                    aux = aux + xD[i];
                }
            }
            int[] c = cc.ToArray();
            Estado e = new Estado(null,0,0,0,c,0);
            return e;
        }
        
        //referente a busqueda
        List<int[]> todos = new List<int[]>();

        public List<Estado> busca(Estado inicial){
            List<Estado> lista = new List<Estado>();
            lista.Add(inicial);
            while (true){
                if (isvacio(lista)){
                    return null;
                }
                Estado actual = BorrarPrimero(lista);
                if (pruevaObj(actual.estado)){
                    List<Estado> res = new List<Estado>();
                    while (actual != null){
                        res.Add(actual);
                        actual = actual.Padre;
                    }
                    return res;
                }
                lista = insertatTodo(Expandir(actual), lista);
                if (radioButton3.Checked){
                    lista = FuncionHeuristica(lista);
                }
            }
        }

        private List<Estado> FuncionHeuristica(List<Estado> lista){
            List<Estado> ListaOrdenada = new List<Estado>();
            ListaOrdenada = lista.OrderBy(o => o.costoH).ToList();
            return ListaOrdenada;
        }

        private List<Estado> insertatTodo(List<Estado> exp, List<Estado> lista){//verificamos repetidos

            for (int i = 0; i < exp.Count; i++){
                bool rep = false;
                for (int j = 0; j < lista.Count; j++){
                    if (lista.ElementAt(j).estado == exp.ElementAt(i).estado){
                        rep = true;
                    }
                }
                if (!rep)
                {
                    lista.Add(exp.ElementAt(i));
                }
            }

            return lista;
        }
  
        private List<Estado> Expandir(Estado actual){
            List<Estado> sucesores = new List<Estado>();
            for (int i = 1; i < 5; i++){
                if (posible(i, actual.estado)){
                    int acc = i;
                    int cost = actual.costo + 1;
                    int[] rea=(int[]) actual.estado.Clone();
                    rea = realiza(i,rea);
                    int pro = actual.profundidad + 1;
                    Estado x;
                    if (radioButton3.Checked){
                        int h = heuristica(rea,cost);
                       x= new Estado(actual, acc, pro, cost, rea, h);
                    }else{
                       x = new Estado(actual, acc, pro, cost, rea, 0);
                    }
                    bool pas = true;
                    for (int q = 0; q < todos.Count; q++){
                        if (compara(todos.ElementAt(q), x.estado)){
                            pas = false;
                        }
                    }
                    if (pas){
                        sucesores.Add(x);
                    }
                    todos.Add(x.estado);
                    
                    if(checkBox1.Checked){
                        textBox1.AppendText("\n");
                        muestra(x.estado);
                        textBox1.AppendText("\n");
                    }
                }
            }
            return sucesores;
        }

        private int heuristica(int[] rea, int cost){
            int heuristicCost = 0;
            int gridX = (int)Math.Sqrt(rea.Length);
            int idealX;
            int idealY;
            int currentX;
            int currentY;
            for (int i = 0; i < rea.Length-1; i++){
                int value = rea[i] - 1;
                if (value != i){
                    // Misplaced tile
                    idealX = value % gridX;
                    idealY = value / gridX;
                    currentX = i % gridX;
                    currentY = i / gridX;
                    heuristicCost += (Math.Abs(idealY - currentY) + Math.Abs(idealX - currentX));
                }
            }

            return heuristicCost;
        }

        private bool compara(int[] p1, int[] p2){
            for (int i = 0; i < p1.Length; i++){
                if (!(p1[i] == p2[i])){
                    return false;
                }
            }
            return true;
        }

        private int[] realiza(int accion, int[] act){

            int tam = act.Length;
            double t = float.Parse(tam + "");
            t = Math.Pow(t, 0.5);
            tam = Convert.ToInt32(t);

            int pv = posNum(act,0);
            int[] r = act;
            if (accion == 1){
                r[pv] = r[pv - tam];
                r[pv - tam] = 0;
            }
            if (accion == 2){
                r[pv] = r[pv + tam];
                r[pv + tam] = 0;
            }
            if (accion == 3){
                r[pv] = r[pv + 1];
                r[pv + 1] = 0;
            }
            if (accion == 4){
                r[pv] = r[pv - 1];
                r[pv - 1] = 0;
            }
            return r;
        }

        private bool posible(int i, int[] estado){
            int p = posNum(estado,0);
            int tam = estado.Length;
            double t = float.Parse(tam + "");
            t = Math.Pow(t, 0.5);
            tam = Convert.ToInt32(t);

            int t1d = tam;
            if (i == 1){
                for (int o = 0; o < tam; o++){
                    if (p == o){
                        return false;
                    }
                }

            }
            if (i == 2){
                tam = (tam * tam) - t1d; 
                for (int o = tam; o < (estado.Length); o++){
                    if (p == o){
                        return false;
                    }
                }
            }
            if (i == 3){
                int a = t1d-1;
                for (int o = 0; o < t1d; o++){
                    if (p == a){
                        return false;
                    }
                    a = a + t1d;
                }
            }
            if (i == 4){
                int a =0;
                for (int o = 0; o < t1d; o++){
                    if (p == a){
                        return false;
                    }
                    a = a + t1d;
                }
            }
            return true;
        }

        private int posNum(int[] estado,int num){
            for (int i = 0; i <estado.Length;i++){
                if (estado[i] == num)
                {
                    return i;
                }
            }
            return 0;//esto nunca pasa
        }

        private Estado BorrarPrimero(List<Estado> lista){
            if (radioButton1.Checked||radioButton3.Checked){
                Estado n = lista.ElementAt(0);
                lista.RemoveAt(0);
                return n;
            }else{//tambien aplica cuando uso funcion euristica 
                Estado n = lista.ElementAt(lista.Count-1);
                lista.RemoveAt(lista.Count-1);
                return n;
            }
        }

        private bool pruevaObj(int[] estado){
            int t = estado.Length;
            int[] ver = CalcularNumeros(t,false);
            for (int i = 0; i< t; i++){
                if (!(estado[i] == ver[i])){
                    return false;
                }
            }
            return true;
        }

        private bool isvacio(List<Estado> lista)
        {
            bool r = false;
            if (lista.Count == 0)
            {
                r = true;
            }
            else
            {
                r = false;
            }
            return r;
        }

    }
}
