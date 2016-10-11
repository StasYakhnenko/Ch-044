﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goods.DbModels;

namespace Goods.Managers
{
    public class GoodDbWorker
    {
        public static string connectionstring = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
        public Good Get(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                string query = "SELECT Goods.Id, Goods.Name, Goods.Price, Categories.Id, Categories.Name, Producers.Id, Producers.Name, Producers.Country";
                query += " FROM Goods";
                query += " INNER JOIN Categories ON Goods.CategoryId = Categories.Id";
                query += " INNER JOIN Producers ON Goods.ProducerId = Producers.Id";
                query += " WHERE Goods.Id = @Id;";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@Id", SqlDbType.Int);
                command.Parameters["@Id"].Value = id;

                SqlDataReader reader = command.ExecuteReader();

                Good res = reader.Read()
                    ? new Good
                    {
                        Id = (int)reader.GetValue(0),
                        Name = reader.GetValue(1).ToString(),
                        Price = (decimal)reader.GetValue(2),
                        Category = new Category { Id = (int)reader.GetValue(3), Name = reader.GetValue(4).ToString() },
                        Producer = new Producer { Id = (int)reader.GetValue(5), Name = reader.GetValue(6).ToString(), Country = reader.GetValue(7).ToString() }
                    }
                    : null;
                reader.Close();

                return res;
            }
        }
        public List<Good> All()
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                var producerworker = new ProducerDbWorker();
                var categoriesworker = new CategoryDbWorker();
                List<Good> goods = new List<Good>();
                string sql = "SELECT * FROM dbo.Goods";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var good = new Good();
                            good.Id = Convert.ToInt32(reader["Id"]);
                            good.Name = reader["Name"].ToString();
                            good.Price = Convert.ToDecimal(reader["Price"]);
                            good.Category = new Category()
                            {
                                Id = Convert.ToInt32(reader["CategoryId"])
                            };
                            good.Producer = new Producer()
                            {
                                Id = Convert.ToInt32(reader["ProducerId"])
                            };

                            goods.Add(good);
                        }
                    }
                }
                return goods;
            }
        }
        public void Add(Good good)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    string sqlStatement = "INSERT INTO dbo.Goods(Id,Name,Price,CategoryId,ProducerId) VALUES(@Id,@Name,@Price,@CategoryId,@ProducerId)";
                    using (SqlCommand cmd = new SqlCommand(sqlStatement, connection))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int);
                        cmd.Parameters.Add("@Name", SqlDbType.Text);
                        cmd.Parameters.Add("@Price", SqlDbType.Decimal);
                        cmd.Parameters.Add("@CategoryId", SqlDbType.Int);
                        cmd.Parameters.Add("@ProducerId", SqlDbType.Int);

                        cmd.Parameters["@id"].Value = good.Id;
                        cmd.Parameters["@Name"].Value = good.Name;
                        cmd.Parameters["@Price"].Value = good.Price;
                        cmd.Parameters["@CategoryId"].Value = good.Category.Id;
                        cmd.Parameters["@ProducerId"].Value = good.Producer.Id;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error-It's impossible to insert good,check the values!");
            }
        }
        public void AddList(List<Good> goods)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    string sqlStatement = "INSERT INTO dbo.Goods(Id,Name,Price,CategoryId,ProducerId) VALUES(@Id,@Name,@Price,@CategoryId,@ProducerId)";
                    using (SqlCommand cmd = new SqlCommand(sqlStatement, connection))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int);
                        cmd.Parameters.Add("@Name", SqlDbType.Text);
                        cmd.Parameters.Add("@Price", SqlDbType.Decimal);
                        cmd.Parameters.Add("@CategoryId", SqlDbType.Int);
                        cmd.Parameters.Add("@ProducerId", SqlDbType.Int);

                        foreach (var item in goods)
                        {
                            cmd.Parameters["@id"].Value = item.Id;
                            cmd.Parameters["@Name"].Value = item.Name;
                            cmd.Parameters["@Price"].Value = item.Price;
                            cmd.Parameters["@CategoryId"].Value = item.Category.Id;
                            cmd.Parameters["@ProducerId"].Value = item.Producer.Id;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error-It's impossible to insert list of goods,check the values of keys!");
            }
        }
        public void Update(Good good)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    string sqlStatement = "UPDATE dbo.Goods SET Name=@Name,Price=@Price,CategoryId=@CategoryId,ProducerId=@ProducerId WHERE Id=@Id";
                    using (SqlCommand cmd = new SqlCommand(sqlStatement, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", good.Id);
                        cmd.Parameters.AddWithValue("@Name", good.Name);
                        cmd.Parameters.AddWithValue("@Price", good.Price);
                        cmd.Parameters.AddWithValue("@CategoryId", good.Category.Id);
                        cmd.Parameters.AddWithValue("@ProducerId", good.Producer.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error-It's impossible to update good because of value key");
            }
        }
        public void Delete(int Id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    string sqlStatement = "DELETE FROM dbo.Goods WHERE Id = " + Id.ToString() + "";
                    using (SqlCommand cmd = new SqlCommand(sqlStatement, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error-it's impossible to delete good because of values!!");
            }
        }

    }
}