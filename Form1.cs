using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.IO;

namespace Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
        private static void WriteFile(int UsersDeadlock, int DeadlocksTime, String UserType)
        {
            
            String FilePath = @"..\Records.txt";
            FileStream fileForRecords = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fileForRecords);
            
            sw.WriteLine(UserType);
            sw.WriteLine(UsersDeadlock);
            sw.WriteLine(DeadlocksTime);
            sw.Flush();
            sw.Close();
            fileForRecords.Close();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            int numberFromA = Convert.ToInt32(txtUserA.Text);
            int numberFromB = Convert.ToInt32(txtUserB.Text);

            for(int i = 0; i < numberFromA; i++)
            {
                Thread threadForA = new Thread(UserThreadA);
                threadForA.Start();
            }
            
            for (int i = 0; i < numberFromB; i++)
            {
                Thread threadForB = new Thread(UserThreadB);
                threadForB.Start();
            }
            
        }
        
        public void UserThreadA()
        {
            SqlConnection conForDatabase = new SqlConnection("Data Source=DESKTOP-S705IE6\\SQL2017;Initial Catalog=AdventureWorks2012;Integrated Security=True");
            SqlTransaction transactionForSql = null;
            SqlCommand command = new SqlCommand();

            DateTime beginTime = DateTime.Now;
            int UserADeadLock = 0;
            TimeSpan totalTime;

            for(int i=0; i<100; i++)
            {
                try
                {
                    conForDatabase.Open();
                    command.Connection = conForDatabase;
                    transactionForSql = conForDatabase.BeginTransaction(IsolationLevel.ReadUncommitted);

                    command.CommandText = ("UPDATE Sales.SalesOrderDetail " +
                            "SET UnitPrice = UnitPrice * 10.0 / 10.0 WHERE UnitPrice > " +
                            "100AND EXISTS(SELECT* FROM Sales.SalesOrderHeader WHERE Sales.SalesOrderHeader.SalesOrderID = " +
                            "Sales.SalesOrderDetail.SalesOrderID AND Sales.SalesOrderHeader.OrderDate BETWEEN" +
                            " @BeginDate AND @EndDate AND Sales.SalesOrderHeader.OnlineOrderFlag = 1)");
                    command.Transaction = transactionForSql;

                    Random random = new Random();


                    if (random.NextDouble() < 0.5)
                    {
                        command.Parameters.AddWithValue("@BeginDate", 20110101);
                        command.Parameters.AddWithValue("@EndDate", 20111231);
                        command.ExecuteNonQuery();
                    }
                    if (random.NextDouble() < 0.5)
                    {
                        
                        command.Parameters.AddWithValue("@BeginDate", 20120101);
                        command.Parameters.AddWithValue("@EndDate", 20121231);
                        command.ExecuteNonQuery();
                    }
                    if (random.NextDouble() < 0.5)
                    {
                        command.Parameters.AddWithValue("@BeginDate", 20130101);
                        command.Parameters.AddWithValue("@EndDate", 20131231);
                        command.ExecuteNonQuery();
                    }
                    if (random.NextDouble() < 0.5)
                    {
                        command.Parameters.AddWithValue("@BeginDate", 20140101);
                        command.Parameters.AddWithValue("@EndDate", 20141231);
                        command.ExecuteNonQuery();
                    }
                    if (random.NextDouble() < 0.5)
                    {
                        command.Parameters.AddWithValue("@BeginDate", 20150101);
                        command.Parameters.AddWithValue("@EndDate", 20151231);
                        command.ExecuteNonQuery();
                    }

                    transactionForSql.Commit();
                    conForDatabase.Close();
                    SqlConnection.ClearPool(conForDatabase);

                }
                catch(SqlException exception)
                {
                    UserADeadLock++;
                    transactionForSql.Rollback();
                }
                finally
                {
                    if (conForDatabase.State == ConnectionState.Open)
                    {
                        conForDatabase.Close();
                        SqlConnection.ClearPool(conForDatabase);
                    }
                }
            }
            DateTime endTime = DateTime.Now;
            totalTime = endTime - beginTime;
            int totalSecond = Convert.ToInt32(totalTime.TotalSeconds);

            //WriteFile(UserADeadLock, totalSecond, "User A");
            Console.WriteLine("User A's Deadlocks are: {0}", UserADeadLock);
            Console.WriteLine("User A's Total Time is: {0}", totalTime);

            if (conForDatabase.State == ConnectionState.Open)
                conForDatabase.Close();
        }
        public void UserThreadB()
        {
            SqlConnection conForDatabase = new SqlConnection("Data Source=DESKTOP-S705IE6\\SQL2017;Initial Catalog=AdventureWorks2012;Integrated Security=True");
            SqlTransaction transactionForSql = null;
            SqlCommand command = new SqlCommand();

            DateTime beginTime = DateTime.Now;
            int UserBDeadLock = 0;
            TimeSpan totalTime;

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    
                    conForDatabase.Open();
                    command.Connection = conForDatabase;
                    transactionForSql = conForDatabase.BeginTransaction(IsolationLevel.ReadUncommitted);
                    command.Transaction = transactionForSql;



                    command.CommandText = ("SELECT SUM(Sales.SalesOrderDetail.OrderQty) FROM Sales.SalesOrderDetail " +
                            "WHERE UnitPrice > 100 AND EXISTS(SELECT* FROM Sales.SalesOrderHeader WHERE " +
                            "Sales.SalesOrderHeader.SalesOrderID = Sales.SalesOrderDetail.SalesOrderID AND " +
                            "Sales.SalesOrderHeader.OrderDate BETWEEN @BeginDate AND @EndDate AND Sales.SalesOrderHeader.OnlineOrderFlag = 1)");

                    Random random = new Random();

                    if (random.NextDouble() < 0.5)
                    {
                        
                        command.Parameters.AddWithValue("@BeginDate", 20110101);
                        command.Parameters.AddWithValue("@EndDate", 20111231);
                        command.ExecuteNonQuery();
                    }

                    if (random.NextDouble() < 0.5)
                    {
                        command.Parameters.AddWithValue("@BeginDate", 20120101);
                        command.Parameters.AddWithValue("@EndDate", 20121231);
                        command.ExecuteNonQuery();
                    }
                    if (random.NextDouble() < 0.5)
                    {
                        command.Parameters.AddWithValue("@BeginDate", 20130101);
                        command.Parameters.AddWithValue("@EndDate", 20131231);
                        command.ExecuteNonQuery();
                    }
                    if (random.NextDouble() < 0.5)
                    {
                        command.Parameters.AddWithValue("@BeginDate", 20140101);
                        command.Parameters.AddWithValue("@EndDate", 20141231);
                        command.ExecuteNonQuery();
                    }
                    if (random.NextDouble() < 0.5)
                    {
                        command.Parameters.AddWithValue("@BeginDate", 20150101);
                        command.Parameters.AddWithValue("@EndDate", 20151231);
                        command.ExecuteNonQuery();
                    }

                    transactionForSql.Commit();
                    conForDatabase.Close();
                    SqlConnection.ClearPool(conForDatabase);

                }
                catch (SqlException exception)
                {
                    UserBDeadLock++;
                    transactionForSql.Rollback();
                }
                finally
                {
                    if (conForDatabase.State == ConnectionState.Open)
                    {
                        conForDatabase.Close();
                        SqlConnection.ClearPool(conForDatabase);
                    }

                }
            }
            DateTime endTime = DateTime.Now;
            totalTime = endTime - beginTime;
            int totalSecond = Convert.ToInt32(totalTime.TotalSeconds);

            //WriteFile(UserBDeadLock, totalSecond, "User B");
            Console.WriteLine("User B's Deadlocks are: {0}", UserBDeadLock);
            Console.WriteLine("User B's Total Time is: {0}", totalTime);

        }
    }
}
