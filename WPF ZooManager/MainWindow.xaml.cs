using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WPF_ZooManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();
            //connecting db to program: add reference System.configuration and then add projectname(replacing spaces with _).Properties.Settings.ConnectionString
            string connectionString = ConfigurationManager.ConnectionStrings["WPF_ZooManager.Properties.Settings.RadefeConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            ShowZoos();
            ShowAllAnimals();
        }

        private void ShowZoos()
        {
            try
            {
                string query = "select * from Zoo";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);
                    listZoos.DisplayMemberPath = "Location";
                    listZoos.SelectedValuePath = "Id";
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void ShowAnimals()
        {
            try
            {
                string query = "select * from Animal a inner join ZooAnimal za on a.Id = za.AnimalId where za.ZooId = @zooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@zooId", listZoos.SelectedValue);
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);
                    listAnimals.DisplayMemberPath = "Name";
                    listAnimals.SelectedValuePath = "Id";
                    listAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.Message);
            }
        }
        private void ShowAllAnimals()
        {
            try
            {
                string query = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);
                    Animals.DisplayMemberPath = "Name";
                    Animals.SelectedValuePath = "Id";
                    Animals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedInTextBox("select location from Zoo where Id = @ZooId", "@ZooId", listZoos, "Location");

            ShowAnimals();
        }
        private void listAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedInTextBox("select name from Animal where Id = @AnimalId", "@AnimalId", Animals, "Name");
        }
        private void DeleteZoo(object sender, RoutedEventArgs e)
        {
            DBChange("delete from Zoo where Id=@ZooId", "@ZooId", listZoos);

        }
        private void AddZoo_OnClick(object sender, RoutedEventArgs e)
        {
            DBChange("insert into Zoo values (@Location)", "@Location");
        }

        private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            DBChange("insert into ZooAnimal values (@ZooId, @AnimalId)", "@AnimalId", "@ZooId", Animals, listZoos);
        }
        private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            DBChange("delete from Animal where Id=@AnimalId", "@AnimalId", Animals);

        }
        private void RemoveFromZoo_Click(object sender, RoutedEventArgs e)
        {
            DBChange("delete from ZooAnimal where ZooId=@ZooId AND AnimalId=@AnimalId", "@AnimalId", "@ZooId", listAnimals, listZoos);
        }
        private void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            DBChange("insert into Animal values (@Name)", "@Name");
        }
        private void DBChange(string s, string value, ListBox box)
        {
            try
            {
                string query = s;
                SqlCommand sqlcommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlcommand.Parameters.AddWithValue(value, box.SelectedValue);
                sqlcommand.ExecuteScalar();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAllAnimals();
                ShowAnimals();
                ShowZoos();

            }
        }
        private void DBChange(string s, string value)
        {
            try
            {
                string query = s;
                SqlCommand sqlcommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlcommand.Parameters.AddWithValue(value, MyTextBox.Text);
                sqlcommand.ExecuteScalar();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAllAnimals();
                ShowAnimals();
                ShowZoos();
            }
        }
        private void DBChange(string s, string value1, string value2, ListBox one, ListBox two)
        {
            try
            {
                string query = s;
                SqlCommand sqlcommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlcommand.Parameters.AddWithValue(value1, one.SelectedValue);
                sqlcommand.Parameters.AddWithValue(value2, two.SelectedValue);
                sqlcommand.ExecuteScalar();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();

            }
        }
        private void DBChange(string s, string value1, string value2, ListBox list)
        {
            try
            {
                string query = s;
                SqlCommand sqlcommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlcommand.Parameters.AddWithValue(value1, list.SelectedValue);
                sqlcommand.Parameters.AddWithValue(value2, MyTextBox.Text);
                sqlcommand.ExecuteScalar();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
                ShowZoos();
                ShowAllAnimals();

            }
        }
        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            DBChange("update Zoo Set Location = @Location where Id = @ZooId", "@ZooId", "@Location", listZoos);
        }

        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            DBChange("update Animal Set Name = @Name where Id = @AnimalId", "@AnimalId", "@Name", Animals);
        }
        private void ShowSelectedInTextBox(string s, string value, ListBox list, string selection)
        {
            try
            {
                string query = s;
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue(value, list.SelectedValue);
                    DataTable Table = new DataTable();
                    sqlDataAdapter.Fill(Table);
                    MyTextBox.Text = Table.Rows[0][selection].ToString();
                }
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.Message);
            }
        }
    }
}
