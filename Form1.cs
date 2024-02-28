using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ventasexamen
{
    public partial class FormVentas : Form
    {
        private static int c1 = 0, c4 = 0, cM = 0, cont = 0;
        private static int sumM = 0, sumF = 0, sumNeto = 0;
        private static string connectionString = "Data Source=GALAGER;Initial Catalog=ventaslibros;Integrated Security=True";

        public FormVentas()
        {
            InitializeComponent();
        }

        private void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            Ingreso();
        }

        private void btnReportarVenta_Click(object sender, EventArgs e)
        {
            Reporte();
        }

        private void Ingreso()
        {
            double porc, precio, bruto, dcto, neto;
            int tipo, cant;
            char gen;

            do
            {
                gen = char.ToUpper(ObtenerGenero());
            } while (gen != 'M' && gen != 'F');

            tipo = ObtenerTipoLibro();
            cant = ObtenerCantidadLibros();

            switch (tipo)
            {
                case 1: precio = 90; break;
                case 2: precio = 100; break;
                case 3: precio = 80; break;
                case 4: precio = 150; break;
                default: precio = 0; break;
            }

            if (cant <= 2)
            {
                switch (tipo)
                {
                    case 1: porc = 0.05; break;
                    case 2: porc = 0.08; break;
                    case 3: porc = 0.09; break;
                    case 4: porc = 0.02; break;
                    default: porc = 0; break;
                }
            }
            else if (cant <= 6)
            {
                switch (tipo)
                {
                    case 1: porc = 0.06; break;
                    case 2: porc = 0.16; break;
                    case 3: porc = 0.18; break;
                    case 4: porc = 0.02; break;
                    default: porc = 0; break;
                }
            }
            else
            {
                switch (tipo)
                {
                    case 1: porc = 0.08; break;
                    case 2: porc = 0.32; break;
                    case 3: porc = 0.36; break;
                    case 4: porc = 0.04; break;
                    default: porc = 0; break;
                }
            }

            bruto = cant * precio;
            dcto = bruto * porc;
            neto = bruto - dcto;

            MessageBox.Show($"Importe a pagar : {bruto}\nDescuento : {dcto}\nImporte Neto : {neto}");

            if (tipo == 4)
                c4++;
            if (tipo == 1 && porc == 0.06)
                c1++;
            if (gen == 'M' && dcto >= 200 && dcto <= 2500)
                cM++;
            sumNeto += (int)neto;

            if (gen == 'F' && tipo == 2)
                sumF += (int)neto;

            if (gen == 'M' && tipo == 3)
            {
                sumM += (int)neto;
                cont++;
            }

            GuardarEnBaseDeDatos(gen, tipo, cant);
        }

        private void Reporte()
        {
            double prom;
            if (cont > 0)
                prom = sumM / cont;
            else
                prom = 0;

            MessageBox.Show($"Cantidad ventas de Fisica Cuantica : {c4}\nCantidad ventas de Ficcion y dcto 6% : {c1}\nCantidad ventas Varones y dcto [200,2500] : {cM}\nTotal Importe Neto : {sumNeto}\nTotal Neto Mujeres y Novelas : {sumF}\nPromedio Neto de Varones y Cuentos : {prom}");
        }

        private void FormVentas_Load(object sender, EventArgs e)
        {
            // Este método se llama cuando el formulario se carga
            // Puedes agregar cualquier inicialización necesaria aquí
        }

        private char ObtenerGenero()
        {
            MessageBox.Show("Ingrese el género del cliente (M: masculino, F: femenino)");
            return Console.ReadKey().KeyChar;
        }

        private int ObtenerTipoLibro()
        {
            int tipo;
            do
            {
                MessageBox.Show("Ingrese el tipo de libro (1: Ficcion, 2: Novelas, 3: Cuentos, 4: Fisica Cuantica)");
                tipo = int.Parse(Console.ReadLine());
            } while (tipo < 1 || tipo > 4);
            return tipo;
        }

        private int ObtenerCantidadLibros()
        {
            MessageBox.Show("Ingrese la cantidad de libros");
            return int.Parse(Console.ReadLine());
        }

        private void GuardarEnBaseDeDatos(char genero, int tipo, int cantidad)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO VentasLibros (GeneroCliente, TipoLibro, Cantidad) VALUES (@genero, @tipo, @cantidad)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@genero", genero);
                command.Parameters.AddWithValue("@tipo", tipo);
                command.Parameters.AddWithValue("@cantidad", cantidad);
                command.ExecuteNonQuery();
            }
        }
    }
}
